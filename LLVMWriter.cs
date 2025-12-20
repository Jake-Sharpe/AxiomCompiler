namespace AxiomCompiler;

public static unsafe class LLVMWriter
{
    public static void Write(Line line)
    {
        switch (line.Main.Type)
        {
            case TokenType.Type:
                Variable variable = new Variable();
                variable.Name = line.Text.Split(" ")[1];
                if (line.Main.Children.Find(x => x.Type == TokenType.Set) != null)
                {
                    Token Set = line.Main.Children.Find(x => x.Type == TokenType.Set);
                }
                break;
        }
    }
}