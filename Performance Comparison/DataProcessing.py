import pandas as pd
import numpy as np

def process_data(df_results):
    methods = ['.Net', 'Python', 'DB Procedure']
    dfs = {method: df_results[df_results['method'] == method].copy() for method in methods}

    for col in ['result', 'execution_time']:
        for method in methods:
            dfs[method][col] = pd.to_numeric(dfs[method][col], errors='coerce')

    merged_all = dfs['.Net'][['data_id', 'targil_id', 'result']].rename(columns={'result': 'result_.Net'})
    merged_all = merged_all.merge(
        dfs['Python'][['data_id', 'targil_id', 'result']].rename(columns={'result': 'result_Python'}),
        on=['data_id', 'targil_id'],
        how='inner'
    )
    merged_all = merged_all.merge(
        dfs['DB Procedure'][['data_id', 'targil_id', 'result']].rename(columns={'result': 'result_DB'}),
        on=['data_id', 'targil_id'],
        how='inner'
    )

    # חישוב mismatch_count
    tolerance = 1e-9
    def calculate_mismatch(row):
        values = [
            row['result_.Net'],
            row['result_Python'],
            row['result_DB']
        ]
        # מקרה 1: כולם NaN → לא mismatch
        if all(pd.isna(v) for v in values):
            return False
        # מקרה 2: חלק NaN וחלק לא → mismatch
        if any(pd.isna(v) for v in values):
            return True
        # מקרה 3: כולם מספרים → בדיקת טולרנס
        return not (
                abs(values[0] - values[1]) < tolerance and
                abs(values[0] - values[2]) < tolerance and
                abs(values[1] - values[2]) < tolerance
        )

    merged_all['mismatch'] = merged_all.apply(calculate_mismatch, axis=1)

    # יצירת df_clean
    df_clean = df_results.dropna(subset=['execution_time'])
    df_clean = df_clean[df_clean['execution_time'] >= 0]

    # merge עם mismatch
    df_clean = df_clean.merge(
        merged_all[['data_id', 'targil_id', 'mismatch']],
        on=['data_id', 'targil_id'],
        how='inner'
    )

    # חישוב mismatch_count
    df_clean['mismatch_count'] = df_clean['mismatch'].fillna(False).astype(int)

    return merged_all, dfs, df_clean