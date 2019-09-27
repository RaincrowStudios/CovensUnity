:: 1. install the SDK: https://cloud.google.com/sdk/docs/
:: 2. run the authentication "gcloud auth login --brief"
:: 3. run the script

@echo off
@echo.

if [%1]==[] goto usage

SET PROJ_PATH=%CD%\..\
SET SRC_PATH=%PROJ_PATH%Dictionaries

SET DEST_FOLDER=dictionary/release/localization
SET DEST_PATH=gs://raincrow-covens/%DEST_FOLDER%/

::del %1 /q
::rd %1 /q

xcopy %SRC_PATH% %CD%\%1 /s /q /i /y /EXCLUDE:exclude

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
call gsutil -m -h "Content-Type:application/json; charset=utf-8" -h "Cache-Control:no-cache" cp -a public-read -r %1 %DEST_PATH% 
rmdir /s /q %1

pause
goto end

:usage
@echo Usage: %0 ^<version^>
@echo.
pause

:end