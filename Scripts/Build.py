import argparse
import subprocess
import os
import sys

parser = argparse.ArgumentParser(description='Compiles GRYLibrary.')
parser.add_argument('configuration')
args = parser.parse_args()
scripts_folder=os.path.abspath("..\\Submodules\\scriptCollection\\Scripts\\Build\\BuildDotNetProgram")
repository_basefolder=os.path.abspath("..")

grylibrary_folder=repository_basefolder+"\\GRYLibrary"
grylibrary_folder_csproj_file=grylibrary_folder+"\\GRYLibrary.csproj"

exit_code=subprocess.call("python " + scripts_folder + "\\BuildProject.py --csproj_filename "+grylibrary_folder_csproj_file+" --folder_of_csproj "+grylibrary_folder +" --configuration "+args.configuration)
if exit_code!=0:
    sys.exit(exit_code)

if args.configuration=="Release":
    grylibrary_tests_folder=repository_basefolder+"\\GRYLibraryTest" 
    grylibrary_tests_csproj_file=grylibrary_tests_folder+"\\GRYLibraryTest.csproj"
    grylibrary_tests_dll_file=grylibrary_tests_folder+"\\bin\\"+args.configuration+"\\GRYLibraryTest.dll"
    exit_code=subprocess.call("python " + scripts_folder + "\\BuildTestprojectAndExecuteTests.py --csproj_filename "+grylibrary_tests_csproj_file+" --folder_of_csproj "+grylibrary_tests_folder +" --configuration "+args.configuration+" --test_dll_file "+grylibrary_tests_dll_file)
    if exit_code!=0:
        sys.exit(exit_code)

sys.exit(0)