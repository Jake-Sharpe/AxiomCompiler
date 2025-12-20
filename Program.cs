using System.Text.RegularExpressions;
using AxiomCompiler;
using LLVMSharp.Interop;

if (args[0] == "build")
{
    string path = args[1];
    string[] Lines = File.ReadAllLines(path);
    List<Line> lines = new List<Line>();
    int index = 0;
    foreach (string linetexta in Lines)
    {
        string linetext = Extras.CleanLine(linetexta);
        string[] cols = Extras.Split(linetext);
        Line line = new Line();
        line.Text = linetext;
        line.LineNumber = index;
        foreach (string col in cols)
        {
            line.Tokens.Add(Tokens.Parse(col));
        }
        line.Main = Tokens.Structure(line.Tokens);
        lines.Add(line);
        index++;
    }
}

public class Extras
{
    public static string[] Split(string line)
    {
        List<string> Columns = new List<string>();
        bool InString = false;
        string ToAdd = "";
        bool change = false;
        foreach (char c in line)
        {
            change = false;
            if (c == '\"')
            {
                InString = !InString;
                Columns.Add(ToAdd);
                ToAdd = "";
            }
            if (c == ' ')
            {
                Columns.Add(ToAdd);
                ToAdd = "";
                change = true;
            }
            if (!change)
            {
                ToAdd = ToAdd + c;
            }
        }
        return Columns.ToArray();
    }
    public static string CleanLine(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // 1. Remove single-line comments (// ...)
        int commentIndex = input.IndexOf("//");
        if (commentIndex >= 0)
        {
            input = input.Substring(0, commentIndex);
        }

        string cleaned = input;
        cleaned = cleaned.Replace("[", " [ ");
        cleaned = cleaned.Replace("(", " ( ");
        cleaned = cleaned.Replace(")", " ) ");
        cleaned = cleaned.Replace("]", " ] ");
        cleaned = cleaned.Replace("<", " < ");
        cleaned = cleaned.Replace(">", " > ");
        cleaned = cleaned.Replace("+", " + ");
        cleaned = cleaned.Replace("-", " - ");
        cleaned = cleaned.Replace("*", " * ");
        cleaned = cleaned.Replace("/", " / ");
        // 3. Replace multiple whitespace characters with a single space
        // 4. Trim leading and trailing whitespace
        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();


        return cleaned;
    }
}

public static class Values
{
    public static LLVMBuilderRef Builder = new LLVMBuilderRef();
    public static Location BuildLocation = new Location();
    public static Space BuildSpace = new Space();
    public static List<Space> Spaces = new List<Space>();
    public static List<Class> Classes = new List<Class>();
}



