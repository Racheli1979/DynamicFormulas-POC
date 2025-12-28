import os
import time
import pandas as pd
import numpy as np
from concurrent.futures import ThreadPoolExecutor
from dotenv import load_dotenv
from sqlalchemy import create_engine
from FormulaUtils import calculating_formulas, sql_to_python
from ThreadExecutor import execute_in_threads

# טעינת משתני סביבה
load_dotenv()

# פרטי חיבור למסד נתונים
server = os.getenv("DB_SERVER")
database = os.getenv("DB_DATABASE")
driver = os.getenv("DB_DRIVER")
table_data = os.getenv("DB_TABLE_DATA")
table_formulas = os.getenv("DB_TABLE_FORMULAS")
results_table = os.getenv("DB_TABLE_RESULTS")

# --- חיבור למסד הנתונים ---
connection_string = f"mssql+pyodbc://{server}/{database}?driver={driver.replace(' ', '+')}&trusted_connection=yes"
engine = create_engine(connection_string)

# --- קריאת נתונים ונוסחאות ---
df_data = pd.read_sql(f"SELECT * FROM {table_data}", engine)
df_formulas = pd.read_sql(f"SELECT * FROM {table_formulas}", engine)

# --- הכנת נתונים ---
df_dict = df_data.to_dict(orient="series")
df_formulas['formula_py'] = df_formulas['targil'].apply(sql_to_python)

# --- הרצת נוסחאות עם ThreadPoolExecutor ---
with ThreadPoolExecutor() as executor:
    results = list(executor.map(lambda t: execute_in_threads(t, df_data, df_dict), df_formulas.itertuples()))

# --- איחוד תוצאות ---
df_results = pd.concat(results, ignore_index=True)

# --- שמירה חזרה ל-SQL Server ---
df_results.to_sql(results_table, engine, if_exists="append", index=False)
