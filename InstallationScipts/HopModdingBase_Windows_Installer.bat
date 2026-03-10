@echo off

echo Installing Holder of Place mod base...
echo.

:: The directory containing this script
set BASE_DIR=%~dp0

:: Attempt to find game directory in common places
if exist "C:\Program Files (x86)\Steam\steamapps\common\Holder of Place" set GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Holder of Place
if exist "C:\Program Files\Steam\steamapps\common\Holder of Place" set GAME_DIR=C:\Program Files\Steam\steamapps\common\Holder of Place


:: If we failed in finding the game directory, ask for user input
if defined GAME_DIR (
    echo Found game directory at "%GAME_DIR%"
) else (
    :: Changed this to abort since taking manual input was causing a lot of issues
    echo Could not find Holder of Place directory automatically. Aborting.
    echo If you are having issues using the installer, make it known on Discord or create a GitHub Issue
    
    pause
    exit /b
)

echo Using directory:
echo %GAME_DIR%
echo.

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

:: Copy the StreamingAssets directory
echo Copying StreamingAssets to Data folder.
robocopy "%BASE_DIR%StreamingAssets" "%GAME_DIR%\HolderOfPlace_Data\StreamingAssets" /E /NJH /NJS
:: If error code is greater or equal to 8, copying failed
if %ERRORLEVEL% GEQ 8 (
    echo ERROR: Copying StreamingAssets failed! Aborting.
    pause
    exit /b
)

:: Copy all the dependency dll files
echo Copying Dependencies to Managed folder.
robocopy "%BASE_DIR%Dependencies" "%GAME_DIR%\HolderOfPlace_Data\Managed" *.dll /NJH /NJS
:: If error code is greater or equal to 8, copying failed
if %ERRORLEVEL% GEQ 8 (
    echo ERROR: Copying dependencies failed! Aborting.
    pause
    exit /b
)

:: Copy the ModBootstrap dll file
echo Copying ModBootstrap.dll to Managed folder.
copy /Y "%BASE_DIR%ModBootstrap.dll" "%GAME_DIR%\HolderOfPlace_Data\Managed\"
:: If error code is not equal to 0, copying failed
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Copying ModBoostrap.dll failed! Aborting.
    pause
    exit /b
)

:: Create sub-directory for input Assembly-CSharp.dll
if not exist "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly" (
    echo Creating sub-directory for default Assembly-CSharp.dll.
    mkdir "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly"

    :: Copy Assembly-CSharp.dll into new sub-directory
    echo Copying Assembly-CSharp.dll into sub-directory.
    copy /Y "%GAME_DIR%\HolderOfPlace_Data\Managed\Assembly-CSharp.dll" "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly\"
    :: If error code is not equal to 0, copying failed
    if %ERRORLEVEL% NEQ 0 (
        echo ERROR: Copying Assembly-CSharp.dll failed! Aborting.
        pause
        exit /b
    )
) else (
    ::User already has a copy of the Assembly
    echo Assembly already copied, skipping.
)
echo.


:: Run DLLEditor to modify game files and enable modding
echo Running dll editor.
call "%BASE_DIR%DLLEditor.exe" "%GAME_DIR%\HolderOfPlace_Data\Managed\DefaultAssembly\Assembly-CSharp.dll" "%GAME_DIR%\HolderOfPlace_Data\Managed"

echo.
echo Modding base installation finished.
echo You can now safely delete this installation package.

pause