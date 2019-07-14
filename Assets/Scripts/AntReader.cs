using System;
using System.IO;
using System.Text;
using ANT_Managed_Library;
using UnityEngine;

public class AntReader : MonoBehaviour
{
    private bool bDone;


    public byte userAntChannel = 0; // ANT Channel to use
    public ushort userDeviceNum = 0; // Device number
    public byte userDeviceType = 120; // Device type

    public byte userTransmissionType = 0; // Transmission type
    public ushort userChannelPeriod = 8070;

    public byte userUserRadioFreq = 57; // RF Frequency + 2400 MHz

    //Network key is available from ANT+ member account
    static readonly byte[] USER_NETWORK_KEY = {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};
    static readonly byte USER_NETWORK_NUM = 0; // The network key is assigned to this network number

    private static ANT_Device _device;
    private static ANT_Channel _channel;

    private readonly RolloverHandler m_CurrentHeartBeatCountHandler = new RolloverHandler(8);
    private readonly RolloverHandler m_CurrentHeartBeatTimeRolloverHandler = new RolloverHandler(16);
    private double m_LastHeartBeatEventTime = -1;
    private double m_HeartBeatCount;

    private double m_LastRr;
    private double m_InitBeatCount = -1;

    public event OnNewHeartBeat onNewHeartBeat;

    // Start is called before the first frame update
    private void Awake()
    {
        CheckEditorVersion();
        AddPluginsToPath();
    }

