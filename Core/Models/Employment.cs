using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Models
{
    public record Employment(Guid Id, Guid ConsumerId, string Name, string Designation, string StartDate, string TerminationDate, string EmploymentTypeId, DateTimeOffset RecordDate)
        : IHasEncryptionKey
    {
        [JsonIgnore]
        public string EncryptionKey => Id.ToString();
    }
}
