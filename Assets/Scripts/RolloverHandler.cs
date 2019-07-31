public class RolloverHandler
{
    private readonly int _bitLength;
    private int _rolloverCount;
    private ulong _basicValue;

    public RolloverHandler(int bitLength)
    {
        _bitLength = bitLength;
    }

    public ulong Value
    {
        set
        {
            if (_basicValue > value)
            {
                _rolloverCount++;
            }

            _basicValue = value;
        }

        get
        {
            var rollover = (ulong) (_rolloverCount * (1 << _bitLength));
            return rollover +_basicValue;
        }
    }

    public static ulong HandleRollover(int value, RolloverHandler handler)
    {
        handler.Value = (ulong) value;
        return handler.Value;
    }
}