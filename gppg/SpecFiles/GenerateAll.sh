#!/bin/bash

pushd `dirname "$0"`

# generate a fresh copy of parser.cs
dotnet run --project ../gppg.csproj /report /verbose /listing /gplex /nolines gppg.y
mv Parser.cs ../ParserGenerator

# generate a fresh copy of Scanner.cs
dotnet run --project ../../gplex/gplex.csproj /verbose /listing gppg.lex
mv Scanner.cs ../ParserGenerator

# generate a fresh copy of ScanAction.cs
dotnet run --project ../../gplex/gplex.csproj /verbose /listing ScanAction.lex
mv ScanAction.cs ../ParserGenerator
mv GplexBuffers.cs ../ParserGenerator

popd
