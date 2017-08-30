namespace DoEko.Models.Payroll
{
    public interface IWageType
    {
        double Amount { get; set; }
        string Code { get; set; }
        string Currency { get; set; }
        double Number { get; set; }
        double Rate { get; set; }
        string ShortDescription { get; set; }
        WageTypeUnit Unit { get; set; }
    }
}