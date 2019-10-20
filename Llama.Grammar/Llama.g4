grammar Llama;

// Parser rules
root: (functionImplementation | functionImport)*;

declaration: type Identifier (Assignment expression)?;

ifControl:
	If OpenParanthesis expression CloseParanthesis statementAny (Else statementAny)?;

whileControl:
	While OpenParanthesis expression CloseParanthesis statementAny;

forControl:
	For OpenParanthesis declaration SemiColon expression SemiColon expression CloseParanthesis statementAny;

returnStatement:
	Return expression?;

statementAny: (statementSingle | statementBlock);

statementBlock: OpenBraces statementSingle* CloseBraces;

statementSingle: (declaration | returnStatement | expression) SemiColon
	| ifControl
	| whileControl
	| forControl;

expression:
	OpenParanthesis expression CloseParanthesis
	| expression methodCallParameters
	| expression OpenSquareBracket expression CloseSquareBracket
	| expression binaryOperator expression
	| typeCast expression
	| unaryOperator expression
	| New type OpenParanthesis expression CloseParanthesis
	| atomicExpression;

binaryOperator: Plus | Minus | Divide | Pointer | Mod | Equals | NotEquals | OpenAngularBracket | CloseAngularBracket | Assignment | GreaterEquals | SmallerEquals;

unaryOperator: Minus | AddressOf | Not | Pointer;

typeCast: OpenAngularBracket type CloseAngularBracket;

atomicExpression: String | IntegerLiteral | FloatLiteral | True | False | Identifier;

type:
	type Pointer
	| type OpenSquareBracket CloseSquareBracket
	| PrimitiveType; 

methodCallParameters:
	OpenParanthesis ((expression Comma)*? expression)? CloseParanthesis;

functionDeclaration:
	type Identifier OpenParanthesis (
		(type Identifier Comma)*? type Identifier
	)? CloseParanthesis;

functionImplementation: functionDeclaration statementBlock;

functionImport:
	Import OpenParanthesis String CloseParanthesis functionDeclaration;

// Trivia rules
WhitespaceOrControl: [ \r\n\t]+ -> skip;
LineComment: '//' ~[\r\n]* -> skip;
BlockComment: '/*' .*? '*/' -> skip;

// Lexer rules
Equals: '==';
NotEquals: '!=';
GreaterEquals: '>=';
SmallerEquals: '<=';
Assignment: '=';
OpenAngularBracket: '<';
CloseAngularBracket: '>';
Not: '!';
AddressOf: '&';
OpenParanthesis: '(';
CloseParanthesis: ')';
OpenBraces: '{';
CloseBraces: '}';
OpenSquareBracket: '[';
CloseSquareBracket: ']';
Comma: ',';
SemiColon: ';';
Pointer: '*';
Plus: '+';
Minus: '-';
Divide: '/';
Mod: '%';
True: 'true';
False: 'false';
New: 'new';
Delete: 'delete';
Import: 'import';
If: 'if';
Else: 'else';
While: 'while';
For: 'for';
Return: 'return';
PrimitiveType:
	'void'
	| 'int'
	| 'long'
	| 'short'
	| 'byte'
	| 'sbyte'
	| 'cstr'
	| 'float'
	| 'double';



String: '"' .*? '"';
IntegerLiteral: [0-9]+ [0-9_]*;
FloatLiteral: IntegerLiteral? '.' IntegerLiteral;
Identifier: [_A-Za-z][_A-Za-z0-9]*;

