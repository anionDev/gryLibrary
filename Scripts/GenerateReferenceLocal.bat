call Build.bat
if %errorlevel% neq 0 exit /b %errorlevel%
pushd ..\GRYLibrary\ReferenceGeneration
docfx docfx.json --disableGitFeatures
popd