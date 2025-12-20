using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class Work
{
    public static PassValue LoadValue(PassValue passValue)
    {
        return Values.Builder.BuildLoad2(passValue, passValue);
    }
    public static LLVMValueRef Get(PassValue passValue)
    {
        if (passValue.GetType() == typeof(Variable))
        {
            return LoadValue(passValue);
        }
        if (passValue.GetType() == typeof(Number))
        {
            return passValue;
        }
        if (passValue.GetType() == typeof(String))
        {
            return passValue;
        }
    }
}