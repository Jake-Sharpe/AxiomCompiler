using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class MathWork
{
    public static LLVMOpaqueValue* Add(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildAdd(lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFAdd(lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* Sub(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildSub(lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFSub(lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* Mul(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildMul(lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFMul(lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* Div(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        return Values.Builder.BuildFDiv(lhs, rhs);
    }
    public static LLVMOpaqueValue* And(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        lhs = Types.JustConvert(lhs,LLVMTypeRef.Int1);
        rhs = Types.JustConvert(rhs, LLVMTypeRef.Int1);
        return Values.Builder.BuildAnd(lhs, rhs);
    }
    public static LLVMOpaqueValue* Or(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        lhs = Types.JustConvert(lhs,LLVMTypeRef.Int1);
        rhs = Types.JustConvert(rhs, LLVMTypeRef.Int1);
        return Values.Builder.BuildOr(lhs, rhs);
    }
    public static LLVMOpaqueValue* Xor(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        lhs = Types.JustConvert(lhs,LLVMTypeRef.Int1);
        rhs = Types.JustConvert(rhs, LLVMTypeRef.Int1);
        return Values.Builder.BuildXor(lhs, rhs);
    }
    public static LLVMOpaqueValue* Not(LLVMOpaqueValue* lhs)
    {
        lhs = Types.JustConvert(lhs,LLVMTypeRef.Int1);
        return Values.Builder.BuildNot(lhs);
    }
    public static LLVMOpaqueValue* Equal(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ,lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ,lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* NotEqual(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildICmp(LLVMIntPredicate.LLVMIntNE,lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFCmp(LLVMRealPredicate.LLVMRealONE,lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* Over(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT,lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT,lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* Under(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT,lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT,lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* OverEqual(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE,lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE,lhs, rhs);
        }
    }
    public static LLVMOpaqueValue* UnderEqual(LLVMOpaqueValue* lhs, LLVMOpaqueValue* rhs)
    {
        LLVMTypeRef BiggestType = Types.BiggestValue(LLVM.TypeOf(lhs), LLVM.TypeOf(rhs));
        lhs = Types.JustConvert(lhs,BiggestType);
        rhs = Types.JustConvert(rhs, BiggestType);
        if (Types.Select(BiggestType) == "int")
        {
            return Values.Builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE,lhs, rhs);
        }
        else
        {
            return Values.Builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE,lhs, rhs);
        }
    }
}