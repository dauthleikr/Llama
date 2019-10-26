# Llama

![](https://github.com/dauthleikr/llama/workflows/BuildAndTest/badge.svg)

A small compiled programming language written from scratch **(0 libraries used)**

- **Where does it run?** x64 Windows only
- **Where is the standard library?** No.
- **Demo Program?** There is a SampleProgram.llama in Llama.Compiler.Cli. Feel free to run "Llama.Compiler.Cli.exe SampleProgram.llama" and execute the resulting SampleProgram.exe

* **Why?** To really understand how everything works from the ground up; No "... yeah uhh let me use a library for [whatever]".
* **Is it usable?** Not really, it's a prototype. The type system is garbage, the optimization is garbage, the language is garbage, the code generator is garbage. I was just exploring.
* **Code generator/assembler source?** No. It's just lots of generated methods from x64 docs. Feel free to give _spit_ on NuGet a closer look. I use it for other stuff too, so it's not in here. Quite garbage though.
* **Unit tests?** Llama.PE has some, it's the only component that I may reuse in the future (after some refactoring). 
* **Whats missing?** 
  * Some easy stuff like: Operators like +=, static variables, multiplication impl for 8-bit types
  * PE work: Exports, Debugging, Resources
  * Proper error reporting
  * Compiler.Cli does not accept any args (console/window app, ...), I just didn't want to create command line parsing code (0 libraries used ...)



### Example Program

The syntax highlighting is a bit off obviously ...

```c
import("kernel32.dll") long GetStdHandle(int handleType)
import("kernel32.dll") bool WriteConsoleA(long handle, byte* str, int count, int* numWritten, long null)

int Main()
{
	long stdOutHandle = GetStdHandle(-11);

	for (int i = 1; true; i = i + 1)
	{
		WriteConsole(stdOutHandle, IntToCstr(i));
		WriteConsole(stdOutHandle, ": ");
		WriteConsole(stdOutHandle, IntToCstr(FibRecursive(i)));
		WriteConsole(stdOutHandle, "\n");
	}
}

int WriteConsole(long handle, cstr str)
{
	if (handle <= 0)
		return 0;
	int numWritten;
	WriteConsoleA(handle, <byte*>str, StrLen(str), &numWritten, 0);
	return numWritten;
}

int StrLen(cstr str)
{
	for (int i = 0; true; i = i + 1)
		if (str[i] == 0)
			return i;
}

long GetStdOutHandle()
{
	return GetStdHandle(-11);
}

long FibRecursive(int index)
{
	if(index < 3)
		return 1;
	return FibRecursive(index - 1) + FibRecursive(index - 2);
}

cstr IntToCstr(long value)
{
	if (value == 0)
	{
		byte[] zeroChars = new byte(2);
		zeroChars[0] = <byte>48;
		return <cstr><byte*>zeroChars;
	}

	int digits = 0;
	if (value < 0)
		digits = 1;

	long valueCopy = value;
	while(valueCopy != 0)
	{
		valueCopy = valueCopy / 10;
		digits = digits + 1;
	}

	byte[] digitChars = new byte(digits + 1);
	if (value < 0)
		digitChars[0] = <byte>45;

	value = Abs(value);
	for (int i = 0; value != 0; i = i + 1)
	{
		digitChars[digits - (i + 1)] = <byte>(48 + value % 10);
		value = value / 10;
	}
	return <cstr><byte*>digitChars;
}

long Abs(long value)
{
	if (value < 0)
		return -value;
	return value;
}
```

