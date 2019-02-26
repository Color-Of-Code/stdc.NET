#!/bin/bash

# generate a fresh copy of Parser.cs
cd gppg
dotnet run /gplex /nolines ../gplex/SpecFiles/gplex.y
mv Parser.cs ../gplex/GPLEX

# generate a fresh copy of Scanner.cs
cd gplex
dotnet run SpecFiles/gplex.lex
mv Scanner.cs ../gplex/GPLEX
mv GplexBuffers.cs ../gplex/GPLEX
