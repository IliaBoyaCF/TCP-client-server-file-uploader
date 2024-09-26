namespace ClientServerApp.Protocol;

public class UploadResponse : Message
{
    public enum UploadResponseValue : byte
    {
        ACCEPTED,
        NO_SPACE,
        REFUSED,
    }

    public static readonly int s_MinSize = 1;
    public static readonly int s_MaxSize = 1;
    public static readonly int s_ConstSize = 1;

    public UploadResponseValue Value { get; }

    public UploadResponse(UploadResponseValue value)
    {
        Value = value;
        serialized = [(byte)Value];
    }

    public override byte[] Serialize()
    {
        return serialized;
    }
}
