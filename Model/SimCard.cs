namespace SimCardApi.Models
{
    public class SimCard
    {
        public int Id { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmployeeName { get; set; }
        public bool IsActive { get; set; }
    }
}