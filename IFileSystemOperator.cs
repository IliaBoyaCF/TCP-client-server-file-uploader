namespace TCP_client_server_uploader;

public interface IFileSystemOperator : IDisposable
{
    public enum Mode
    {
        READ,
        WRITE,
    }

    // Opens file if exists or creates a new one.
    void SelectFile(FileInfo fileInfo);

    FileStream GetStream(Mode mode);

    long GetFileSize();

}