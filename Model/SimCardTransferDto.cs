public class SimCardTransferRequestDto
{
    public int CurrentEmployeeId { get; set; }
    public int TransferToEmployeeId { get; set; }
}
public class SimCardTransferDto
    {
        public int CurrentEmployeeId { get; set; }
        public int TransferToEmployeeId { get; set; }
        public List<int> SimCardIds { get; set; }
    }