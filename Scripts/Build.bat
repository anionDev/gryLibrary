pushd ..
msbuild GRYLibrary.sln /t:Build /verbosity:detailed /p:Configuration=Release
popd
pause