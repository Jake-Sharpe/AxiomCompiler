using LLVMSharp.Interop;

namespace AxiomCompiler;

public unsafe static class Types
{
    public static LLVMOpaqueValue* JustConvert(LLVMOpaqueValue* Value, LLVMTypeRef TypeRef)
    {
        LLVMTypeRef InTypeRef = LLVM.TypeOf(Value);
        if (InTypeRef != TypeRef)
        {

            if (Types.Select(InTypeRef) == "int" && Types.Select(TypeRef) == "int")
            {
                if (Types.ValueSize(InTypeRef).Item1 > Types.ValueSize(TypeRef).Item1)
                    Value = LLVM.BuildTrunc(Values.Builder, Value, TypeRef, null);
                else
                    Value = LLVM.BuildSExt(Values.Builder, Value, TypeRef, null);
            }
            else
            {
                if (InTypeRef != LLVMTypeRef.FP128)
                    Value = LLVM.BuildFPExt(Values.Builder, Value, TypeRef, null);
                else
                    Value = LLVM.BuildFPTrunc(Values.Builder, Value, TypeRef, null);
            }

        }
        return Value;
    }
    public static string Select(LLVMTypeRef Type)
    {
        if (Type == LLVMTypeRef.Int1) return "int";
        if (Type == LLVMTypeRef.Int8) return "int";
        if (Type == LLVMTypeRef.Int16) return "int";
        if (Type == LLVMTypeRef.Int32) return "int";
        if (Type == LLVMTypeRef.Int64) return "int";
        if (Type == LLVMTypeRef.Float) return "decimal";
        if (Type == LLVMTypeRef.BFloat) return "decimal";
        if (Type == LLVMTypeRef.Half) return "decimal";
        if (Type == LLVMTypeRef.Double) return "decimal";
        if (Type == LLVMTypeRef.FP128) return "decimal";
        throw new NotImplementedException($"Type {Type} not implemented");
    }
    public static LLVMTypeRef BiggestValue(LLVMTypeRef Type1, LLVMTypeRef Type2)
    {
        var Sizes = (Math.Max(ValueSize(Type1).Item1, ValueSize(Type2).Item1),
            Math.Max(ValueSize(Type1).Item2, ValueSize(Type2).Item2));
        var var1 = ValueSize(Type1);
        var var2 = ValueSize(Type2);
        if (Sizes.Item1 == var1.Item1)
            if (Sizes.Item2 == var1.Item2)
                return Type1;

        if (Sizes.Item1 == var2.Item1)
            if (Sizes.Item2 == var2.Item2)
                return Type2;

        return LLVMTypeRef.FP128;
    }

    public static (int, int) ValueSize(LLVMTypeRef Type)
    {
        if (Type == LLVMTypeRef.Int1) return (1, 0);
        if (Type == LLVMTypeRef.Int8) return (8, 0);
        if (Type == LLVMTypeRef.Int16) return (16, 0);
        if (Type == LLVMTypeRef.Int32) return (32, 0);
        if (Type == LLVMTypeRef.Int64) return (64, 0);
        if (Type == LLVMTypeRef.BFloat) return (8, 8);
        if (Type == LLVMTypeRef.Half) return (11, 5);
        if (Type == LLVMTypeRef.Float) return (24, 8);
        if (Type == LLVMTypeRef.Double) return (52, 15);
        if (Type == LLVMTypeRef.X86FP80) return (65, 15);
        if (Type == LLVMTypeRef.FP128) return (112, 15);
        if (Type == LLVMTypeRef.PPCFP128) return (105, 11);
        throw new NotImplementedException($"Type {Type} not implemented");
    }

    public static LLVMTypeRef PullFromString(string text)
    {
        switch (text)
        {
            case "bool":
                return LLVM.Int1Type();
            case "byte":
                return LLVM.Int8Type();
            case "short":
                return LLVM.Int16Type();
            case "int":
                return LLVM.Int32Type();
            case "long":
                return LLVM.Int64Type();
            case "float":
                return LLVM.FloatType();
            case "double":
                return LLVM.DoubleType();
            case "shortfloat":
                return LLVM.BFloatType();
            case "precise":
                return LLVMTypeRef.FP128;
            case "half":
                return LLVMTypeRef.Half;
            default:
                return LLVMTypeRef.Void;
        }
    }
}