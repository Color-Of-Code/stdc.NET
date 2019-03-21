#!/bin/bash

pushd `dirname "$0"`

# generate a fresh copy of Scanner.cs
dotnet run --project ../gplex/gplex.csproj  /listing /verbose CLexer.lex

popd
