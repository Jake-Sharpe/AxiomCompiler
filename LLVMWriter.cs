using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class LLVMWriter
{
    public static int CreateVar(string Name,LLVMTypeRef Type)
    {
        Variable var = new Variable();
        var.Name = Name;
        var.Type = Type;
        var.Location = Values.BuildLocation;
        var.Set(Values.Builder.BuildAlloca(Type, var.Name));
        Values.BuildSpace.Variables.Add(var);
        return Values.BuildSpace.Variables.Count - 1;
    }
    public static void Write(Line line)
    {
        string[] cols = Extras.Split(line.Text);
        switch (line.Main.Type)
        {
            case TokenType.Type:
                int Dimensions = int.Parse(cols[1]);
                string Name = cols[2];
                switch (Dimensions)
                {
                    case 1:
                        var Index = CreateVar(Name,Types.PullFromString(line.Main.Value));
                        if (cols.Length > 3)
                        {
                            Values.BuildSpace.Variables[Index].Set(Resources.Get(line.Main.Children[1]));
                        }
                        break;
                    default:
                        for (int i = 1; i < Dimensions; i++)
                        {
                            CreateVar(Name + "." + i.ToString(),Types.PullFromString(line.Main.Value));
                        }
                        break;
                }
                break;
            case TokenType.Set:
                switch (line.Main.Children[0].Type)
                {
                    case TokenType.ArrayRef: //TODO
                        Variable Array = Work.Pull(line.Main.Children[0].Children[0].Value);
                        
                        break;
                    case TokenType.Unknown:
                        Variable var = Work.Pull(line.Main.Children[0].Value);
                        var.Set(Work.StoreValue(Resources.Get(line.Main.Children[1]),var));
                        Work.Set(var);
                        break;
                }
                break;
        }
    }
}