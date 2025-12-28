import numpy as np
import re
import math
import pandas as pd

# מיפוי פונקציות SQL ל-Python
sql_function_to_python = {
    "SQRT": "np.sqrt",
    "LOG": "np.log",
    "ABS": "np.abs",
    "POWER": "np.power"
}

# קימפול regex עבור CASE WHEN
case_when_pattern = re.compile(r"CASE\s+WHEN\s+(.*?)\s+THEN\s+(.*?)\s+ELSE\s+(.*?)\s+END", re.IGNORECASE)

def case_when_to_python(expr: str) -> str:
    if expr is None:
        return ""
    match = case_when_pattern.match(expr)
    if match:
        condition, true_expr, false_expr = match.groups()
        condition = condition.replace("<>", "!=")
        condition = re.sub(r'(?<![<>=!])=(?!=)', '==', condition)
        return f"np.where({condition}, {true_expr}, {false_expr})"
    return expr

def sql_to_python(expr: str) -> str:
    if expr is None:
        return ""
    expr_conv = case_when_to_python(expr.strip())
    for sql_func, py_func in sql_function_to_python.items():
        expr_conv = re.sub(rf'\b{sql_func}\b', py_func, expr_conv, flags=re.IGNORECASE)
    def replace_power(match):
        left, right = match.group(1), match.group(2)
        return f"{left}**{right}"
    expr_conv = re.sub(r'(\w+)\s*\^\s*(\w+)', replace_power, expr_conv)
    expr_conv = expr_conv.replace("np.np", "np").replace("math.math", "math")
    return expr_conv

def calculating_formulas(df, formula, condition=None, df_dict=None):
    formula_py = sql_to_python(formula)
    if not formula_py:
        return pd.Series(np.nan, index=df.index)
    if df_dict is None:
        df_dict = df.to_dict(orient="series")
    safe_globals = {"__builtins__": None, "np": np, "math": math}

    if "np.where" in formula_py:
        return pd.Series(eval(formula_py, safe_globals, df_dict))
    if condition:
        condition_py = sql_to_python(condition)
        mask = pd.Series(True, index=df.index) if not condition_py else eval(condition_py, safe_globals, df_dict)
        df_dict_subset = {k: v[mask] for k, v in df_dict.items()}
        values = eval(formula_py, safe_globals, df_dict_subset)
        res_array = pd.Series(np.nan, index=df.index)
        res_array[mask] = values
        return res_array
    return pd.Series(eval(formula_py, safe_globals, df_dict))
