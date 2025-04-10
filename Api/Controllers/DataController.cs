﻿using Ardalis.Result.AspNetCore;
using Ardalis.Result;
using Coravel.Cache.Interfaces;
using Marten;
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

        [TranslateResultToActionResult]
        [HttpPost("employer")]
        public async Task<Result<Models.v1.EmploymentResponse>> AddEmployer(Models.v1.EmploymentRequest request)
        {
            try
            {
                var address = new Core.Models.Employment(
                        Guid.NewGuid(),
                        request.ConsumerId,
                        request.Name,
                        request.Designation,
                        request.StartDate.ToString(),
                        request.TerminationDate.ToString(),
                        request.EmploymentTypeId.ToString(),
                        request.RecordDate
                    );

                await using var session = _store.LightweightSession();
                session.Store(address);
                await session.SaveChangesAsync();

                return Result<Models.v1.EmploymentResponse>.Success(new Models.v1.EmploymentResponse
                {
                    Id = address.Id,
                    ConsumerId = address.ConsumerId,
                    Name = address.Name,
                    Designation = address.Designation,
                    StartDate = DateTime.Parse(address.StartDate),
                    TerminationDate = DateTime.Parse(address.TerminationDate),
                    EmploymentTypeId = Int32.Parse(address.EmploymentTypeId),
                    RecordDate = address.RecordDate
                });
            }
            catch (Exception Ex)
            {
                return Result<Models.v1.EmploymentResponse>.Error(Ex.Message);
            }
        }

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
                        Name = x.Name,
                        Designation = x.Designation,
                        StartDate = DateTime.Parse(x.StartDate),
                        TerminationDate = DateTime.Parse(x.TerminationDate),
                        EmploymentTypeId = Int32.Parse(x.EmploymentTypeId),
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