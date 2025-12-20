using System.Text.RegularExpressions;
using AxiomCompiler;
using LLVMSharp.Interop;

if (args[0] == "build")
{
    string path = args[1];
    string[] TempLines = File.ReadAllLines(path);
    List<string> OutputLines = new List<string>();
    foreach (string line in TempLines)
    {
        OutputLines.Add(CleanLine(line));
    }

    List<Line> lines = new List<Line>();
    int index = 0;
    foreach (string linetext in OutputLines)
    {
        string[] cols = linetext.Split(' ');
        Line line = new Line();
        foreach (string col in cols)
        {
            line.Tokens.Add(Tokens.Parse(linetext));
            line.Text = linetext;
            line.LineNumber = index;
        }
        line.Main = Tokens.Structure(line.Tokens);
        lines.Add(line);
        index++;
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
    string cleaned = "";
    cleaned = cleaned.Replace("[", " [ ");
    cleaned = cleaned.Replace("(", " ( ");
    cleaned = cleaned.Replace(")", " ) ");
    cleaned = cleaned.Replace("]", " ] ");
    cleaned = cleaned.Replace("<", " < ");
    cleaned = cleaned.Replace(">", " > ");
    // 3. Replace multiple whitespace characters with a single space
    // 4. Trim leading and trailing whitespace
    cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
    

    return cleaned;
}
public static class Values
{
    public static LLVMBuilderRef Builder = new LLVMBuilderRef();
    public static Location BuildLocation = new Location();
    public static List<Variable> Variables = new List<Variable>();
    public static List<Class> Classes = new List<Class>();
}


