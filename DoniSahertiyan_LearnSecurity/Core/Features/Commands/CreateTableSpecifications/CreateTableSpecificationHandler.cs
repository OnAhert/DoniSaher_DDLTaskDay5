using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.Models;
using Persistence.Repositories;
using System.Text.Json;

namespace Core.Features.Commands.CreateTableSpecification
{
    public class CreateTableSpecificationHandler : IRequestHandler<CreateTableSpecificationCommand, CreateTableSpecificationResponse>
    {
        private readonly ITableSpecificationRepository _tableSpecificationRepository;
        private readonly IDistributedCache _cache;
        private const string CacheKeyPrefix = "TableSpecification_";
        private const int CacheTTLMinutes = 10;

        public CreateTableSpecificationHandler(ITableSpecificationRepository tableSpecificationRepository, IDistributedCache cache)
        {
            _tableSpecificationRepository = tableSpecificationRepository;
            _cache = cache;
        }

        public async Task<CreateTableSpecificationResponse> Handle(CreateTableSpecificationCommand command, CancellationToken cancellationToken)
        {
            var tableSpecification = new TableSpecification
            {
                TableId = Guid.NewGuid(),
                TableNumber = command.TableNumber,
                ChairNumber = command.ChairNumber,
                TablePic = command.TablePic,
                TableType = command.TableType
            };

            await _tableSpecificationRepository.AddAsync(tableSpecification);
            await _tableSpecificationRepository.SaveChangesAsync();

            var response = new CreateTableSpecificationResponse
            {
                TableId = tableSpecification.TableId,
                TableNumber = tableSpecification.TableNumber,
                ChairNumber = tableSpecification.ChairNumber,
                TablePic = tableSpecification.TablePic,
                TableType = tableSpecification.TableType,
            };

            string cacheKey = $"{CacheKeyPrefix}{response.TableId}";
            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheTTLMinutes)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), cacheOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error: {ex.Message}");
            }

            return response;
        }
    }
}
