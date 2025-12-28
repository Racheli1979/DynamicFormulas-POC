import matplotlib.pyplot as plt
import seaborn as sns
from FixHebrew import fix_hebrew
import numpy as np

def generate_graphs(df_clean, stats_df):
    # יצירת גרפים נפרדים לכל שיטה
    for method in stats_df['method']:
        subset = df_clean[df_clean['method'] == method]
        stat = stats_df[stats_df['method'] == method].iloc[0]

        plt.figure(figsize=(7, 4))
        ax = sns.boxplot(x='execution_time', data=subset, orient='h', color='lightblue')

        mean = stat['mean']
        min_val = stat['min']
        max_val = stat['max']
        mismatches = subset['mismatch_count'].sum()

        explanation = (
            f"mean: {mean:.4f}\n"
            f"min-max: {min_val:.4f}-{max_val:.4f}\n"
            f"חוסר התאמות: {mismatches}"
        )

        x_text = max_val * 0.5
        y_text = 0
        ax.text(x_text, y_text, fix_hebrew(explanation), va='center', ha='right', fontsize=10,
                bbox=dict(facecolor='white', alpha=0.7, boxstyle='round,pad=0.5'))

        plt.title(fix_hebrew(f'זמן ריצה - {method}'))
        plt.xlabel(fix_hebrew('שניות'))
        if method == 'Python':
            plt.legend(title=fix_hebrew("Mismatch"), loc='upper right')
            plt.tight_layout()
            plt.savefig(f"{method}.png")
            plt.close()
        elif method == '.Net':
            plt.legend(title=fix_hebrew("Mismatch"), loc='upper right')
            plt.tight_layout()
            plt.savefig(".Net.png")
            plt.close()
        else:
            plt.legend(title=fix_hebrew("Mismatch"), loc='upper right')
            plt.tight_layout()
            plt.savefig("DB_Procedure.png")
            plt.close()

    # יצירת גרף משולב עם זמן ריצה ממוצע וסטיית תקן
    plt.figure(figsize=(8, 6))
    bars = plt.bar(
        stats_df['method'],
        stats_df['mean'],
        yerr=stats_df['std'],
        capsize=5,
        color=['skyblue', 'lightgreen', 'salmon']
    )

    plt.ylim(0, stats_df['mean'].max() + stats_df['std'].max() + 0.01)

    tick_step = 0.1
    max_tick = round(plt.ylim()[1] + tick_step, 1)
    plt.yticks(np.arange(0, max_tick, tick_step))

    for i, bar in enumerate(bars):
        height = bar.get_height()
        plt.text(bar.get_x() + bar.get_width() / 2, height + 0.002, f"{height:.3f}",
                 ha='center', va='bottom', fontsize=10)

    plt.xlabel(fix_hebrew('שיטה'))
    plt.ylabel(fix_hebrew('זמן ריצה (שניות)'))
    plt.title(fix_hebrew('השוואת זמן ריצה ממוצע עם סטיית תקן - גרף משולב'))
    plt.tight_layout()
    plt.savefig("combined_graph.png")
    plt.close()
