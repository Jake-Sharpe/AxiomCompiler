using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using LLVMSharp;
using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class Resources
{
    public static PassValue Get(Token token)
    {
        string Name = token.Value;
        if (token.Children.Count != 0)
        {
            return Work.Excecute(token);
        }
        foreach (var VARIABLE in Values.BuildSpace.Variables)
        {
            if (VARIABLE.Name == Name)
            {
                return VARIABLE;
            }
        }
        if (double.TryParse(Name, out double value))
        {
            return value;
        }
        if (Name[0] == '\'')
        {
            string str = Name.Substring(1, Name.Length - 2);
            String s = new String();
            s.Value = str;
            sbyte* text = StringtoSbyteptr(str);
            s.Loaded = LLVM.ConstString(text, (uint)str.Length - 1, 0);
            text = null;
            s.Location = Location.RAM;
            return s;
        }
        throw new Exception($"Time or other type not implemented: {Name}");
    }
    public static unsafe sbyte* StringtoSbyteptr(string text)
    {
        return (sbyte*) Marshal.StringToHGlobalAnsi(text);
    }
}

public unsafe class Space
{
    public List<Variable> Variables = new List<Variable>();
}

public class Class
{
    public string Name;
    public List<(string, LLVMTypeRef Type)> Properties;
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
    public Class AssiotiatedClass = new Class();
    public void Set(LLVMValueRef Value,bool LoadedSet = false)
    {
        if (LoadedSet)
        {
            Loaded = Value;
        }
        else
        {
            switch (Location)
            {
                case Location.RAM:
                    CPUPointer = Value;
                    break;
                case Location.GLOBAL:
                    GlobalPointer = Value;
                    break;
                case Location.SHARED:
                    SharedPointer = Value;
                    break;
            }
        }
    }
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
    public static implicit operator PassValue(double Parse)
    {
        PassValue passValue = new PassValue();
        double value = Parse;
        if (double.IsInteger(value))
        {
            if (double.MaxMagnitude(2147483647,value) == value)
            {
                return LLVM.ConstReal(LLVM.Int32Type(), value);
            }
            return LLVM.ConstReal(LLVM.Int64Type(), value);
        }
        return LLVM.ConstReal(LLVM.DoubleType(), value);
        
    }
    public static implicit operator PassValue(LLVMOpaqueValue* Parse)
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

public class String : PassValue
{
    public string Value;
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