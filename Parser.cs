using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using LLVMSharp.Interop;

namespace AxiomCompiler;

public enum TokenType
{
    // --- Importance 0-5: Structural / Declarations ---
    [Description("0")] Import,
    [Description("1")] Class,
    [Description("1")] Struct,
    [Description("1")] Function,
    [Description("1")] Type,
    [Description("1")] ClassRef,
    [Description("5")] LeftCurly, // {
    [Description("5")] RightCurly, // }

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
    [Description("20")] Set, // =
    [Description("20")] AddEqual, // +=
    [Description("20")] SubtractEqual, // -=
    [Description("20")] MultiplyEqual, // *=
    [Description("20")] DivideEqual, // /=

    // --- Importance 30-40: Logical Operators ---
    [Description("30")] Or,
    [Description("35")] Xor,
    [Description("40")] And,

    // --- Importance 50-60: Comparison/Equality ---
    [Description("50")] Equal, // ==
    [Description("50")] NotEqual, // !=
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
    [Description("90")] Not, // !
    [Description("95")] Dot, // .

    // --- Importance 100: Grouping (Handled by Depth) ---
    [Description("95")] ArrayRef,
    [Description("95")] LeftBracket, // [
    [Description("100")] RightBracket, // ]
    [Description("100")] LeftParen, // (
    [Description("100")] RightParen, // )


    // --- Importance 110: Literals (Leaves) ---
    [Description("110")] Number,
    [Description("110")] Text, // String literals
    [Description("110")] Time, // e.g. 5s
    [Description("110")] Vector,
    [Description("110")] List,
    [Description("110")] Bounds,
    [Description("110")] Unknown, // Variable names / Identifiers
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
        var token = new Token();
        token.Value = Text;

        // Basic Keyword/Symbol Mapping
        switch (Text.ToLower())
        {
            case "import": token.Type = TokenType.Import; break;
            case "class": token.Type = TokenType.Class; break;
            case "struct": token.Type = TokenType.Struct; break;
            case "function": token.Type = TokenType.Function; break;
            case "{": token.Type = TokenType.LeftCurly; break;
            case "}": token.Type = TokenType.RightCurly; break;
            case "(": token.Type = TokenType.LeftParen; break;
            case ")": token.Type = TokenType.RightParen; break;
            case "if": token.Type = TokenType.If; break;
            case "else": token.Type = TokenType.Else; break;
            case "while": token.Type = TokenType.While; break;
            case "for": token.Type = TokenType.For; break;
            case "foreach": token.Type = TokenType.Foreach; break;
            case "switch": token.Type = TokenType.Switch; break;
            case "case": token.Type = TokenType.Case; break;
            case "default": token.Type = TokenType.Default; break;
            case "=": token.Type = TokenType.Set; break;
            case "+=": token.Type = TokenType.AddEqual; break;
            case "-=": token.Type = TokenType.SubtractEqual; break;
            case "*=": token.Type = TokenType.MultiplyEqual; break;
            case "/=": token.Type = TokenType.DivideEqual; break;
            case "==": token.Type = TokenType.Equal; break;
            case "!=": token.Type = TokenType.NotEqual; break;
            case "<": token.Type = TokenType.LessThan; break;
            case ">": token.Type = TokenType.GreaterThan; break;
            case "<=": token.Type = TokenType.LessThanOrEqual; break;
            case ">=": token.Type = TokenType.GreaterThanOrEqual; break;
            case "+": token.Type = TokenType.Add; break;
            case "-": token.Type = TokenType.Subtract; break;
            case "*": token.Type = TokenType.Multiply; break;
            case "/": token.Type = TokenType.Divide; break;
            case "&": token.Type = TokenType.And; break;
            case "|": token.Type = TokenType.Or; break;
            case "xor": token.Type = TokenType.Xor; break;
            case "!": token.Type = TokenType.Not; break;
            case ".": token.Type = TokenType.Dot; break;
            default: token.Type = TokenType.Unknown; break;
        }

