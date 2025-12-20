using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace AxiomCompiler
{
    public enum TokenType
    {
        // --- Importance 0-5: Structural / Declarations ---
        [Description("0")] Import,
        [Description("1")] Class,
        [Description("1")] Struct,
        [Description("1")] Function,
        [Description("1")] Type,
        [Description("1")] ClassRef,
        [Description("5")] LeftCurly,    // {
        [Description("5")] RightCurly,   // }

        // --- Importance 10: Control Flow Statements ---
        [Description("10")] While,
        [Description("10")] If,
        [Description("10")] Else,
        [Description("10")] For,
        [Description("10")] Foreach,
        [Description("10")] Switch,
        [Description("10")] Case,
        [Description("10")] Default,

        // --- Importance 20: Assignment (Right-to-Left Associative) ---
        [Description("20")] Set,            // =
        [Description("20")] AddEqual,       // +=
        [Description("20")] SubtractEqual,  // -=
        [Description("20")] MultiplyEqual,  // *=
        [Description("20")] DivideEqual,    // /=

        // --- Importance 30-40: Logical Operators ---
        [Description("30")] Or,
        [Description("35")] Xor,
        [Description("40")] And,

        // --- Importance 50-60: Comparison/Equality ---
        [Description("50")] Equal,          // ==
        [Description("50")] NotEqual,       // !=
        [Description("60")] LessThan,
        [Description("60")] LessThanOrEqual,
        [Description("60")] GreaterThan,
        [Description("60")] GreaterThanOrEqual,

        // --- Importance 70-80: Math ---
        [Description("70")] Add,
        [Description("70")] Subtract,
        [Description("80")] Multiply,
        [Description("80")] Divide,

        // --- Importance 90: Unary and Accessors ---
        [Description("90")] Not,            // !
        [Description("95")] Dot,            // .

        // --- Importance 100: Grouping (Handled by Depth) ---
        [Description("100")] LeftParen,     // (
        [Description("100")] RightParen,    // )
        [Description("100")] LeftBracket,   // [
        [Description("100")] RightBracket,  // ]

        // --- Importance 110: Literals (Leaves) ---
        [Description("110")] Number,
        [Description("110")] Speech,       // String literals
        [Description("110")] Time,         // e.g. 5s
        [Description("110")] Vector,
        [Description("110")] List,
        [Description("110")] Bounds,
        [Description("110")] Unknown,      // Variable names / Identifiers
        [Description("110")] String
    }

    public class Line
    {
        public string Text;
        public int LineNumber;
        public List<Token> Tokens = new List<Token>();
        public Token Main;
    }
    public class Token
    {
        public TokenType Type;
        public string Value;
        public List<Token> Children = new List<Token>();
        public string Info;
        public bool Structured;

        public static implicit operator Token(string Text)
        {
            Token token = new Token();
            token.Value = Text;
            
            // Basic Keyword/Symbol Mapping
            switch (Text.ToLower())
            {
                case "type":     token.Type = TokenType.Type; break;
                case "import":   token.Type = TokenType.Import; break;
                case "class":    token.Type = TokenType.Class; break;
                case "struct":   token.Type = TokenType.Struct; break;
                case "function": token.Type = TokenType.Function; break;
                case "{":        token.Type = TokenType.LeftCurly; break;
                case "}":        token.Type = TokenType.RightCurly; break;
                case "(":        token.Type = TokenType.LeftParen; break;
                case ")":        token.Type = TokenType.RightParen; break;
                case "[":        token.Type = TokenType.LeftBracket; break;
                case "]":        token.Type = TokenType.RightBracket; break;
                case "if":       token.Type = TokenType.If; break;
                case "else":     token.Type = TokenType.Else; break;
                case "while":    token.Type = TokenType.While; break;
                case "for":      token.Type = TokenType.For; break;
                case "foreach":  token.Type = TokenType.Foreach; break;
                case "switch":   token.Type = TokenType.Switch; break;
                case "case":     token.Type = TokenType.Case; break;
                case "default":  token.Type = TokenType.Default; break;
                case "=":        token.Type = TokenType.Set; break;
                case "+=":       token.Type = TokenType.AddEqual; break;
                case "-=":       token.Type = TokenType.SubtractEqual; break;
                case "*=":       token.Type = TokenType.MultiplyEqual; break;
                case "/=":       token.Type = TokenType.DivideEqual; break;
                case "==":       token.Type = TokenType.Equal; break;
                case "!=":       token.Type = TokenType.NotEqual; break;
                case "<":        token.Type = TokenType.LessThan; break;
                case ">":        token.Type = TokenType.GreaterThan; break;
                case "<=":       token.Type = TokenType.LessThanOrEqual; break;
                case ">=":       token.Type = TokenType.GreaterThanOrEqual; break;
                case "+":        token.Type = TokenType.Add; break;
                case "-":        token.Type = TokenType.Subtract; break;
                case "*":        token.Type = TokenType.Multiply; break;
                case "/":        token.Type = TokenType.Divide; break;
                case "&":        token.Type = TokenType.And; break;
                case "|":        token.Type = TokenType.Or; break;
                case "xor":      token.Type = TokenType.Xor; break;
                case "!":        token.Type = TokenType.Not; break;
                case ".":        token.Type = TokenType.Dot; break;
                case "\"":       token.Type = TokenType.Speech; break;
                default:         token.Type = TokenType.Unknown; break;
            }

            // Special Pattern Matching
            if (Text.StartsWith("#"))
            {
                token.Type = TokenType.Import;
            }
            else if (token.Type == TokenType.Unknown)
            {
                foreach (var Class in Values.Classes)
                {
                    if (Class.Name == Text)
                    {
                        token.Type = TokenType.ClassRef;
                    }
                }
                if (double.TryParse(Text, out _))
                {
                    token.Type = TokenType.Number;
                }
                else if (Text.EndsWith("s") && double.TryParse(Text.Substring(0, Text.Length - 1), out _))
                {
                    token.Type = TokenType.Time;
                }
            }
            return token;
        }

        public override string ToString() => $"{Type}({Value})";
    }

    public static class Tokens
    {
        public static Token Parse(string Line)
        {
            if (string.IsNullOrWhiteSpace(Line)) return null;

            // Split by space - logic assumes tokens are space-separated
            List<Token> tokens = Line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => (Token)s)
                                     .ToList();

            return Structure(tokens);
        }

        public static Token? Structure(List<Token> tokens)
        {
            if (tokens == null || tokens.Count == 0) return null;

            // 1. Strip redundant outer parentheses: ( a + b ) -> a + b
            while (IsWrappedInParens(tokens))
            {
                tokens = tokens.GetRange(1, tokens.Count - 2);
            }

            if (tokens.Count == 1) return tokens[0];

            int rootIndex = -1;
            int lowestEffectiveImportance = int.MaxValue;
            int currentDepth = 0;

            // 2. Find the "Weakest" operator to be the Root
            for (int i = 0; i < tokens.Count; i++)
            {
                TokenType type = tokens[i].Type;

                // Track parenthesis depth to protect internal expressions
                if (type == TokenType.LeftParen || type == TokenType.LeftBracket) { currentDepth++; continue; }
                if (type == TokenType.RightParen || type == TokenType.RightBracket) { currentDepth--; continue; }

                int baseImportance = EnumExtensions.GetImportance(type);
                
                // Literals/Unknowns (110) are never roots if an operator exists
                if (baseImportance >= 100) continue;

                // Operators inside parentheses get a massive importance boost
                int effectiveImportance = baseImportance + (currentDepth * 1000);

                // 3. Handle Associativity
                // Assignment (=) is Right-to-Left: Pick the FIRST one encountered as root (<)
                // Math (+, *) is Left-to-Right: Pick the LAST one encountered as root (<=)
                bool isRightAssociative = (type == TokenType.Set || type == TokenType.AddEqual || 
                                           type == TokenType.SubtractEqual || type == TokenType.MultiplyEqual || 
                                           type == TokenType.DivideEqual);

                if (isRightAssociative)
                {
                    if (effectiveImportance < lowestEffectiveImportance)
                    {
                        lowestEffectiveImportance = effectiveImportance;
                        rootIndex = i;
                    }
                }
                else
                {
                    if (effectiveImportance <= lowestEffectiveImportance)
                    {
                        lowestEffectiveImportance = effectiveImportance;
                        rootIndex = i;
                    }
                }
            }

            // If no operator found outside literals, just return the first leaf
            if (rootIndex == -1) return tokens[0];

            Token root = tokens[rootIndex];
            root.Children = new List<Token>();

            // 4. Split and recurse
            List<Token> leftSide = tokens.GetRange(0, rootIndex);
            List<Token> rightSide = tokens.GetRange(rootIndex + 1, tokens.Count - rootIndex - 1);

            Token? leftChild = Structure(leftSide);
            Token? rightChild = Structure(rightSide);

            if (leftChild != null) root.Children.Add(leftChild);
            if (rightChild != null) root.Children.Add(rightChild);

            return root;
        }

        private static bool IsWrappedInParens(List<Token> tokens)
        {
            if (tokens.Count < 2) return false;
            if (tokens[0].Type != TokenType.LeftParen || tokens[^1].Type != TokenType.RightParen) return false;

            int depth = 0;
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i].Type == TokenType.LeftParen) depth++;
                if (tokens[i].Type == TokenType.RightParen) depth--;
                if (depth == 0) return false; // Parens closed early like: (a+b) + (c)
            }
            return true;
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null) return null;

            FieldInfo field = type.GetField(name);
            DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();

            return attr != null ? attr.Description : name;
        }

        public static int GetImportance(this Enum value)
        {
            string description = value.GetDescription();
            return int.TryParse(description, out int result) ? result : 0;
        }
    }
}