using ClientServerApp.Protocol;

namespace TCP_client_server_uploader.Client;

public interface IClientSession : ISession
{
    public enum State
    {
        WAITING_FILE_TO_BE_ATTACHED,
        READY_TO_SEND_UPLOAD_REQUEST,
        WAITING_SERVER_RESPONSE,
        READY_TO_UPLOAD,
        FINISHED_UPLOADING,
    }

    State CurrentState { get; }

    void SelectFileToUpload(FileInfo path);

    void SendUploadRequest(UploadRequest uploadRequest);

    UploadResponse ReceiveUploadResponse();

    void UploadFile();

    FinishResponse ReceiveFinishResponse();

}
