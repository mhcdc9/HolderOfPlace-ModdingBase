@echo off
setlocal EnableExtensions EnableDelayedExpansion

set UPDATE_MODE=0
::0 -> Don't Update
::1 -> Unknown Version (Update)
::2 -> Old Version (Update)

if not exist currentCatalog.txt (
	echo Missing: currentCatalog.txt. Creating default txt...
	echo Bootstrap 0 >> currentCatalog.txt
	echo StreamingAssets 0 >> currentCatalog.txt
	echo Dependencies 0 >> currentCatalog.txt
)
if "%1" neq "skip" (
	choice /C YN /M "Connect to GitHub and search for updates?
	if !errorlevel! EQU 2 (
		echo Download the required files manually and then run the installer.
		pause
		exit /b
	)
)

::Fetch catalog from latest release
echo Fetching Catalog...
for /f "tokens=1,* delims=:" %%A in ('curl -ks https://api.github.com/repos/mhcdc9/HolderOfPlace-ModdingBase/releases/latest ^| findstr "browser_download_url"') do (
	set url=%%B
	if not "!url:catalog.txt=!"=="!url!" (
		echo Downloading !url!
		curl -kOL !url!
	)
)

if not exist catalog.txt (
	echo Could not find the latest release catalog. Connection issue or contact developer.
	pause
	exit /b
)

::Find which files to download and download them
echo Checking files...
for /f "tokens=1,2" %%A in (catalog.txt) do (
	set TO_UPDATE=1
	for /f "tokens=1,2" %%a in (currentCatalog.txt) do (
		if %%A==%%a (
			set TO_UPDATE=0
			if not %%B == %%b (
				set TO_UPDATE=2
			)
		)
	)
	if !TO_UPDATE!==0 (
	echo %%A: Up To Date
	) else ( if !TO_UPDATE!==1 ( echo %%A: Unknown Version. Updating...
		) else ( 
			echo %%A: Old Version. Updating...
			if %%A==Installer (
			echo A new Updater is required! Please download a new one from the GitHub repository:
			echo https://github.com/mhcdc9/HolderOfPlace-ModdingBase
			pause
			exit
		)
		)
		echo Downloading %%B...
		curl -OL %%B
		if %%A==StreamingAssets (
			echo Unzipping [StreamingAssets.zip]...
			::Delete an old version of streamingassets
			del /Q "StreamingAssets" 2>nul
			call :UnZipFile "%~dp0" "%~dp0StreamingAssets.zip"
		)
		if %%A==Dependencies (
			echo Unzipping [Dependencies.zip]...
			::Delete an old version of dependencies
			del /Q "Dependencies" 2>nul
			call :UnZipFile "%~dp0" "%~dp0Dependencies.zip"
		)
	)
)

::Failsafe in case unzip files does not work
set NEED_TO_UNZIP=0
if not exist Dependencies (
	set /a NEED_TO_UNZIP+=1
	echo [Dependencies.zip] needs to be unzipped
)
if not exist StreamingAssets (
	set /a NEED_TO_UNZIP+=1
	echo [StreamingAssets.zip] needs to be unzipped
)
if %NEED_TO_UNZIP% neq 0 (
	echo Unzipping failed. Please unzip the %NEED_TO_UNZIP% file^(s^) above.
	set NEED_TO_UNZIP=0
	pause
)
set NEED_TO_UNZIP=0
if not exist Dependencies (
	set /a NEED_TO_UNZIP+=1
	echo [Dependencies] still not extracted.
)
if not exist StreamingAssets (
	set /a NEED_TO_UNZIP+=1
	echo [StreamingAssets] still not extracted.
)
if %NEED_TO_UNZIP% neq 0 (
	echo Files remain unzipped. Refusing to record changes.
	endlocal
	exit /b
)

::Everything is complete. Recording changes to avoid double-downloading
echo Recording changes in currentCatalog... 
move /Y catalog.txt currentCatalog.txt>nul
echo Download Complete. Ready to install.
pause
endlocal
exit /b

:UnZipFile <ExtractTo> <newzipfile>
set vbs="%temp%\_.vbs"
if exist %vbs% del /a /f %vbs%
>%vbs%  echo Set fso = CreateObject("Scripting.FileSystemObject")
>>%vbs% echo If NOT fso.FolderExists(%1) Then
>>%vbs% echo fso.CreateFolder(%1)
>>%vbs% echo End If
>>%vbs% echo set objShell = CreateObject("Shell.Application")
>>%vbs% echo set FilesInZip=objShell.NameSpace(%2).items
>>%vbs% echo call objShell.NameSpace(%1).CopyHere(FilesInZip,16)
>>%vbs% echo Set fso = Nothing
>>%vbs% echo Set objShell = Nothing
%SystemRoot%\System32\cscript.exe //nologo %vbs%
if exist %vbs% del %vbs%