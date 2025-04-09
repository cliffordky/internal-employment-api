namespace Api.Models.v1
{
    public class EmploymentRequest
    {
        public Guid ConsumerId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public int EmploymentTypeId { get; set; }
        public DateTimeOffset RecordDate { get; set; }
    }
}
