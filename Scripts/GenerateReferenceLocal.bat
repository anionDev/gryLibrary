call Build.bat
if %errorlevel% neq 0 exit /b %errorlevel%
pushd ..\GRYLibrary
docfx docfx.json
popd