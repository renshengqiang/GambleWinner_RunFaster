set PROTO_PATH=.\Proto
set PROTO2CS_PATH=.\Proto2CSharp
set CS_DEST=..\..\clientProj\Assets\Scripts\Protocal

@echo off
set PROTO_DESC_POSTFIX=.protodesc
set ROOT_PATH=%~dp0
if not exist %PROTO_PATH% (echo "PROTO_PATH is not exist" && goto :end)
if not exist %PROTO2CS_PATH% (echo "PROTO2CS_PATH is not exist" && goto :end)
if not exist %CS_DEST% (echo "CS_DEST is not exist" && goto :end)

::---------------------------------------------------
::first step: proto -> cs
::---------------------------------------------------
@echo on
cd %PROTO2CS_PATH%
dir %ROOT_PATH%\%PROTO_PATH%\*.proto /b  > protolist.txt

@echo on
for /f "delims=." %%i in (protolist.txt) do protoc --descriptor_set_out=%%i%PROTO_DESC_POSTFIX% --proto_path=%ROOT_PATH%\%PROTO_PATH% %ROOT_PATH%\%PROTO_PATH%\%%i.proto 
for /f "delims=." %%i in (protolist.txt) do ProtoGen\protogen -i:%%i%PROTO_DESC_POSTFIX% -o:%%i.cs 

::---------------------------------------------------
::second step: copy cs files
::---------------------------------------------------
@echo on
cd %ROOT_PATH%
copy %PROTO2CS_PATH%\*.cs %CS_DEST%

::---------------------------------------------------
::last step: clear temp files
::---------------------------------------------------
@echo off
cd %ROOT_PATH%\%PROTO2CS_PATH%
del *.cs
del *.protodesc
del *.txt

:end
pause