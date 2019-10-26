@echo off
setlocal
cd ..\Llama.IDE.VSCode
npx vsce package -o ..\Example\llama-lang.vsix
endlocal