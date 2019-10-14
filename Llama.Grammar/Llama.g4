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

statementAny: (statementSingle | statementBlock);

statementBlock: OpenBraces statementSingle* CloseBraces;

statementSingle: (declaration | expression) SemiColon
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

binaryOperator: Plus | Minus | Equals | NotEquals | OpenAngularBracket | CloseAngularBracket | Assignment | GreaterEquals | SmallerEquals;

unaryOperator: Minus | AddressOf | Not;

typeCast: OpenAngularBracket type CloseAngularBracket;

atomicExpression: String | IntegerLiteral | FloatLiteral | Identifier;

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
Equals: '==';
NotEquals: '!=';
GreaterEquals: '>=';
SmallerEquals: '<=';
Not: '!';
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