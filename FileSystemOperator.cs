namespace TCP_client_server_uploader;

public abstract class FileSystemOperator : IFileSystemOperator
{
    protected FileInfo? _fileInfo;
    protected FileStream? _stream;

    public virtual void Dispose()
    {
        if (_stream != null)
        {
            _stream.Dispose();
        }
    }

    public virtual long GetFileSize()
    {
        if (_fileInfo == null)
        {
            throw new InvalidOperationException("No file was open");
        }
        return _fileInfo.Length;
    }

    public FileStream GetStream(IFileSystemOperator.Mode mode)
    {
        if (_fileInfo == null) 
        {
            throw new InvalidOperationException("No file has been selected yet.");
        }
        if (_stream != null)
        {
            throw new InvalidOperationException("Other stream has been opened already.");
        }
        if (mode == IFileSystemOperator.Mode.WRITE)
        {
            _stream = _fileInfo.OpenWrite();
            return _stream;
        }
        if (mode == IFileSystemOperator.Mode.READ)
        {
            _stream = _fileInfo.OpenRead();
            return _stream;
        }
        throw new ArgumentException("Unknown mode.");
    }

    public abstract void SelectFile(FileInfo fileInfo);
}