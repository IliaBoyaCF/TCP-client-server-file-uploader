namespace TCP_client_server_uploader.Server;

internal class ServerFileSystemOperator : FileSystemOperator
{
    public static readonly string s_UploadsDirectory = "./uploads/";
    private DriveInfo? _driveInfo;

    public ServerFileSystemOperator()
    {
        if (!Directory.Exists(s_UploadsDirectory)) {
            Directory.CreateDirectory(s_UploadsDirectory);
        }
    }

    public override void SelectFile(FileInfo fileInfo)
    {
        _fileInfo = new FileInfo(s_UploadsDirectory + fileInfo.Name);
        if (_fileInfo.Exists)
        {
            ChangeNameToBeUnique();
        }
        _fileInfo.Create().Dispose();
    }

    public bool CanSaveFile(long size)
    {
        if (_fileInfo == null)
        {
            throw new InvalidOperationException("No file was selected.");
        }
        if (_driveInfo == null)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            string? root = Path.GetPathRoot(_fileInfo.FullName);

            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == root)
                {
                    _driveInfo = d;
                    break;
                }
            }
            if (_driveInfo == null)
            {
                throw new Exception("Couldn't find root.");
            }
        }
        return _driveInfo.AvailableFreeSpace - size > 0;
    }

    private void ChangeNameToBeUnique()
    {
        if (_fileInfo == null)
        {
            throw new NullReferenceException("_fileInfo shouldn't be null here");
        }
        while (_fileInfo.Exists)
        {
            _fileInfo = new FileInfo(InsertNextNumberInName(_fileInfo.FullName));
        }
    }

    public override long GetFileSize()
    {
        if (_fileInfo == null)
        {
            throw new InvalidOperationException("No file was open");
        }
        _stream?.Flush();

        _fileInfo.Refresh();
        return _fileInfo.Length;
    }

    private string InsertNextNumberInName(string fullName)
    {
        for (int i = fullName.Length - 1; (fullName[i] != Path.DirectorySeparatorChar || fullName[i] != Path.AltDirectorySeparatorChar) && i >= 0; 
                                        i--)
        {
            if (fullName[i] == '.')
            {
                return fullName.Substring(0, i) + "_Other_" + fullName.Substring(i);
            }
        }
        return fullName + "_Other_";
    }
}
