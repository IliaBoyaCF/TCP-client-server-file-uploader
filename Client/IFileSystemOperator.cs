namespace Client;

public interface IFileSystemOperator
{
    void OpenFile(string path);

    Stream GetInputStream();

    long GetFileSize();

    void Close();
}