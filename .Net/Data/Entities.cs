namespace FormulaCalculatorEF.Data
{
    public class DataRow
    {
        public int id { get; set; }
        public double a { get; set; }
        public double b { get; set; }
        public double c { get; set; }
        public double d { get; set; }
        public double e { get; set; }
        public double f { get; set; }
        public double g { get; set; }
        public double h { get; set; }
    }

    public class Formula
    {
        public int id { get; set; }
        public string targil { get; set; }
        public string? tnai { get; set; }
    }

    public class Result
    {
        public int id { get; set; }
        public int data_id { get; set; }
        public int targil_id { get; set; }
        public string method { get; set; }
        public double? result { get; set; }
        public double execution_time { get; set; }
    }
}
