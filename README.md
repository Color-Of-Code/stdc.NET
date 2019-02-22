# stdc.NET

## CI-Status

[![Travis build status](https://img.shields.io/travis/Color-Of-Code/stdc.NET/master.svg?label=master&style=flat-square)](https://travis-ci.org/Color-Of-Code/stdc.NET)

## Goals

Provide a set of C# static methods mimicking standard C library's behavior to be able to
straightforwardly make a first port of a C application to .NET core.

This initial port can then iteratively be converted into a more idiomatic .NET core
application, keeping it functional all the way through.

Ideally you would also have a strong set of unit tests that would support the refactoring
process.

As a starter, have some look at these [simple examples](./Examples.md)

## Notes

Could not find this anymore

Project("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}") = "ctest", "ctest\ctest.vcxproj", "{4492194E-7BC0-43F2-964E-F621F4D62597}"
EndProject

## Hints

Excellent hints on parsing in C#:

* https://tomassetti.me/parsing-in-csharp/

## GPlex

### Other GitHub projects

* https://github.com/kellyw42/GardensPointCompilerTools
* https://github.com/KommuSoft/Gardens-Point-Parser-Generator
* https://github.com/KommuSoft/Gardens-Point-LEX
* https://github.com/deAtog/gplex
* https://github.com/k-john-gough/gpcp (pascal)
