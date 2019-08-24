# Llama.PE + Tests

My basic idea on how to work on this in a structured and 'safe' way

## Start

1. Recreate (all) PE structs in C# ('exact' structs so that I do not have to write 'reading & writing' code, because I can just take their binary representation using MemoryMarshal and friends / just map them to memory)

   ('exact' structs means members have correct offsets and lengths (`[StructLayout(LayoutKind.Sequential)]`, ...))

2. Write unit tests that read from a working, minimal .exe, using my structs.

   I can use these tests to check if I have correctly understood how the headers work. I can also inspect the values of the working .exe to discover plausible values and deprecated fields.

3. Create some .exe writing code that turns a minimal set of parameters (code, import library list, ...) into the PE structs and writes them in the correct order (thus creating a working .exe)

   I already know that my structs are correct, thus minimizing possible mistakes on my part.

   I can use my tests to verify my .exe. This will help me analyze where I messed up

## 24.08.2019

* Creating 'exact' structs is a pain in the ass.  Some, like the [Hint/Name Table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#hintname-table), are impossible to do because I'd need to marshal the name with something like ByValTStr, without the fixed size. I'd need to write a custom Marshaller, which also does not fit into my existing 'architecture' of using MemoryMarshal. I've written manual converting code (IPEReader, IPE32PlusContext, ...) where needed, which is something I had wanted to avoid.
* I should create some ez-pz builder pattern for creating .exe files. `.PlzImport("kernel32", "WriteFile")` , `AddCode(code)` yum. Other parts of this project should not have to deal with all this complexity.
* It probably makes sense to create some intermediate model, instead of going from user calls/parameters directly to PE structs. This would contain minimal clean information (without offsets and so on) in a C# way.
* I can't actually just create the structs and write them in the correct order - many structs have RVAs or offsets as members. Some ideas on how to fix this:
  * Write individual structs but keep track what offsets/RVAs/... need to be fixed later. Maybe match them automatically by giving them names?
  * One mega-function that writes everything. It can write all references correctly because it knows everything. Seems like a dumb idea, but it may be the simplest one.
  * There should be no chicken/egg problem. Create what you can from an intermediate model, and pass the created binary blob and its meta-information to the next module. The last module in the chain creates the full .exe.