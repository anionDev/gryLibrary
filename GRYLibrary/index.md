# General

The GRYLibrary contains some useful functions and extensions written in C#. For example:
* Tablegenerator: Generates ASCII-tables
* GRYLog: Represents a (more or less) simple log-functionality
* SimpleGenericXMLSerializer: To serialize/deserialize objects as XML in a very easy way.

All labels (for classes, properties, etc.) have meaningful names.

# Reference

The concrete reference all types can be found [here](https://aniondev.github.io/gryLibraryReference/Reference/api/GRYLibrary.html).

# Additional information

[![NuGet](https://img.shields.io/nuget/v/GRYLibrary.svg?color=green)](https://www.nuget.org/packages/GRYLibrary/)

![Nuget](https://img.shields.io/nuget/dt/GRYLibrary.svg)

![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/anionDev/gryLibrary.svg)

![GitHub repo size](https://img.shields.io/github/repo-size/anionDev/gryLibrary.svg)

![GitHub issues](https://img.shields.io/github/issues-raw/anionDev/gryLibrary.svg)

![Maintenance](https://img.shields.io/maintenance/yes/2020.svg)

[![Coverage Status](https://coveralls.io/repos/github/anionDev/gryLibrary/badge.svg?branch=dev%2Fdevelopment)](https://coveralls.io/github/anionDev/gryLibrary?branch=dev%2Fdevelopment)

# Contribute

You are welcome to contribute by
* fixing bugs
* adding new useful functions
* adding documentation for existing functions
* adding tests for existing functions

and you are also welcome to share your commits under the terms of [LGPL](https://raw.githubusercontent.com/anionDev/gryLibrary/master/ConcreteLicenseTexts/GNU%20Lesser%20General%20Public%20License%20version%203.txt) by creating a pullrequest. If you want to do that then please create a fork of the development-branch (`development`) and then create a pullrequest back to this branch (not to the `master`-branch).

# Links

[Github GRYLibrary-project](https://github.com/anionDev/gryLibrary)

[Github GRYLibraryReference-project](https://github.com/anionDev/gryLibraryReference)

[Nuget GRYLibrary-package](https://www.nuget.org/packages/GRYLibrary)

[Class Reference and Documentation](https://aniondev.github.io/gryLibraryReference/Site/api/GRYLibrary.html)

[Test-coverage](https://aniondev.github.io/gryLibraryReference/TestReports/index.htm)

[License](https://raw.githubusercontent.com/anionDev/gryLibrary/master/License.txt)

# Version-system

For every merge to the master the minor version will be increased and a new nuget-package will be published.

# Changelog

## Version 0.4 (Planned)

### Release-information

[Tag](https://github.com/anionDev/gryLibrary/releases/tag/v0.4)

[Nuget](https://www.nuget.org/packages/GRYLibrary/0.4.0)

### Changes

- [ ] Support [Common Log Format](https://httpd.apache.org/docs/1.3/logs.html#common) in GRYLog
- [ ] Support [Extended Log File Format](https://www.w3.org/TR/WD-logfile.html) in GRYLog
- [ ] Support custom formats in GRYLog
- [ ] Implement HTML-Tables in TableGenerator

## Version 0.3 (In Progress)

### Release-information

[Tag](https://github.com/anionDev/gryLibrary/releases/tag/v0.3)

[Nuget](https://www.nuget.org/packages/GRYLibrary/0.3.0)

### Changes

- [x] Create Semaphore
- [x] Create TableGenerator
- [x] Create TaskQueue
- [x] Create DocFX-Project
- [x] Add more testcases
- [x] Improve information-files (.md-files etc.)
- [x] Add More doc-comments

## Version 0.2

### Release-information

[Tag](https://github.com/anionDev/gryLibrary/releases/tag/v0.2)

[Nuget](https://www.nuget.org/packages/GRYLibrary/0.2.0)

[Commit 87ba33fc](https://github.com/anionDev/gryLibrary/commit/87ba33fc9073126bcbfdb83acb1eda56311fa6a8)

### Changes

- [x] Create Fileselector
- [x] Create SimpleObjectPersistence
- [x] Fixes

## Version 0.1 

### Release-information

[Tag](https://github.com/anionDev/gryLibrary/releases/tag/v0.1)

[Nuget](https://www.nuget.org/packages/GRYLibrary/0.1.0)

[Commit 952bb1ac](https://github.com/anionDev/gryLibrary/commit/952bb1ac347852e978016afa6926f9b6256e7cb0)

### Changes

- [x] Create ColorGradient
- [x] Create ExtendedColor
- [x] Create ExternalProgramExecutor
- [x] Create GRYLog
- [x] Create NonPersistentInputHistoryList
- [x] Create PercentValue
- [x] Create PlaylistHandler
- [x] Create SimpleGenericXMLSerializer
- [x] Create SupervisedThread
- [x] Create Testproject
