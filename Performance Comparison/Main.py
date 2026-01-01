from Database import engine
from DataProcessing import process_data
from GraphGeneration import generate_graphs
from PdfReport import generate_pdf_report
from FixHebrew import fix_hebrew
import pandas as pd
import os

# קריאת תוצאות מהמסד
df_results = pd.read_sql(f"SELECT * FROM {os.getenv('DB_TABLE_RESULTS')}", engine)

# עיבוד נתונים
merged_all, dfs, df_clean = process_data(df_results)  

# החלפת ערכים ששווים ל-0 ב-0.001
df_clean.loc[df_clean['execution_time'] == 0, 'execution_time'] = 0.001

# חישוב סטטיסטיקות
stats_df = df_clean.groupby('method')['execution_time'].agg(
    mean='mean', median='median', std='std', min='min', max='max').reset_index()

# חישוב השיטה עם החציון הנמוך ביותר
best_method = stats_df.loc[stats_df['median'].idxmin(), 'method']

# יצירת גרפים
generate_graphs(df_clean, stats_df)

# יצירת טקסט יתרונות/חסרונות והמלצות
analysis_text = """
סיכום ביצועים של השיטות:

. .NET:
יתרונות:
  • יתרון ביצועים: חישובים עם .NET, במיוחד אם משתמשים בטכנולוגיות כמו DataTable.Compute או ספריות חיצוניות כמו NCalc, עשויים להיות יעילים מאוד בזכות האופטימיזציות שבוצעו על ידי המהדר.
  • שילוב טוב עם מערכות Windows: מתאים במיוחד אם כבר יש לך מערכת שפותחה בסביבה של .NET.
  • ביצועים יציבים עם נוסחאות פשוטות: הפתרון יעיל מאוד עבור נוסחאות מתמטיות פשוטות.

חסרונות:
  • מגבלות עם נוסחאות מורכבות: ייתכן שלפעמים .NET לא יהיה גמיש מספיק בנוגע לנוסחאות מורכבות או תנאים דינמיים.
  • תלות במערכת Windows: הפתרון עשוי לא להיות אופטימלי עבור מערכות הפעלה אחרות אם זהו צורך בעתיד.

Python:
יתרונות:
  • גמישות גבוהה: Python מציעה יכולת כתיבה גמישה מאוד, במיוחד עם פונקציות כמו eval() או ספריות כמו ast.literal_eval().
  • קלות אינטגרציה עם ספריות נוספות: אם יש צורך בהרחבות נוספות (כמו חישובים מתקדמים או אינטגרציות עם APIs), Python מציעה מגוון רחב של ספריות חיצוניות.
  • יכולת התאמה אישית: Python מאפשרת שליטה מלאה על אופן החישוב והפרמטרים, במיוחד בשימוש עם קוד מותאם אישית.

חסרונות:
  • סיכון אבטחה: שימוש ב-eval() מסוכן במיוחד כאשר הקלט לא תמיד נשלט. חייבים לוודא שכל הקלט שנכנס הוא מהימן ומאומת.
  • ביצועים נמוכים עם כמויות נתונים גדולות: עבור מערכות בהן יש כמויות גדולות מאוד של נתונים, ביצועים עלולים להיות נמוכים יותר מאשר ב-.NET או בפרוצדורות SQL.

DB Procedure:
יתרונות:
  • ביצועים על כמויות נתונים גדולות: בפרוצדורות מאוחסנות (Stored Procedures) במסד הנתונים, החישוב נעשה קרוב מאוד למקור הנתונים, מה שמפחית את העומס על השרתים החיצוניים.
  • שמירה על ביצועים עקביים: חישובים בתוך מסד הנתונים מבטיחים עקביות וביצועים טובים במיוחד על נתונים גדולים.

חסרונות:
  • פחות גמיש עם נוסחאות מורכבות: שפת SQL פחות גמישה לעיתים בחישוב נוסחאות מתמטיות מורכבות.
  • תלות בבסיס הנתונים: כל שינוי בנוסחאות או בתנאים מחייב עדכון בפרוצדורה המאוחסנת, דבר שעשוי להקשות על תחזוקה והתאמה לצרכים משתנים.

המלצה כללית:
המערכת המומלצת ביותר לחישוב נוסחאות דינמיות היא {best_method}, שכן היא משיגה את החציון הנמוך ביותר וזמני ריצה יציבים עם מינימום mismatches.
"""

# עיבוד הטקסט כך שיהיה מוצג כראוי בעברית
analysis_text = fix_hebrew(analysis_text)
analysis_text = analysis_text.format(best_method=best_method)

# יצירת דו"ח PDF
generate_pdf_report(df_clean, stats_df, analysis_text)
