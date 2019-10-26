# Llama

![](https://github.com/dauthleikr/llama/workflows/BuildAndTest/badge.svg)

A small compiled programming language written from scratch **(0 libraries used)**

- **Where does it run?** x64 Windows only
- **Where is the standard library?** No.
- **Demo Program?** There is a SampleProgram.llama in Llama.Compiler.Cli. Feel free to run "Llama.Compiler.Cli.exe SampleProgram.llama" and execute the resulting SampleProgram.exe

* **Why?** To really understand how everything works from the ground up; No "... yeah uhh let me use a library for [whatever]".
* **Is it usable?** Not really, it's a prototype. The type system is garbage, the optimization is garbage, the language is garbage, the code generator is garbage. I was just exploring.
* **Code generator/assembler source?** No. It's just lots of generated methods from x64 docs. Feel free to give _spit_ on NuGet a closer look. I use it for other stuff too, so it's not in here. Quite trash though.
* **Unit tests?** Llama.PE has some, it's the only component that I may reuse in the future (after some refactoring). 

