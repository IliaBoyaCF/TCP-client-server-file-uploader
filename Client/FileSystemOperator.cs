
namespace Client;

internal class FileSystemOperator : IFileSystemOperator
{
    private FileInfo? _fileInfo;

    public void Close()
    {
        if (_fileInfo == null)
        {
            throw new InvalidOperationException("No file was open");
        }
    }

    public long GetFileSize()
    {
        if (_fileInfo == null)
        {
            throw new InvalidOperationException("No file was open");
        }
        return _fileInfo.Length;
    }

    public Stream GetInputStream()
    {
        if (_fileInfo == null)
        {
            throw new InvalidOperationException("No file was open");
        }
        return File.OpenRead(_fileInfo.FullName);
    }

    public void OpenFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new ArgumentException("File not found.");
        }
        _fileInfo = new FileInfo(path);
    }
}