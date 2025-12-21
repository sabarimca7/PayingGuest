public class CurrentBookingDetailsDto
{
    public string PropertyName { get; set; } = string.Empty;
    public DateTime MoveInDate { get; set; }
    public int ContractMonths { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal SecurityDeposit { get; set; }
    public bool IsDepositPaid { get; set; }
    public string Address { get; set; } = string.Empty;
}
