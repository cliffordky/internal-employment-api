using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Coravel.Cache.Interfaces;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly IDocumentStore _store;
        private readonly ICache _cache;

        public DataController(ILogger<DataController> logger, IDocumentStore store, ICache cache)
        {
            _logger = logger;
            _store = store;
            _cache = cache;
        }

        //[Authorize("write")]
        [Authorize]
        [TranslateResultToActionResult]
        [HttpPost("employer")]
        public async Task<Result<Models.v1.EmploymentResponse>> AddEmployer(Models.v1.EmploymentRequest request)
        {
            try
            {
                string hash = Core.Encryption.Hash.GetHashString(request.ConsumerId.ToString() +
                    request.SubscriberId.ToString() +
                    request.Name +
                    request.Designation +
                    request.StartDate.ToString() +
                    request.TerminationDate.ToString() +
                    request.EmploymentTypeCode +
                     request.ISOA3CountryCode +
                    request.RecordDate);

                await using var session = _store.LightweightSession();
                var existing = await session.Query<Core.Models.Employment>().SingleOrDefaultAsync(x => x.Hash == hash);
                if (existing != null)
                {
                    return Result<Models.v1.EmploymentResponse>.Error("Employment already exists");
                }

                var address = new Core.Models.Employment(
                        Guid.NewGuid(),
                        request.ConsumerId,
                        request.SubscriberId,
                        request.Name,
                        request.Designation,
                        request.StartDate.ToString(),
                        request.TerminationDate?.ToString(),
                        request.EmploymentTypeCode,
                        request.ISOA3CountryCode,
                        request.RecordDate,
                        hash
                    );

                session.Store(address);
                await session.SaveChangesAsync();

                return Result<Models.v1.EmploymentResponse>.Success(new Models.v1.EmploymentResponse
                {
                    Id = address.Id,
                    ConsumerId = address.ConsumerId,
                    SubscriberId = address.SubscriberId,
                    Name = address.Name,
                    Designation = address.Designation,
                    StartDate = DateOnly.Parse(address.StartDate),
                    TerminationDate = DateOnly.TryParse(address.TerminationDate, out DateOnly result) ? result : null,
                    EmploymentTypeCode = address.EmploymentTypeCode,
                    ISOA3CountryCode = address.ISOA3CountryCode,
                    RecordDate = address.RecordDate
                });
            }
            catch (Exception Ex)
            {
                return Result<Models.v1.EmploymentResponse>.Error(Ex.Message);
            }
        }

        //[Authorize("read")]
        [Authorize]
        [TranslateResultToActionResult]
        [HttpGet("employers")]
        public async Task<Result<List<Models.v1.EmploymentResponse>>> GetEmployersForConsumer(Guid ConsumerId)
        {
            try
            {
                await using var session = _store.LightweightSession();
                var addresses = await session.Query<Core.Models.Employment>().Where(x => x.ConsumerId == ConsumerId).ToListAsync();

                return Result<List<Models.v1.EmploymentResponse>>.Success(addresses.Select(
                    x => new Models.v1.EmploymentResponse
                    {
                        Id = x.Id,
                        ConsumerId = x.ConsumerId,
                        SubscriberId = x.SubscriberId,
                        Name = x.Name,
                        Designation = x.Designation,
                        StartDate = DateOnly.TryParse(x.StartDate, out DateOnly sd) ? sd : null,
                        TerminationDate = DateOnly.TryParse(x.TerminationDate, out DateOnly td) ? td : null,
                        EmploymentTypeCode = x.EmploymentTypeCode,
                        ISOA3CountryCode = x.ISOA3CountryCode,
                        RecordDate = x.RecordDate
                    }).ToList());
            }
            catch (Exception Ex)
            {
                return Result<List<Models.v1.EmploymentResponse>>.Error(Ex.Message);
            }
        }
    }
}