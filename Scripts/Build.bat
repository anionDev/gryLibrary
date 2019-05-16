pushd ..\GRYLibrary
rmdir /s /q bin\Release
nuget restore -SolutionDirectory ..
msbuild GRYLibrary.csproj /t:Build /verbosity:normal /property:Configuration=Release
popd