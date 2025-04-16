using Core.Encryption;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public record Employment(Guid Id, Guid ConsumerId, Guid SubscriberId, string Name, string Designation, string StartDate, string? TerminationDate, string EmploymentTypeCode, string ISOA3CountryCode, DateTimeOffset RecordDate, string Hash)
        : IHasEncryptionKey
    {
        [JsonIgnore]
        public string EncryptionKey => Id.ToString();
    }
}