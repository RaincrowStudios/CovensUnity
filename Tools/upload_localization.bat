:: 1. install the SDK: https://cloud.google.com/sdk/docs/
:: 2. run the authentication "gcloud auth login --brief"
:: 3. run the script

@echo off
@echo.

if [%1]==[] goto usage
if [%2]==[] goto usage

SET PROJ_PATH=%CD%\..\
SET SRC_PATH=%PROJ_PATH%\Covens\Assets\Resources\Localization

SET DEST_FOLDER=dictionary/%1/localization
SET DEST_PATH=gs://raincrow-covens/%DEST_FOLDER%/

::del %1 /q
::rd %1 /q

xcopy %SRC_PATH% %CD%\%2 /s /q /i /y /EXCLUDE:exclude

:: SET FCIV=%CD%\External\fciv.exe
:: SET MD5=%CD%\External\md5.bat

::cd Temp
::ren *.json *.
::ren *.xml *.
::ren *.txt *.

::@echo generating hashes...
::for %%a in (%CD%\*) do call %MD5% "%%~a" >> MD5

::cd ..

::call gsutil -m rm -r %DEST_PATH%
call gsutil -m -h "Content-Type:application/json; charset=utf-8" -h "Cache-Control:no-cache" cp -Z -a public-read -r %2 %DEST_PATH% 
rmdir /s /q %2

pause
goto end

:usage
@echo Usage: %0 ^<environment^> ^<version^>
@echo.
pause

:end