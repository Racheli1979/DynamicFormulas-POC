# DynamicFormulas - Proof of Concept (POC)

## תיאור הפרויקט

פרויקט זה מבצע **Proof of Concept (POC)** להשוואת ביצועים של שלוש שיטות שונות לחישוב נוסחאות מתמטיות דינמיות:
- **DB Procedure (SQL)** – חישוב נוסחאות באמצעות Stored Procedure במסד הנתונים.
- **.NET** – חישוב נוסחאות באמצעות טכנולוגיות .NET.
- **Python** – חישוב נוסחאות באמצעות שפת Python.

הפרויקט בודק איזו שיטה היא היעילה ביותר לחישוב נוסחאות דינמיות על מיליון רשומות נתונים. כל שיטה מתמודדת עם נוסחאות שונות, כולל נוסחאות עם תנאים מתמטיים.

> ⚠️ **חשוב:** שלב ה-SQL **חייב להתבצע ראשון**, מאחר והוא יוצר את בסיס הנתונים והטבלאות שעליהם פרויקטי Python ו-.NET נשענים.

---

## דרישות מערכת

### SQL Server
- SQL Server או SQL Server Express
- הרשאות ליצירת Database, Tables ו-Stored Procedures

### .NET
- .NET 8.0 ומעלה
- חבילות: DotNetEnv, Microsoft.Data.SqlClient, NCalc, System.Data.Odbc

### Python
- Python 3.8 ומעלה
- חבילות: pandas, numpy, matplotlib, seaborn, python-dotenv, sqlalchemy, fpdf, arabic-reshaper, bidi

---

## מבנה הנתונים

### טבלת נתונים (`tmp_data`)
| עמודה  | סוג   | הערות           |
|--------|-------|-----------------|
| id     | INT   | Primary Key     |
| a-h    | FLOAT | שדות מספריים עם נתונים רנדומליים |

### טבלת נוסחאות (`tmp_targil`)
| עמודה  | סוג   | הערות           |
|--------|-------|-----------------|
| id     | INT   | Primary Key     |
| targil | VARCHAR  | נוסחה דינמית    |
| tnai   | VARCHAR  | אופציונלי – תנאי לביצוע הנוסחה |

### טבלת תוצאות (`tmp_results`)
| עמודה        | סוג   | הערות                              |
|--------------|-------|------------------------------------|
| id           | INT   | Primary Key                        |
| data_id      | INT   | Foreign Key ל-`tmp_data.id`        |
| targil_id    | INT   | Foreign Key ל-`tmp_targil.id`      |
| method       | VARCHAR | שם השיטה (.NET, Python, DB Procedure) |
| result       | FLOAT | תוצאה                              |
| execution_time | FLOAT | זמן ריצה בשניות                  |

---

## הרצת הפרויקט
## שלב 1 – SQL (חובה להריץ ראשון)

### יצירת בסיס נתונים
```sql
CREATE DATABASE DynamicFormulasDB;
USE DynamicFormulasDB;
```

### יצירת טבלאות
יש ליצור את הטבלאות הבאות במסד הנתונים:
tmp_data, tmp_targil, tmp_results

הרץ כאן את הסקריפטים ליצירת הטבלאות ומילוי נתונים 

(קבצים CreateTables.sql, InsertData.sql, InsertFormulas.sql)

### יצירה והרצה של Stored Procedure
הרץ את קובץ Procedure.sql בSql Server

> התוצאות נכתבות לטבלת tmp_results.
> שלב זה מכין את כל התשתית שעליה פרויקטי Python ו-.NET עובדים.

## שלב 2 – Python

### התקנת הספריות הדרושות
```bash
pip install -r requirements.txt
```

### הגדרת משתני סביבה (.env)
צור קובץ env. והעתק לשם את הקוד הזה ושנה את DB_SERVER ו DB_DRIVER

```bash
DB_SERVER=<your_server_name>
DB_DATABASE=DynamicFormulasDB
DB_DRIVER=<your_driver_name>
DB_TABLE_DATA=tmp_data
DB_TABLE_FORMULAS=tmp_targil
DB_TABLE_RESULTS=tmp_results
```

### הרצה
יש לקחת את הקוד מתוך תיקיית Python ולהריץ את קובץ Main.py.

```bash
python Main.py
```

> גם כאן התוצאות נכתבות לטבלת tmp_results.

> ⚠️ **אבטחה:** הקוד ב-Python משתמש ב-eval(), לכן יש להיזהר לא להריץ נוסחאות שמגיעות ממקורות לא מהימנים.

## שלב 3 – NET.

### הורדת הספריות הנדרשות
```bash
dotnet restore
```

### הרצה
יש לקחת את הקוד מתוך תיקיית .Net ולהריץ:

```bash
dotnet run
```

> גם כאן התוצאות נכתבות לטבלת tmp_results.

## שלב 4 - ניתוח תוצאות

לאחר הרצת שלושת המימושים (**SQL**, **.NET**, **Python**) ניתן לנתח את הפרמטרים הבאים:

- **זמני ריצה**
- **תוצאות חישוב**
- **אי־התאמות (mismatches)**

כל הנתונים נשמרים בטבלה:

`tmp_results`

### יצירת דו"ח

- **גרפי ביצועים**  
- **סטטיסטיקות**: mean, median, std  
- **דו״ח PDF מסכם**  
- **המלצה על השיטה היעילה ביותר**

### הגדרת משתני סביבה (.env)
צור קובץ env. והעתק לשם את הקוד הזה ושנה את DB_SERVER ו DB_DRIVER

```bash
DB_SERVER=<your_server_name>
DB_DATABASE=DynamicFormulasDB
DB_DRIVER=<your_driver_name>
DB_TABLE_RESULTS=tmp_results
```

### הרצה
יש לקחת את הקוד מתוך תיקיית Performance Comparison ולהריץ את קובץ Main.py.

```bash
python Main.py
```

## הערות חשובות

- תיקיית `fonts` חייבת להיות בתוך תיקיית הפרויקט.
- כל הדוחות, הגרפים וה־PDF נוצרות תחת תיקיית הפרויקט שבה מריצים את הקוד.

