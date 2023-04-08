cd C:\Users\Administrator\Desktop\altvBase\server
dotnet publish -c Release
cd C:\Users\Administrator\Desktop\altvServer

xcopy /y /e C:\AltV\resources\Release C:\Users\Administrator\Desktop\altvServer\resources\server

C:\Users\Administrator\Desktop\altvServer\altv-server.exe
