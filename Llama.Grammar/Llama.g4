grammar Llama;

// Parser rules
root: (anyToken)+;

anyToken:
	Control
	| Identifier
	| Literal
	| Type
	| BinaryOperator
	| OpenBraces
	| CloseBraces
    | SemiColon
    | Comma
	| OpenParanthesis
	| CloseParanthesis;

// Lexer rules
Control: 'if' | 'else' | 'while' | 'for';
Type: PrimitiveType '*'*;
Literal: String | IntegerLiteral | FloatLiteral;
Identifier: [_A-Za-z][_A-Za-z0-9]*;
BinaryOperator: '=' | '+' | '-';

OpenParanthesis: '(';
CloseParanthesis: ')';
OpenBraces: '{';
CloseBraces: '}';
Comma: ',';
SemiColon: ';';

fragment String: '"' .*? '"';
fragment IntegerLiteral: [0-9]+ [0-9_]*;
fragment FloatLiteral: IntegerLiteral? '.' IntegerLiteral;
fragment PrimitiveType:
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