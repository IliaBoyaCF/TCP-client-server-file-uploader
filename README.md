# TCP-client-server-file-uploader

General
----

Application implements client side for uploading files to a specified server and server side for accepting uploadings. The mode in which application should work is specified by command line arguments. In server mode applicatoin shows information about current uploading sessions: current uploading speed and average uploading speed for session.

The src provides:

- Client package which can be used to implement separate client application.
- Server package which can be used to implement separate server application.
- _DownloadInfo_ class which can be gotten from any _ServerSession_ class. It contains information for current and average uploading speed for session. 
- Simple parser for command line arguments.

Usage
----    
    $ upload <flag> <args>
```
Flags can be skipped as following:
    > upload file_path server_address server_port
is the same as:
    > upload -c file_path server_address server_port
Flags:
    -c Start program as client for uploading.
        After specifying this flag you have to pass the following arguments: file_path server_address server_port
    -s Start program as server for downloading files from clients.
        After specifying this flag you have to pass the following arguments: server_port
Arguments:
    file_path: Specifying for client mode. 
                The path to the file client wants to upload.
    server_address: Specifying for client mode. 
                    The hostname, IPv4, IPv6 address of the server to which client wants to upload file.
    server_port: In client mode: port of the server to which client wants to upload file. 
                In server mode: port for listening for incoming connections.
```