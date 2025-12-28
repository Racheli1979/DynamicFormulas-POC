using System;
using System.Data;
using NCalc;

namespace FormulaEngine
{
    public static class NCalcFunctions
    {
        public static void Register(Expression e, DataRow dataRow, DataColumnCollection columns)
        {
            e.EvaluateFunction += (name, args) =>
            {
                switch (name.ToLower())
                {
                    case "sqrt":
                        args.Result = Math.Sqrt(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                    case "log":
                        args.Result = Math.Log(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                    case "abs":
                        args.Result = Math.Abs(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                    case "pow":
                        args.Result = Math.Pow(Convert.ToDouble(args.Parameters[0].Evaluate()), Convert.ToDouble(args.Parameters[1].Evaluate()));
                        break;
                    case "iif":
                        Expression condExp = new Expression(args.Parameters[0].ParsedExpression.ToString());

                        // רישום isequal
                        condExp.EvaluateFunction += (name2, args2) =>
                        {
                            if (name2.ToLower() == "isequal")
                            {
                                var x = args2.Parameters[0].Evaluate();
                                var y = args2.Parameters[1].Evaluate();
                                args2.Result = (x == null && y == null) || (x != null && y != null && x.Equals(y));
                            }
                        };

                        // העברת כל השדות מהשורה ל-condExp
                        foreach (DataColumn col in columns)
                            condExp.Parameters[col.ColumnName] = dataRow[col];

                        bool condition = Convert.ToBoolean(condExp.Evaluate());
                        var trueVal = args.Parameters[1].Evaluate();
                        var falseVal = args.Parameters[2].Evaluate();

                        args.Result = condition ? trueVal : falseVal;
                        break;
                }
            };
        }
    }
}
