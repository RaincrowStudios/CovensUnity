:: 1. install the SDK: https://cloud.google.com/sdk/docs/
:: 2. run the authentication "gcloud auth login --brief"
:: 3. run the script

@echo off
@echo.

if [%1]==[] goto usage
::if [%2]==[] goto usage

SET PROJ_PATH=%CD%\..\
SET SRC_PATH=%PROJ_PATH%\CovensAssetBundles\AssetBundles\%1

SET DEST_FOLDER=assetbundles
SET DEST_PATH=gs://raincrow-covens/%DEST_FOLDER%/

xcopy %SRC_PATH% %CD%\%1 /s /q /i /y /EXCLUDE:exclude

call gsutil -m -h "Content-Type:application/json; charset=utf-8" -h "Cache-Control:no-cache" cp -Z -a public-read -r %1 %DEST_PATH% 
rmdir /s /q %1

pause
goto end

:usage
@echo Usage: %0 ^<Android^|iOS^>
@echo.
pause

:end