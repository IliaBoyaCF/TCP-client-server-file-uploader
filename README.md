# TCP-client-server-file-uploader

General
----

Application implements client side for uploading files to a specified server and server side for accepting uploadings. The mode in which application should work is specified by command line arguments. In server mode applicatoin shows information about current uploading sessions: current uploading speed and average uploading speed for session.

The src provides:

- Client package which can be used to implement separate client application.
- Server package which can be used to implement separate server application.
- _DownloadInfo_ class which can be gotten from any _ServerSession_ class. It contains information for current and average uploading speed for session. 
- Simple parser for command line arguments.
