using LLVMSharp.Interop;

namespace AxiomCompiler;

public static unsafe class Work
{
    public static PassValue LoadValue(PassValue passValue)
    {
        return Values.Builder.BuildLoad2(passValue, passValue);
    }
    public static PassValue StoreValue(LLVMOpaqueValue* Input, PassValue ToStore)
    {
        ToStore.Set(Values.Builder.BuildStore(Input,ToStore));
        return ToStore;
    }
    public static Variable Pull(string Name)
    {
        foreach (var variable in Values.BuildSpace.Variables)
        {
            if (variable.Name == Name)
            {
                return variable;
            }
        }
        throw  new NotImplementedException("Not Found");
    }
    public static void Set(Variable var)
    {
        for (int i = 0; i < Values.BuildSpace.Variables.Count;i++) 
        {
            var variable = Values.BuildSpace.Variables[i];
            if (variable.Name == var.Name)
            {
                Values.BuildSpace.Variables[i] = var;
            }
        }
        throw  new NotImplementedException("Not Found");
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
        throw  new NotImplementedException("Wrong Type");
    }
    public static LLVMValueRef Get(PassValue passValue,PassValue index)
    {
        if (passValue.GetType() == typeof(Array))
        {
            LLVMValueRef[] indices = new LLVMValueRef[] {LLVM.ConstInt(LLVM.Int32Type(),0,0),index};
            return Values.Builder.BuildGEP2(passValue.Type, passValue, indices, null);
        }
        throw  new NotImplementedException("Wrong Type");
    }
    public static void Update(Variable var)
    {
        for (int i = 0; i < Values.BuildSpace.Variables.Count; i++)
        {
            var variable = Values.BuildSpace.Variables[i];
            if (variable.Name == var.Name)
            {
                Values.BuildSpace.Variables[i] = var;
            }
        }
    }
    public static PassValue Excecute(Token tree,Token ToOperate)
    {
        throw new NotImplementedException("Function currently under construction");
        PassValue LeftValue = Resources.Get(tree.Children[0]);
        LeftValue.Loaded = LoadValue(LeftValue);
        PassValue RightValue = Resources.Get(tree.Children[1]);
        RightValue.Loaded = LoadValue(RightValue);
        PassValue MiddleValue = Resources.Get(tree.Children[1]);
        MiddleValue.Loaded = LoadValue(MiddleValue);
        switch (tree.Type)
        {
            case TokenType.AddEqual:
                return MathWork.Add(MiddleValue,MathWork.Add(RightValue.Loaded, LeftValue.Loaded));
            case TokenType.SubtractEqual:
                return MathWork.Sub(MiddleValue,MathWork.Sub(LeftValue.Loaded, RightValue.Loaded));
            case TokenType.MultiplyEqual:
                return MathWork.Mul(MiddleValue,MathWork.Mul(LeftValue.Loaded, RightValue.Loaded));
            case TokenType.DivideEqual:
                return MathWork.Mul(MiddleValue,MathWork.Div(LeftValue.Loaded, RightValue.Loaded));
        }
    }
    public static PassValue Excecute(Token tree)
    {
        PassValue LeftValue = Resources.Get(tree.Children[0]);
        PassValue RightValue = new PassValue();
        LeftValue.Loaded = LoadValue(LeftValue);
        if (tree.Children.Count == 2) //if Not a singular operation
        {
            RightValue = Resources.Get(tree.Children[1]);
            RightValue.Loaded = LoadValue(RightValue);
        }
        var MathOperations = new[] {TokenType.Add, TokenType.Subtract, TokenType.Multiply, TokenType.Divide};
        var Bool = new[] {TokenType.And, TokenType.Xor, TokenType.Or, TokenType.Not};
        switch (tree.Type)
        {
            case TokenType.Add:
                return MathWork.Add(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.Subtract:
                return MathWork.Sub(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.Multiply:
                return MathWork.Mul(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.Divide:
                return MathWork.Div(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.And:
                return MathWork.And(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.Xor:
                return MathWork.Xor(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.Or:
                return MathWork.Or(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.Not:
                return MathWork.Not(LeftValue.Loaded);
            case TokenType.Equal:
                return MathWork.Equal(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.NotEqual:
                return MathWork.NotEqual(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.LessThan:
                return MathWork.Under(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.LessThanOrEqual:
                return MathWork.UnderEqual(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.GreaterThan:
                return MathWork.Over(LeftValue.Loaded, RightValue.Loaded);
            case TokenType.GreaterThanOrEqual:
                return MathWork.OverEqual(LeftValue.Loaded, RightValue.Loaded);
        }
        throw new NotImplementedException("Wrong Type");
    }
    
}