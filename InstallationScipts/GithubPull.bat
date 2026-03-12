@echo off
setlocal EnableExtensions EnableDelayedExpansion

set TO_UPDATE=0

if not exist currentCatalog.txt (
	echo Missing: currentCatalog.txt. Creating default txt...
	echo Bootstrap 0 >> currentCatalog.txt
	echo StreamingAssets 0 >> currentCatalog.txt
	echo Dependencies 0 >> currentCatalog.txt
)

choice /C YN /M "Connect to GitHub and search for updates?
if %errorlevel% EQU 2 (
	echo Download the required files manually and then run the installer.
	pause
	exit /b
)

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

echo Checking files...

for /f "tokens=1,2" %%A in (catalog.txt) do (
	set TO_UPDATE=0
	for /f "tokens=1,2" %%a in (currentCatalog.txt) do (
		if %%A==%%a (
			if not %%B == %%b (
				set TO_UPDATE=1
				echo %%A: Updating from [%%B]...
				if %%A==Installer (
					echo A new Updater is required! Please download a new one from the GitHub repository:
					echo https://github.com/mhcdc9/HolderOfPlace-ModdingBase
					pause
					exit
				)
				echo Downloading %%B...
				curl -OL %%B
				if %%A==StreamingAssets (
					echo Unzipping [StreamingAssets.zip]...
					call :UnZipFile "%~dp0" "StreamingAssets.zip"
					del /Q "StreamingAssets.zip"
				)
				if %%A==Dependencies (
					echo Unzipping [Dependencies.zip]...
					call :UnZipFile "%~dp0" "Dependencies.zip"
					del /Q "Dependencies.zip"
				)
			)
		)
	)
	if !TO_UPDATE!==0 (
	echo %%A: Up To Date
	)
)
echo Replacing currentCatalog... 
move /Y catalog.txt currentCatalog.txt
echo Download Complete. Please run the installer
pause
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