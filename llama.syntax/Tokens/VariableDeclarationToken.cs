using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class VariableDeclarationToken : StatementToken
    {
        public readonly ExpressionIdentifierToken Type;
        public readonly ExpressionIdentifierToken Name;

        public VariableDeclarationToken(ExpressionIdentifierToken type, ExpressionIdentifierToken name)
        {
            Type = type;
            Name = name;
        }

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true)
        {
            Type.WriteSourceRecursive(textOutput, codeOnly);
            Name.WriteSourceRecursive(textOutput, codeOnly);
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            Type.WalkRecursive(walker, codeChildrenOnly);
            Name.WalkRecursive(walker, codeChildrenOnly);
            base.WalkRecursive(walker, codeChildrenOnly);
        }

        public static bool TryParseWithOptionalAssignment(ISourceReader reader, out VariableDeclarationToken declaration)
        {
            var start = reader.Position;
            if (!ExpressionIdentifierToken.TryParse(reader, out var type) || !ExpressionIdentifierToken.TryParse(reader, out var name))
            {
                reader.Position = start;
                declaration = null;
                return false;
            }

            if (EqualsToken.MayStartWith(reader.Peek()))
            {
                if (!EqualsToken.TryParse(reader, out var equals) || !ExpressionToken.TryParse(reader, out var initial))
                {
                    reader.Position = start;
                    declaration = null;
                    return false;
                }

                declaration = new VariableDeclarationWithAssignmentToken(type, name, equals, initial);
            }
            else
                declaration = new VariableDeclarationToken(type, name);
            return true;
        }

        public static bool MayStartWith(char character) => ExpressionIdentifierToken.MayStartWith(character);
    }
}
