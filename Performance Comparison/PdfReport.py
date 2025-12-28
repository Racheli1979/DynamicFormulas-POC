from fpdf import FPDF
from FixHebrew import fix_hebrew

def generate_pdf_report(df_clean, stats_df, analysis_text):
    pdf = FPDF()
    pdf.add_page()

    pdf.add_font('DejaVu', '', r'fonts\DejaVuSans.ttf', uni=True)
    pdf.add_font('DejaVu', 'B', r'fonts\DejaVuSans-Bold.ttf', uni=True)

    pdf.set_font("DejaVu", 'B', 16)
    pdf.cell(0, 10, fix_hebrew("דו\"ח POC - ניתוח ביצועים"), ln=True, align='C')

    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew("מבנה הנתונים (5 שורות ראשונות)"), ln=True, align='C')
    pdf.set_font("DejaVu", '', 10)
    for index, row in df_clean.head().iterrows():
        row_text = " | ".join([str(v) for v in row.values])
        pdf.cell(0, 6, fix_hebrew(row_text), ln=True, border=1, align='C')

    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew("סטטיסטיקות עבור כל שיטה"), ln=True, align='C')
    pdf.set_font("DejaVu", '', 10)
    pdf.multi_cell(0, 5, fix_hebrew(stats_df.to_string(index=False)), align='C')

    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew('השוואת זמן ריצה ממוצע עם סטיית תקן - גרף משולב'), ln=True, align='C')
    pdf.image("combined_graph.png", w=180)

    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew("גרף זמן ריצה "), ln=True, align='C')
    pdf.image(".Net.png", w=150)
    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew("גרף זמן ריצה "), ln=True, align='C')
    pdf.image("Python.png", w=150)
    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew("גרף זמן ריצה "), ln=True, align='C')
    pdf.image("DB_Procedure.png", w=150)

    pdf.set_font("DejaVu", 'B', 12)
    pdf.cell(0, 10, fix_hebrew("ניתוח והמלצה"), ln=True, align='C')
    pdf.set_font("DejaVu", '', 10)
    pdf.multi_cell(0, 5, analysis_text, align='C')

    pdf.output("POC_Report_Final.pdf")
    print(fix_hebrew("דו\"ח PDF סופי נוצר בהצלחה בשם POC_Report_Final.pdf"))
