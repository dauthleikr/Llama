using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    using System.Linq;

    public class AccessModifierToken : TokenBase
    {
        public static readonly IReadOnlyDictionary<string, AccessModifier> ModifierToEnum = new Dictionary<string, AccessModifier>
        {
            {"private", AccessModifier.Private},
            {"protected", AccessModifier.Protected},
            {"public", AccessModifier.Public},
        };

        public static readonly IReadOnlyDictionary<AccessModifier, string> EnumToModifier = ModifierToEnum.ToDictionary(item => item.Value, item => item.Key);
        private static readonly HashSet<string> Modifiers = ModifierToEnum.Keys.ToHashSet();
        private static readonly HashSet<char> ValidFirstModifierChars = ModifierToEnum.Select(item => item.Key.First()).ToHashSet();
        private static readonly int MaxModifierLength = Modifiers.Max(k => k.Length);

        public string ModifierText => EnumToModifier[Modifier];

        public readonly AccessModifier Modifier;

        public AccessModifierToken(AccessModifier modifier) => Modifier = modifier;

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true) => WriteSource(textOutput, ModifierText, !codeOnly);

        public static bool TryParse(ISourceReader reader, out AccessModifierToken result)
        {
            var accessModifier = reader.TryReadLongest(Modifiers, MaxModifierLength);
            if (string.IsNullOrEmpty(accessModifier))
            {
                result = null;
                return false;
            }
            result = new AccessModifierToken(ModifierToEnum[accessModifier]) { PostNonCodeToken = NonCodeToken.ParseOrNull(reader) };
            return true;
        }

        public static bool MayStartWith(char character) => ValidFirstModifierChars.Contains(character);
    }
}
