#!/bin/bash

pushd `dirname "$0"`

# generate a fresh copy of Parser.cs
dotnet run --project ../../gppg/gppg.csproj /report /listing /verbose /gplex /nolines gplex.y
mv Parser.cs ../Gplex

# generate a fresh copy of Scanner.cs
dotnet run --project ../gplex.csproj /listing /verbose gplex.lex
mv Scanner.cs ../Gplex

popd