    private static void AddPluginsToPath()
    {
        var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        var dllPath = Path.GetFullPath(Application.dataPath + "/" + "Plugins");


        if (currentPath != null && currentPath.Contains(dllPath) == false)
        {
            var newPath = currentPath;
            if (newPath.EndsWith(Path.PathSeparator.ToString()))
            {
                newPath += dllPath + Path.PathSeparator;
            }
            else
            {
                newPath += Path.PathSeparator + dllPath;
            }

            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Process);
        }
    }

    private static void CheckEditorVersion()
    {
#if UNITY_EDITOR_32
        Debug.Log("32");
#elif UNITY_EDITOR_64
        Debug.Log("64");
#endif
    }


    private void OnEnable()
    {
        InitAnt();
    }

    void Update()
    {
    }

    private void InitAnt()
    {
        try
        {
            String path = Application.dataPath + "/Plugins";
            ANT_Common.CustomDllSearchPath = Path.GetFullPath(path);
            _device = new ANT_Device();
            _device.deviceResponse += DeviceResponse;

            _channel = _device.getChannel(userAntChannel);
            _channel.channelResponse += ChannelResponse;

            System.Threading.Thread.Sleep(500);

            if (_device.setNetworkKey(USER_NETWORK_NUM, USER_NETWORK_KEY, 500))
                Debug.Log("Network key set");
            else
                throw new Exception("Error configuring network key");
            
            if (!_channel.assignChannel(ANT_ReferenceLibrary.ChannelType.BASE_Slave_Receive_0x00, USER_NETWORK_NUM,
                500))
                throw new Exception("Error assigning channel");

            if (_channel.setChannelID(userDeviceNum, false, userDeviceType, userTransmissionType, 500)
            ) // Not using pairing bit
                Debug.Log("Channel ID set");
            else
                throw new Exception("Error configuring Channel ID");


            if (_channel.setChannelFreq(userUserRadioFreq, 500))
                Debug.Log("Radio Frequency set");
            else
                throw new Exception("Error configuring Radio Frequency");

            Debug.Log("Setting Channel Period...");
            if (_channel.setChannelPeriod(userChannelPeriod, 500))
                Debug.Log("Channel Period set");
            else
                throw new Exception("Error configuring Channel Period");

            if (!_channel.openChannel(500))
            {
                throw new Exception("Error during opening channel");
            }

            _device.enableRxExtendedMessages(true);
        }
        catch (Exception e)
        {
            if (_device == null)
            {
                throw new Exception("Could not connect to any ANT device\nDetails:\n" + e);
            }

            throw new Exception("Error connecting to ANT: " + e.Message);
        }
    }



    public delegate void OnNewHeartBeat(HeartRateResponse heartRateResponse);

    void ChannelResponse(ANT_Response response)
    {
        switch ((ANT_ReferenceLibrary.ANTMessageID) response.responseID)
        {
            case ANT_ReferenceLibrary.ANTMessageID.RESPONSE_EVENT_0x40:
            {
                //TODO: Handle many EVENT_RX_FAIL (close channel, throw exception?) 
                Debug.Log(response.getChannelEventCode());
                break;
            }
            case ANT_ReferenceLibrary.ANTMessageID.BROADCAST_DATA_0x4E:
            case ANT_ReferenceLibrary.ANTMessageID.ACKNOWLEDGED_DATA_0x4F:
            case ANT_ReferenceLibrary.ANTMessageID.BURST_DATA_0x50:
            case ANT_ReferenceLibrary.ANTMessageID.EXT_BROADCAST_DATA_0x5D:
            case ANT_ReferenceLibrary.ANTMessageID.EXT_ACKNOWLEDGED_DATA_0x5E:
            case ANT_ReferenceLibrary.ANTMessageID.EXT_BURST_DATA_0x5F:
            {
                var dataPayload = response.getDataPayload();
                ExtractHeartRateInfo(dataPayload);
                break;
            }
            default:
            {
                Debug.Log("Unknown Message " + response.responseID);
                break;
            }
        }
    }

    //FIXME: Event time -   
    private void ExtractHeartRateInfo(byte[] dataPayload)
    {
        double currBeatCount = RolloverHandler.HandleRollover(dataPayload[6], m_CurrentHeartBeatCountHandler);
        var currBeatTime = GetBeatTime(dataPayload);


        if (m_InitBeatCount < 0)
        {
            m_InitBeatCount = currBeatCount;
        }

        currBeatCount = currBeatCount - m_InitBeatCount + 1;

        int hr = dataPayload[7];
        var response = HeartRateResponse.builder(hr, currBeatCount, currBeatTime);
        AddRrParameters(response);
        var heartRateResponse = response.Build();
        if (m_HeartBeatCount < heartRateResponse.HeartRateBeatCount)
        {
            onNewHeartBeat?.Invoke(heartRateResponse);
        }
        
        m_LastHeartBeatEventTime = response.lastHeartRateBeatTime;
        m_HeartBeatCount = response.heartRateBeatCount;
        Debug.Log(heartRateResponse);
    }

    private void AddRrParameters(HeartRateResponse.Builder response)
    {
        //if new heart beat and not first then recalculate RR and HRV
        if (m_HeartBeatCount < response.heartRateBeatCount && m_LastHeartBeatEventTime > 0)
        {
            var rr = response.lastHeartRateBeatTime - m_LastHeartBeatEventTime;
            rr = rr * 1000 / 1024;

            m_LastRr = rr;
        }

        response.WithRr(m_LastRr);
    }

    private double GetBeatTime(byte[] dataPayload)
    {
        int currBeatTime = dataPayload[4];
        currBeatTime |= dataPayload[5] << 8;
        return RolloverHandler.HandleRollover(currBeatTime, m_CurrentHeartBeatTimeRolloverHandler);
    }

    private void OnDisable()
    {
        _channel?.closeChannel();
        _device?.Dispose();
    }

    void DeviceResponse(ANT_Response response)
    {
        switch ((ANT_ReferenceLibrary.ANTMessageID) response.responseID)
        {
            case ANT_ReferenceLibrary.ANTMessageID.STARTUP_MESG_0x6F:
            {
                Debug.Log("RESET Complete, reason: ");

                var ucReason = response.messageContents[0];

                if (ucReason == (byte) ANT_ReferenceLibrary.StartupMessage.RESET_POR_0x00)
                    Debug.Log("RESET_POR");
                if (ucReason == (byte) ANT_ReferenceLibrary.StartupMessage.RESET_RST_0x01)
                    Debug.Log("RESET_RST");
                if (ucReason == (byte) ANT_ReferenceLibrary.StartupMessage.RESET_WDT_0x02)
                    Debug.Log("RESET_WDT");
                if (ucReason == (byte) ANT_ReferenceLibrary.StartupMessage.RESET_CMD_0x20)
                    Debug.Log("RESET_CMD");
                if (ucReason == (byte) ANT_ReferenceLibrary.StartupMessage.RESET_SYNC_0x40)
                    Debug.Log("RESET_SYNC");
                if (ucReason == (byte) ANT_ReferenceLibrary.StartupMessage.RESET_SUSPEND_0x80)
                    Debug.Log("RESET_SUSPEND");
                break;
            }
            case ANT_ReferenceLibrary.ANTMessageID.VERSION_0x3E:
            {
                Debug.Log("VERSION: " + new ASCIIEncoding().GetString(response.messageContents));
                break;
            }
            case ANT_ReferenceLibrary.ANTMessageID.RESPONSE_EVENT_0x40:
            {
                switch (response.getMessageID())
                {
                    case ANT_ReferenceLibrary.ANTMessageID.CLOSE_CHANNEL_0x4C:
                    {
                        if (response.getChannelEventCode() ==
                            ANT_ReferenceLibrary.ANTEventID.CHANNEL_IN_WRONG_STATE_0x15)
                        {
                            Debug.Log("Channel is already closed");
                            Debug.Log("Unassigning Channel...");
                            if (_channel.unassignChannel(500))
                            {
                                Debug.Log("Unassigned Channel");
                                Debug.Log("Press enter to exit");
                                bDone = true;
                            }
                        }

                        break;
                    }
                    case ANT_ReferenceLibrary.ANTMessageID.NETWORK_KEY_0x46:
                    case ANT_ReferenceLibrary.ANTMessageID.ASSIGN_CHANNEL_0x42:
                    case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_ID_0x51:
                    case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_RADIO_FREQ_0x45:
                    case ANT_ReferenceLibrary.ANTMessageID.CHANNEL_MESG_PERIOD_0x43:
                    case ANT_ReferenceLibrary.ANTMessageID.OPEN_CHANNEL_0x4B:
                    case ANT_ReferenceLibrary.ANTMessageID.UNASSIGN_CHANNEL_0x41:
                    {
                        if (response.getChannelEventCode() != ANT_ReferenceLibrary.ANTEventID.RESPONSE_NO_ERROR_0x00)
                        {
                            Debug.Log(String.Format("Error {0} configuring {1}", response.getChannelEventCode(),
                                response.getMessageID()));
                        }

                        break;
                    }
                    case ANT_ReferenceLibrary.ANTMessageID.RX_EXT_MESGS_ENABLE_0x66:
                    {
                        if (response.getChannelEventCode() == ANT_ReferenceLibrary.ANTEventID.INVALID_MESSAGE_0x28)
                        {
                            Debug.Log("Extended messages not supported in this ANT product");
                            break;
                        }
                        else if (response.getChannelEventCode() !=
                                 ANT_ReferenceLibrary.ANTEventID.RESPONSE_NO_ERROR_0x00)
                        {
                            Debug.Log(String.Format("Error {0} configuring {1}", response.getChannelEventCode(),
                                response.getMessageID()));
                            break;
                        }

                        Debug.Log("Extended messages enabled");
                        break;
                    }
                    case ANT_ReferenceLibrary.ANTMessageID.REQUEST_0x4D:
                    {
                        if (response.getChannelEventCode() == ANT_ReferenceLibrary.ANTEventID.INVALID_MESSAGE_0x28)
                        {
                            Debug.Log("Requested message not supported in this ANT product");
                            break;
                        }

                        break;
                    }
                    default:
                    {
                        Debug.Log("Unhandled response " + response.getChannelEventCode() + " to message " +
                                  response.getMessageID());
                        break;
                    }
                }

                break;
            }
        }
    }
}