namespace Server.Protocol;

public class FinishResponse : Message
{
    public enum FinishResponseValue : byte
    {
        SUCCESS,
        FAILURE,
    }
    public static readonly int s_MinSize = 1;
    public static readonly int s_MaxSize = 1;
    public FinishResponseValue Value { get; }

    public FinishResponse(FinishResponseValue value)
    {
        Value = value;
        serialized = [(byte)Value];
    }

    public override byte[] Serialize()
    {
        return serialized;
    }
}
