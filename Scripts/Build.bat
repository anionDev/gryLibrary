pushd ..
rmdir /s /q GRYLibrary\bin\Release
msbuild GRYLibrary.sln /t:Build /verbosity:normal /property:Configuration=Release
popd