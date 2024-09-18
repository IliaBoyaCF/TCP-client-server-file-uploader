using System.Text;

namespace Server.Protocol;

public class MessageParser
{
    public static FinishResponse ParseFinishResponse(byte[] bytes)
    {
        Validate(bytes, FinishResponse.s_MinSize, FinishResponse.s_MaxSize);
        return new FinishResponse((FinishResponse.FinishResponseValue)bytes[0]);
    }

    private static void Validate(byte[] bytes, int minSize, int maxSize)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException("bytes");
        }
        if (!(bytes.Length >= minSize && bytes.Length <= maxSize))
        {
            throw new ArgumentException("Provided sequence is not a message.");
        }
    }

    public static UploadResponse ParseUploadResponse(byte[] bytes)
    {
        Validate(bytes, UploadResponse.s_MinSize, UploadResponse.s_MaxSize);
        return new UploadResponse((UploadResponse.UploadResponseValue)bytes[0]);
    }

    public static UploadRequest ParseUploadRequest(byte[] bytes)
    {
        Validate(bytes, UploadRequest.s_MinSize, UploadRequest.s_MaxSize);
        Stream stream = new MemoryStream(bytes);
        BinaryReader reader = new(stream);
        short fileNameLength = reader.ReadInt16();
        string fileName = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));
        long fileSize = reader.ReadInt64();
        return new UploadRequest(fileName, fileSize);
    }
}
