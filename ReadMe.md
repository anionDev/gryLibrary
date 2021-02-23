# GRYLibrary

[![CodeFactor](https://www.codefactor.io/repository/github/aniondev/grylibrary/badge/master)](https://www.codefactor.io/repository/github/aniondev/grylibrary/overview/master)
![Generic badge](https://img.shields.io/badge/coverage-1%25-green)

The GRYLibrary is a collection with some useful .NET classes and functions which are very easy (re)usable.

The GRYLibrary follows the declarative-programming-paradigm:

You should say what you want to do, and not how to do it. This paradigm results in code which is easy to understand and can be written very quickly without loosing the overview of your code.

## Getting Started

### Installation

[![NuGet](https://img.shields.io/nuget/v/GRYLibrary.svg?color=green)](https://www.nuget.org/packages/GRYLibrary) ![Nuget](https://img.shields.io/nuget/dt/GRYLibrary.svg)

Install the GRYLibrary as NuGet-package using the Package Manager Console:

```bash
Install-Package GRYLibrary
```

### Reference

The GRYLibrary-reference can be found [here](https://aniondev.github.io/GRYLibraryReference).

## Hints

### Platform

The latest NuGet-package is designed for .NET-standard 2.1.

### Signing

The GRYLibrary-NuGet-packages are always signed. You can check the public key token by using [sn](https://docs.microsoft.com/en/dotnet/framework/tools/sn-exe-strong-name-tool): `sn -T GRYLibrary.dll`

The public key token of all official GRYLibrary-releases is `fa37b6e9de549c68`. For security-reasons you should only use GRYLibrary.dll-files which you have compiled by yourself from the source code in this repository or which have this public key token.

### Contribute

Feel free to contribute to this product by creating [issues](https://github.com/anionDev/GRYLibrary/issues) for feature-requests, bug-reports, etc.
Since the GRYLibrary is not an open-source-project in the [conventional sense of free software](https://www.gnu.org/philosophy/free-sw.en.html) contributing by creating a pullrequest is a little bit tricky (concerning license-issues). If you want to contribute to this project then please contact the owner of the GRYLibrary.

## License

There are the following licenses for the GRYLibrary available

- The GRYLibrary is generally and commonly licensed under the terms of GRYL. The concrete license-text can be found [here](https://raw.githubusercontent.com/anionDev/GRYLibrary/master/License.txt). This license-text does obviously not apply to the other following licenses.
- There are some special licenses for certain scopes:
  - [epew](https://github.com/anionDev/ExternalProgramExecutionWrapper) is allowed to use the [Nuget-release](https://www.nuget.org/packages/GRYLibrary) of the GRYLibrary under the Terms of the MIT-license for executing programs as main-purpose of epew and for nothing else.
  - [ReliablePlayer](https://github.com/anionDev/ReliablePlayer) is allowed to use the [Nuget-release](https://www.nuget.org/packages/GRYLibrary) of the GRYLibrary under the Terms of the MIT-license to help implement features provided in the official repository and release of ReliablePlayer and for nothing else.
- If you need another license-type or you want to use the GRYLibrary in your company then please contact the owner of the GRYLibrary.
