using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class Work
{
    public static PassValue LoadValue(PassValue passValue)
    {
        return Values.Builder.BuildLoad2(passValue, passValue);
    }
}