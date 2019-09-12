grammar Llama;

// Parser rules
root: (functionImplementation | functionImport)*;

declaration: type Identifier (Assignment expression)?;

assignment: expression Assignment expression;

methodCall: expression methodCallParameters;

ifControl:
	If OpenParanthesis expression CloseParanthesis statementAny;

whileControl:
	While OpenParanthesis expression CloseParanthesis statementAny;

forControl:
	For OpenParanthesis declaration ';' expression ';' expression CloseParanthesis statementAny;

statementAny: (statementSingle | statementBlock);

statementBlock: OpenBraces statementSingle* CloseBraces;

statementSingle: (declaration | assignment | methodCall) SemiColon
	| ifControl
	| whileControl
	| forControl;

expression:
	OpenParanthesis expression CloseParanthesis
	| expression methodCallParameters
	| expression binaryOperator expression
	| expression OpenSquareBracket expression CloseSquareBracket
	| typeCast expression
	| unaryOperator expression
	| New PrimitiveType OpenSquareBracket expression CloseSquareBracket
	| literal;

binaryOperator: Plus | Minus;

unaryOperator: Minus | AddressOf;

typeCast: OpenAngularBracket type CloseAngularBracket;

literal: String | IntegerLiteral | FloatLiteral | Identifier;

type:
	PrimitiveType (
		Pointer
		| OpenSquareBracket CloseSquareBracket
	)*; 

methodCallParameters:
	OpenParanthesis ((expression Comma)*? expression)? CloseParanthesis;

functionDeclaration:
	type Identifier OpenParanthesis (
		(type Identifier Comma)*? type Identifier
	)? CloseParanthesis;

functionImplementation: functionDeclaration statementBlock;

functionImport:
	Import OpenParanthesis String CloseParanthesis functionDeclaration SemiColon;

// Lexer rules
Assignment: '=';
OpenParanthesis: '(';
CloseParanthesis: ')';
OpenBraces: '{';
CloseBraces: '}';
OpenSquareBracket: '[';
CloseSquareBracket: ']';
OpenAngularBracket: '<';
CloseAngularBracket: '>';
Comma: ',';
SemiColon: ';';
Pointer: '*';
Plus: '+';
Minus: '-';
AddressOf: '&';

New: 'new';
Delete: 'delete';
Import: 'import';
If: 'if';
Else: 'else';
While: 'while';
For: 'for';
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

// Trivia rules
WhitespaceOrControl: [ \r\n\t]+ -> skip;
LineComment: '//' ~[\r\n]* -> skip;
BlockComment: '/*' .*? '*/' -> skip;