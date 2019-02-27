
# generate a fresh copy of parser.cs
gppg /gplex /nolines gppg.y
move parser.cs ..

# generate a fresh copy of Scanner.cs
gplex gppg.lex
move Scanner.cs ..

# generate a fresh copy of ScanAction.cs
gplex ScanAction.lex
move ScanAction.cs ..

move GplexBuffers.cs ..


