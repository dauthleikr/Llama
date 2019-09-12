grammar Llama;

root: (anyToken )+;
anyToken: Keyword | Identifier;

// Lexer rules
Keyword: 'if' | 'else' | 'while' | 'for';

Identifier: [_A-Za-z][_A-Za-z0-9]*;

// Trivia rules
WhitespaceOrControl: [ \r\n\t]+
    -> skip;
LineComment: '//'~[\r\n]* 
    -> skip;
BlockComment: '/*' .*? '*/' 
    -> skip;