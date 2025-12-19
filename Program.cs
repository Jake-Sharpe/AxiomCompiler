using System.Text.RegularExpressions;
using AxiomCompiler;using LLVMSharp.Interop;

public static class Values
{
    public static LLVMBuilderRef Builder = new LLVMBuilderRef();
    public static Location BuildLocation = new Location();
}
if (args[0] == "build")
{
    string path = args[1];
    string[] TempLines = File.ReadAllLines(path);
    List<string> OutputLines = new List<string>();
    foreach (string line in TempLines)
    {
        OutputLines.Add(CleanLine(line));
    }

    List<Token> tokens = new List<Token>();
    foreach (string line in OutputLines)
    {
        tokens.Add(Tokens.Parse(line));
    }
}

static string CleanLine(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return string.Empty;

    // 1. Remove single-line comments (// ...)
    int commentIndex = input.IndexOf("//");
    if (commentIndex >= 0)
    {
        input = input.Substring(0, commentIndex);
    }

    // 2. Remove the word "string" (as per your example)
    // Note: \b ensures we only match the whole word "string", not "substring"
    input = Regex.Replace(input, @"\bstring\b", "");

    // 3. Replace multiple whitespace characters with a single space
    // 4. Trim leading and trailing whitespace
    string cleaned = Regex.Replace(input, @"\s+", " ").Trim();

    return cleaned;
}