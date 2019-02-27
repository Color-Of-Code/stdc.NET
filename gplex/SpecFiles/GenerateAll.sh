#!/bin/bash

# generate a fresh copy of Parser.cs
dotnet run --project ../../gppg/gppg.csproj /gplex /nolines gplex.y
mv Parser.cs ../GPLEX

# generate a fresh copy of Scanner.cs
dotnet run --project ../gplex.csproj gplex.lex
mv Scanner.cs ../GPLEX
mv GplexBuffers.cs ../GPLEX
