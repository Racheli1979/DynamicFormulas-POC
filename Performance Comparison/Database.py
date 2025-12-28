import os
from dotenv import load_dotenv
from sqlalchemy import create_engine

load_dotenv()
server = os.getenv("DB_SERVER")
database = os.getenv("DB_DATABASE")
driver = os.getenv("DB_DRIVER")
results_table = os.getenv("DB_TABLE_RESULTS")

connection_string = f"mssql+pyodbc://{server}/{database}?driver={driver.replace(' ', '+')}&trusted_connection=yes"
engine = create_engine(connection_string)
