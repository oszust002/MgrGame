using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Filtering;
using MathNet.Filtering.IIR;

public class SensorController : MonoBehaviour
{
    [Header("Unity objects")]
    public BitalinoManager manager;
    public BitalinoReader reader;

    [Header("Parameters")]
    public float calibrationTime = 30f;
    public float calibrationWaitTime = 3f;
    public float thresholdMultiplier = 1.1f;

    [Header("Debug purposes")]
    public float meanEmg;
    public float calmEmg = 0;
    
    private bool calibrated = false;


    public float FullCalibrationTime => calibrationTime + calibrationWaitTime;

    public bool FinishedCalibration => reader.asStart && calibrated;

    private void Start()
    {
        GetCommandLineArgs();
    }

    public event OnEmgThresholdPassed onThresholdPassed;
    
    public delegate void OnEmgThresholdPassed(float value);

    private void GetCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        int portArgIndex = Array.FindIndex(args, x => x == "--bitalino-port" || x == "-bp");
        if (portArgIndex != -1 && args.Length > portArgIndex)
        {
            reader.manager.scriptSerialPort.portName = args[portArgIndex + 1];
        }
    }

    private void OnEnable()
    {
        calibrated = false;
        calmEmg = 0;
        meanEmg = 0;
        reader.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!FinishedCalibration) return;
        BITalinoFrame[] buffer = reader.getBuffer();
        if (buffer != null && buffer.Length > 0)
        {
            meanEmg = GetAverageEmg(buffer);
            if (meanEmg > thresholdMultiplier * calmEmg)
            {
                onThresholdPassed?.Invoke(meanEmg);
            }
        }
    }

    public IEnumerator StartReadings()
    {
        reader.enabled = true;
        while (!reader.asStart)
            yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(calibrationWaitTime);
        var timeLeft = calibrationTime;
        var samples = 0;
        while (timeLeft > 0)
        {
            var currentTime = Time.time;
            BITalinoFrame[] buffer = reader.getBuffer();
            calmEmg += GetAverageEmg(buffer);
            samples++;
            yield return new WaitForSeconds(0.3f);
            timeLeft -= Time.time - currentTime;
        }
        calmEmg = samples != 0 ? calmEmg / samples : 0;
        calibrated = true;
    }

    private float GetAverageEmg(BITalinoFrame[] buffer)
    {
        return buffer.Select(x => Mathf.Abs((float) x.GetAnalogValue(0))).Average();
    }

    private void OnDisable()
    {
        reader.enabled = false;
    }

    public void StartCalibration()
    {
        StartCoroutine(StartReadings());
    }
}