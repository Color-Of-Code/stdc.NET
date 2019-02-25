#!/bin/bash

# TODO: find a replacement for .NET core
# generate resource resX file and copy to destination
#csc GenerateResource.cs
#GenerateResource.exe
#move Content.resx ..\IncludeResources
#del GenerateResource.exe

# generate a fresh copy of Parser.cs
cd gppg
dotnet run /gplex /nolines ../gplex/SpecFiles/gplex.y
mv Parser.cs ../gplex/GPLEX

# generate a fresh copy of Scanner.cs
gplex gplex.lex
mv Scanner.cs ../gplex/GPLEX

if not exist GplexBuffers.cs goto finish
mv GplexBuffers.cs ../gplex/GPLEX


