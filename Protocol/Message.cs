namespace ClientServerApp.Protocol;

public abstract class Message
{

    public enum MessageType
    {
        UPLOAD_REQUEST,
        UPLOAD_RESPONSE,
        FINISH_RESPONSE,
        NOT_A_MESSAGE,
    }

    protected byte[] serialized = [];
    public abstract byte[] Serialize();

}
