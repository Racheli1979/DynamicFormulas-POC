using System;
using NCalc;

namespace FormulaCalculatorEF.FormulaEngine
{
    public static class NCalcFunctions
    {
        public static void Register(Expression e, object dataRow)
        {
            e.EvaluateFunction += (name, args) =>
            {
                switch (name.ToLower())
                {
                    case "sqrt":
                        args.Result = args.Parameters[0].Evaluate() is null ? null : Math.Sqrt(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                    case "log":
                        args.Result = args.Parameters[0].Evaluate() is null ? null : Math.Log(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                    case "abs":
                        args.Result = args.Parameters[0].Evaluate() is null ? null : Math.Abs(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                    case "pow":
                        var b = args.Parameters[0].Evaluate();
                        var p = args.Parameters[1].Evaluate();
                        args.Result = (b == null || p == null) ? null : Math.Pow(Convert.ToDouble(b), Convert.ToDouble(p));
                        break;
                    case "iif":
                        Expression condExp = new Expression(args.Parameters[0].ParsedExpression.ToString());
                        condExp.EvaluateFunction += (name2, args2) =>
                        {
                            if (name2.ToLower() == "isequal")
                            {
                                var x = args2.Parameters[0].Evaluate();
                                var y = args2.Parameters[1].Evaluate();
                                args2.Result = (x == null && y == null) || (x != null && y != null && x.Equals(y));
                            }
                        };
                        foreach (var prop in dataRow.GetType().GetProperties())
                            condExp.Parameters[prop.Name] = prop.GetValue(dataRow);

                        bool condition = condExp.Evaluate() != null && Convert.ToBoolean(condExp.Evaluate());
                        args.Result = condition ? args.Parameters[1].Evaluate() : args.Parameters[2].Evaluate();
                        break;
                }
            };
        }
    }
}
