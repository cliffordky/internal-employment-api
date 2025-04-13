namespace Api.Models.v1
{
    public class EmploymentResponse
    {
        public Guid Id { get; set; }
        public Guid ConsumerId { get; set; }
        public Guid SubscriberId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public string EmploymentTypeCode { get; set; }
        public DateTimeOffset RecordDate { get; set; }
    }
}