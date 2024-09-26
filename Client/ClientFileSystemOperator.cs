
namespace TCP_client_server_uploader.Client;

internal class ClientFileSystemOperator : FileSystemOperator
{
    public override void SelectFile(FileInfo fileInfo)
    {
        if (!fileInfo.Exists)
        {
            throw new ArgumentException("File doesn't exist.");
        }
        _fileInfo = fileInfo;
    }
}
