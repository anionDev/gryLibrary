pushd ..\GRYLibrary
rmdir /s /q bin\Release
nuget restore -SolutionDirectory ..
msbuild GRYLibrary.csproj /t:Build /verbosity:normal /property:Configuration=Release
popd

pushd ..\GRYLibraryTest
rmdir /s /q bin\Release
nuget restore -SolutionDirectory ..
msbuild GRYLibraryTest.csproj /t:Build /verbosity:normal /property:Configuration=Release
popd

push ..\GRYLibraryTest\bin\Release
vstest.console GRYLibraryTest.dll
set vstestResult = %errorlevel%
popd
exit /b %vstestResult%