        if (Types.PullFromString(Text) != LLVMTypeRef.Void)
        {
            token.Type = TokenType.Type;
        }
        // Special Pattern Matching
        if (Text.Contains('\"'))
        {
            token.Type = TokenType.Text;
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

    public override string ToString()
    {
        return $"{Type}({Value})";
    }
}

public static class Tokens
{
    public static Token Parse(string Line)
    {
        if (string.IsNullOrWhiteSpace(Line)) return null;

        // Split by space - logic assumes tokens are space-separated
        var tokens = Line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => (Token)s)
            .ToList();

        return Structure(tokens);
    }

    public static Token? Structure(List<Token> tokens)
    {
        if (tokens == null || tokens.Count == 0) return null;

        while (IsWrappedInParens(tokens))
        {
            tokens = tokens.GetRange(1, tokens.Count - 2);
        }

        if (tokens.Count == 1) return tokens[0];

        int rootIndex = -1;
        int lowestEffectiveImportance = int.MaxValue;
        int currentDepth = 0;

        for (int i = 0; i < tokens.Count; i++)
        {
            var type = tokens[i].Type;

            // Still track depth for grouping
            if (type == TokenType.LeftParen)
            {
                currentDepth++;
                continue;
            }
            if (type == TokenType.RightParen)
            {
                currentDepth--;
                continue;
            }

            // Brackets also track depth for internal math: a[ 5 + 2 ]
            // But we allow LeftBracket to be a root if depth is 0
            if (type == TokenType.LeftBracket)
            {
                // If this is the start of an array access at current level
                int baseImp = EnumExtensions.GetImportance(type);
                int effImp = baseImp + currentDepth * 1000;

                if (effImp <= lowestEffectiveImportance)
                {
                    lowestEffectiveImportance = effImp;
                    rootIndex = i;
                }
                currentDepth++;
                continue;
            }
            if (type == TokenType.RightBracket)
            {
                currentDepth--;
                continue;
            }

            int baseImportance = EnumExtensions.GetImportance(type);
            if (baseImportance >= 100) continue;

            int effectiveImportance = baseImportance + currentDepth * 1000;

            bool isRightAssociative = type == TokenType.Set || type == TokenType.AddEqual ||
                                      type == TokenType.SubtractEqual || type == TokenType.MultiplyEqual ||
                                      type == TokenType.DivideEqual;

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

        if (rootIndex == -1) return tokens[0];

        var root = tokens[rootIndex];

        // --- SPECIAL CASE: ARRAY ACCESS ---
        if (root.Type == TokenType.LeftBracket)
        {
            // if rootIndex == 0, it's a list literal [1, 2], not an access a[1]
            if (rootIndex > 0)
            {
                root.Type = TokenType.ArrayRef;
                root.Value = "[]";
            }
            else
            {
                root.Type = TokenType.List;
            }
        }

        var leftSide = tokens.GetRange(0, rootIndex);
        var rightSide = tokens.GetRange(rootIndex + 1, tokens.Count - rootIndex - 1);

        // If it's an ArrayRef or List, the right side will end with a ']'
        // We need to remove that trailing bracket before recursing
        if ((root.Type == TokenType.ArrayRef || root.Type == TokenType.List) &&
            rightSide.Count > 0 && rightSide[^1].Type == TokenType.RightBracket)
        {
            rightSide.RemoveAt(rightSide.Count - 1);
        }

        var leftChild = Structure(leftSide);
        var rightChild = Structure(rightSide);

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
        var type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name == null) return null;

        var field = type.GetField(name);
        var attr = field.GetCustomAttribute<DescriptionAttribute>();

        return attr != null ? attr.Description : name;
    }

    public static int GetImportance(this Enum value)
    {
        string description = value.GetDescription();
        return int.TryParse(description, out int result) ? result : 0;
    }
}