namespace Api.Models.v1
{
    public class EmploymentResponse
    {
        public Guid Id { get; set; }
        public Guid ConsumerId { get; set; }
        public Guid SubscriberId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? TerminationDate { get; set; }
        public string EmploymentTypeCode { get; set; }

        public string ISOA3CountryCode { get; set; }
        public DateTimeOffset RecordDate { get; set; }
    }
}