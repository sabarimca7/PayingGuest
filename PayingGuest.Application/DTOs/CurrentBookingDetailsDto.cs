// Application/DTOs/CurrentBookingDto.cs
public class CurrentBookingDetailsDto
{
    public string PropertyName { get; set; }
    public string RoomNo { get; set; }
    public string RoomType { get; set; }
    public int Floor { get; set; }

    public DateTime MoveInDate { get; set; }
    public int ContractMonths { get; set; }

    public decimal MonthlyRent { get; set; }
    public decimal SecurityDeposit { get; set; }

    public string Address { get; set; }
    public string OwnerContact { get; set; }
    public bool IsDepositPaid { get; set; }
}
