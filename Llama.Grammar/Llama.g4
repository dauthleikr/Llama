grammar Llama;

// Parser rules
root: (functionImplementation | functionImport)*;

declaration: Type Identifier (Assignment expression)?;

methodCall:
	expression methodCallParameters;

ifControl:
	If OpenParanthesis expression CloseParanthesis statementAny;

whileControl:
	While OpenParanthesis expression CloseParanthesis statementAny;

forControl:
	For OpenParanthesis declaration ';' expression ';' expression CloseParanthesis statementAny;

statementAny: (statementSingle | statementBlock);

statementBlock: OpenBraces statementSingle* CloseBraces;

statementSingle: (
		declaration
		| ifControl
		| whileControl
		| forControl
		| methodCall
	) SemiColon;

expression:
	OpenParanthesis expression CloseParanthesis
	| expression methodCallParameters
	| expression BinaryOperator expression
	| atomicExpression;

atomicExpression:
	String
	| IntegerLiteral
	| FloatLiteral
	| Identifier;

methodCallParameters: OpenParanthesis (
		(expression Comma)*? expression
	)? CloseParanthesis;

functionDeclaration: Type Identifier OpenParanthesis (
		(Type Identifier Comma)*? Type Identifier
	)? CloseParanthesis;

functionImplementation: functionDeclaration statementBlock;
functionImport: Import OpenParanthesis String CloseParanthesis functionDeclaration SemiColon;

// Lexer rules
Assignment: '=';
OpenParanthesis: '(';
CloseParanthesis: ')';
OpenBraces: '{';
CloseBraces: '}';
Comma: ',';
SemiColon: ';';

Import: 'import';
If: 'if';
Else: 'else';
While: 'while';
For: 'for';
Type: PrimitiveType '*'*;
Identifier: [_A-Za-z][_A-Za-z0-9]*;
BinaryOperator: Assignment | '+' | '-';

String: '"' .*? '"';
IntegerLiteral: [0-9]+ [0-9_]*;
FloatLiteral: IntegerLiteral? '.' IntegerLiteral;
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

// Trivia rules
WhitespaceOrControl: [ \r\n\t]+ -> skip;
LineComment: '//' ~[\r\n]* -> skip;
BlockComment: '/*' .*? '*/' -> skip;