@echo off
setlocal EnableDelayedExpansion

echo Installing Holder of Place mod base...
echo.

:: The directory containing this script
cd /d "%~dp0"
set BASE_DIR=%~dp0
set FORCE_COPY=0

echo Which option would you like^?
echo [A] Install
echo [B] Install from scratch
echo [C] Update only
choice /C ABC
set OPTION=%errorlevel%
if %OPTION% == 2 (
	set FORCE_COPY=1
)

::Pull files from GitHub (optional)  
choice /C YN /M "Would you like to look on GitHub for the latest version? (Highly recommended)"
if %errorlevel%==1 (
		call GithubPull.bat skip
	)
)


:: Attempt to find game directory in common places
if exist "C:\Program Files (x86)\Steam\steamapps\common\Holder of Place" set GAME_DIR="C:\Program Files (x86)\Steam\steamapps\common\Holder of Place"
if exist "C:\Program Files\Steam\steamapps\common\Holder of Place" set GAME_DIR="C:\Program Files\Steam\steamapps\common\Holder of Place"

:: If we failed in finding the game directory, ask for user input
if defined GAME_DIR (
    echo Found game directory at %GAME_DIR%
) else (
    echo Could not find Holder of Place automatically.
    set /p GAME_DIR=Please drag the Holder of Place directory into this terminal:
)

:: Strip quatation marks from paths because robocopy has issues when they are there
for %%I in (%GAME_DIR%) do set "GAME_DIR=%%~I"

:: State the used directory
echo Using directory:
echo %GAME_DIR%
echo.
:: ###################################################################################################

:: Check that all required installation files are present at the same location as this .bat file
echo Checking installer contents...
set MISSING_FILES=0

if not exist "%BASE_DIR%Dependencies\" (
    echo Missing: Dependencies folder!
    set MISSING_FILES=1
)

if not exist "%BASE_DIR%StreamingAssets\" (
    echo Missing: StreamingAssets folder!
    set MISSING_FILES=1
)

if not exist "%BASE_DIR%ModBootstrap.dll" (
    echo Missing: ModBootstrap.dll!
    set MISSING_FILES=1
)

if not exist "%BASE_DIR%DLLEditor.exe" (
    echo Missing: DLLEditor.exe!
    set MISSING_FILES=1
)

:: If any file is missing, abort
if %MISSING_FILES%==1 (
    echo.
    echo One or more required files are missing.
    echo Please ensure you have all required files.
    pause
    exit /b
)

echo All required files found.
echo.
:: ###################################################################################################

:: Doing a robocopy dry run, to ensure the destination path was formatted correctly
echo Starting robocopy dry run.
robocopy "%BASE_DIR%Dependencies" "%GAME_DIR%\HolderOfPlace_Data\Managed" *.dll /L /NFL /NJS
if %ERRORLEVEL% GEQ 8 (
    echo.
    echo ERROR: Dry run failed.
    pause
    exit /b
)

:: Ask user wether the destination looks correect
echo.
echo Please verify the destination directory. If it does not end with HolderOfPlace_Data\Managed, please abort now.
choice /C YN /M "Proceed?"
if %ERRORLEVEL%==2 (
    echo User aborted.
    pause
    exit /b
)

echo User confirmed correctness.
echo.
:: ###################################################################################################

:: Copy the StreamingAssets directory
echo Copying StreamingAssets to Data folder.
robocopy "%BASE_DIR%StreamingAssets" "%GAME_DIR%\HolderOfPlace_Data\StreamingAssets" /E /NJH /NJS
:: If error code is greater or equal to 8, copying failed
if %ERRORLEVEL% GEQ 8 (
    echo.
    echo ERROR: Copying StreamingAssets failed! Aborting.
    pause
    exit /b
)

:: Copy all the dependency dll files
echo Copying Dependencies to Managed folder.
robocopy "%BASE_DIR%Dependencies" "%GAME_DIR%\HolderOfPlace_Data\Managed" *.dll /NJH /NJS
:: If error code is greater or equal to 8, copying failed
if %ERRORLEVEL% GEQ 8 (
    echo.
    echo ERROR: Copying dependencies failed! Aborting.
    pause
    exit /b
)

:: Copy the ModBootstrap dll file
echo Copying ModBootstrap.dll to Managed folder.
copy /Y "%BASE_DIR%ModBootstrap.dll" "%GAME_DIR%\HolderOfPlace_Data\Managed\"
:: If error code is not equal to 0, copying failed
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Copying ModBoostrap.dll failed! Aborting.
    pause
    exit /b
)

::Update option ends prematurely
if %OPTION%==3 (
	echo Files successfully updated^!
	echo.
	pause
	exit
)

:: Create sub-directory for input Assembly-CSharp.dll
if not exist "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly" (
	set FORCE_COPY=1
	echo Creating sub-directory for default Assembly-CSharp.dll.
    mkdir "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly"
)

if %FORCE_COPY%==1 (
    :: Copy Assembly-CSharp.dll into new sub-directory
    echo Copying Assembly-CSharp.dll into sub-directory.
    copy /Y "%GAME_DIR%\HolderOfPlace_Data\Managed\Assembly-CSharp.dll" "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly\"
    :: If error code is not equal to 0, copying failed
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo ERROR: Copying Assembly-CSharp.dll failed! Aborting.
        pause
        exit /b
    )
) else (
    ::User already has a copy of the Assembly
    echo Assembly already copied, skipping.
)

echo All required files succesfully copied.
echo.
:: ###################################################################################################


:: Run DLLEditor to modify game files and enable modding
echo Running dll editor. Please wait a moment...
call "%BASE_DIR%DLLEditor.exe" "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly\Assembly-CSharp.dll" "%GAME_DIR%\HolderOfPlace_Data\Managed"
:: If error code is not equal to 0, the program threw an exception
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Modifying the game files failed! Code %ERRORLEVEL%.
    pause
    exit /b
)

:: ###################################################################################################

echo.
echo Modding base installation finished.
echo Keep this folder around to streamline updates.

pause