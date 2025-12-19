using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class Resources
{
    
}

public unsafe class Space
{
    public List<Variable> Variables = new List<Variable>();
}

public unsafe class PassValue
{
    public LLVMValueRef CPUPointer;
    public LLVMValueRef SharedPointer;
    public LLVMValueRef GlobalPointer;
    public LLVMValueRef Loaded;
    public bool LoadBool;
    public LLVMTypeRef Type;
    public Location Location =  Location.RAM;
    public static implicit operator LLVMTypeRef(PassValue Parse)
    {
        return LLVM.TypeOf(Parse);
    }
    public static implicit operator LLVMOpaqueValue*(PassValue Parse)
    {
        switch (Parse.Location)
        {
            case Location.RAM:
                return Parse.CPUPointer;
            case Location.GLOBAL:
                return Parse.GlobalPointer;
            case Location.SHARED:
                return Parse.SharedPointer;
            default:
                return Parse.Loaded;
        }
    }
    public static implicit operator LLVMValueRef(PassValue Parse)
    {
        switch (Parse.Location)
        {
            case Location.RAM:
                return Parse.CPUPointer;
            case Location.GLOBAL:
                return Parse.GlobalPointer;
            case Location.SHARED:
                return Parse.SharedPointer;
            default:
                return Parse.Loaded;
        }
    }
    public static implicit operator PassValue(LLVMValueRef Parse)
    {
        PassValue passValue = new PassValue();
        switch (Values.BuildLocation)
        {
            case Location.RAM:
                passValue.CPUPointer = Parse;
                break;
            case Location.GLOBAL:
                passValue.GlobalPointer = Parse;
                break;
            case Location.SHARED:
                passValue.SharedPointer = Parse;
                break;
        }
        passValue.Location = Values.BuildLocation;
        return passValue;
    }
}

public class Variable : PassValue
{
    public string Name;
}

public class Number : PassValue
{
    public double Value;
}

public class Array : Variable
{
    public int Size;
}

public enum Location
{
    RAM,
    GLOBAL,
    SHARED,
    LOADED
}