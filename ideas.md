# The llama programming language and framework

Key features:
* **Polymorphic code generation**: Generates functionally equivalent but different machine code depending on a seed
* **Lightweight and self-contained**: Supports being embedded into an unknown process dynamically, without needing to create a module or load any dependencies
* **Self-modifying features**: Runtime transformation/deletion/creation of a programs own code
* **Remote execution**: The program may run remotely, and only JIT compile & send relevant execution instructions to a client

Key goals:
* Able to generate sophisticated shellcode-like programs with a low footprint
* Evade signature-based detection
* Make dumps useless via self-modifying and remoting



## llama Syntax

### Primitive data types

| Name   | Description      | Bits |
| ------ | ---------------- | ---- |
| sbyte  | Signed Integer   | 8    |
| byte   | Unsigned Integer | 8    |
| short  | Signed Integer   | 16   |
| ushort | Unsigned Integer | 16   |
| int    | Signed Integer   | 32   |
| uint   | Unsigned Integer | 32   |
| long   | Signed Integer   | 64   |
| ulong  | Unsigned Integer | 64   |
|        |                  |      |
|        |                  |      |
|        |                  |      |
|        |                  |      |
|        |                  |      |
|        |                  |      |

### Operators

Operators earlier on the list are evaluated first

| Operator                           | Comment                                                      |
| ---------------------------------- | ------------------------------------------------------------ |
| .                                  | Member access                                                |
| new                                | Instantiation                                                |
|                                    |                                                              |
| ++x --x ! ~                        | Increment Prefix, Decrement Prefix, Logical Negation, Bitwise Complement |
| (T)                                | Type cast                                                    |
| * / %                              | Multiplication, Division, Remainder                          |
| + -                                | Addition, Substration                                        |
| << >>                              | Left Shift, Right Shift                                      |
| < > <= >= is                       | Less Than, Greater Than, Less/Equals Than, Greater/Equals Than, Type Check |
| == !=                              | Equal, Not Equal                                             |
| &                                  | Logical and                                                  |
| ^                                  | Logical xor                                                  |
| \|                                 | Logical or                                                   |
| &&                                 | Conditional and                                              |
| \|\|                               | Conditional or                                               |
| t ? x : y                          | conditional operator                                         |
| = += -= *= /= %= &= \|= ^= <<= >>= | assignment operators                                         |
|                                    |                                                              |
|                                    |                                                              |
|                                    |                                                              |

