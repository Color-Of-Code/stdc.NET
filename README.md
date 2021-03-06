﻿# stdc.NET

## CI-Status

[![Action Status](https://github.com/Color-Of-Code/stdc.NET/workflows/.NET%20Core/badge.svg)](https://github.com/Color-Of-Code/stdc.NET/actions)

## Goals

Provide a set of C# static methods mimicking standard C library's behavior to be able to
straightforwardly make a first port of a C application to .NET core.

This initial port can then iteratively be converted into a more idiomatic .NET core
application, keeping it functional all the way through.

Ideally you would also have a strong set of unit tests that would support the refactoring
process.

As a starter, have some look at these [simple examples](./Examples.md)

## Hints

Excellent hints on parsing in C#:

* https://tomassetti.me/parsing-in-csharp/

## Other GitHub projects

### Parser generators

* https://github.com/kellyw42/GardensPointCompilerTools
* https://github.com/KommuSoft/Gardens-Point-Parser-Generator
* https://github.com/KommuSoft/Gardens-Point-LEX
* https://github.com/deAtog/gplex
* https://github.com/k-john-gough/gpcp (pascal)

### Monadic parser combinator libraries

* [Sprache](https://github.com/sprache/sprache)
* [Superpower](https://github.com/datalust/superpower)
* [Pidgin](https://github.com/benjamin-hodgson/Pidgin)
* [Parsley](https://github.com/plioi/parsley)
* [FParsec](https://github.com/stephan-tolksdorf/fparsec)
* [Irony](https://github.com/IronyProject/Irony)
