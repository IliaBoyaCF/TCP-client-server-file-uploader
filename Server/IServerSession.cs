using ClientServerApp.Protocol;
using System.Net;

namespace TCP_client_server_uploader.Server;

public interface IServerSession : ISession
{
    public enum State
    {
        WAITING_UPLOAD_REQUEST,
        DOWNLOADING_FILE,
        CLOSED,
    }

    public record DownloadInfo(double CurrentSpeed, double AverageSessionSpeed);

    State CurrentState { get; }

    UploadRequest ReceiveUploadRequest();

    void SendUploadResponse(UploadResponse response);

    void SendFinishResponse(FinishResponse finishResponse);

    FinishResponse DownloadFile(UploadRequest request);

    DownloadInfo GetDownloadInfo();

    EndPoint GetRemoteAddress();

}
