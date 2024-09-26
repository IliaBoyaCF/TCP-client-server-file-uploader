using System.Buffers;
using System.Text;

namespace ClientServerApp.Protocol;

public class UploadRequest : Message
{
    public static readonly int s_maxFileNameLength = 4096;
    public static readonly long s_maxFileSize = 1024L * 1024L * 1024L * 1024L;

    public static readonly int s_MinSize = 11;
    public static readonly int s_MaxSize = s_maxFileNameLength + 2 + 8;

    public string FileName { get; }
    public long FileSize { get; }

    public UploadRequest(string fileName, long fileSize) 
    { 
        if (!MeetConstraints(fileName, fileSize))
        {
            throw new ArgumentException("Provided arguments do not meet constraints.");
        }
        FileName = fileName;
        FileSize = fileSize;
        ArrayBufferWriter<byte> arrayBufferWriter = new();
        arrayBufferWriter.Write(BitConverter.GetBytes((short)fileName.Length).AsSpan());
        arrayBufferWriter.Write(Encoding.UTF8.GetBytes(fileName).AsSpan());
        arrayBufferWriter.Write(BitConverter.GetBytes(fileSize).AsSpan());
        serialized = arrayBufferWriter.WrittenSpan.ToArray();
    }

    public bool MeetConstraints(string fileName, long fileSize)
    {
        return fileName.Length > 0 && fileSize >= 0 && fileSize <= s_maxFileSize && fileName.Length <= s_maxFileNameLength;
    }

    public override byte[] Serialize()
    {
        return serialized;
    }

    public override string ToString()
    {
        return $"name : '{FileName}', name length: '{FileName.Length}', file size: '{FileSize}'";
    }
}
