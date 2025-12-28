import pandas as pd
import time
from FormulaUtils import calculating_formulas

def execute_in_threads(t, df_data, df_dict):
    start_time = time.perf_counter()
    res = calculating_formulas(df_data, t.formula_py, t.tnai, df_dict=df_dict)
    total_time = time.perf_counter() - start_time
    return pd.DataFrame({
        "data_id": df_data["id"],
        "targil_id": t.id,
        "method": "Python",
        "result": res,
        "execution_time": total_time
    })
