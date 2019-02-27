#!/bin/bash

# generate a fresh copy of parser.cs
dotnet run --project ../gppg.csproj /gplex /nolines gppg.y
mv Parser.cs ../ParserGenerator

# generate a fresh copy of Scanner.cs
dotnet run --project ../../gplex/gplex.csproj gppg.lex
mv Scanner.cs ../ParserGenerator

# generate a fresh copy of ScanAction.cs
dotnet run --project ../../gplex/gplex.csproj ScanAction.lex
mv ScanAction.cs ../ParserGenerator

mv GplexBuffers.cs ../ParserGenerator
