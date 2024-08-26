using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.Repositories;
using System.Text.Json;

namespace Core.Features.Queries.GetTableSpecifications
{
    public class GetTableSpecificationsHandler : IRequestHandler<GetTableSpecificationsQuery, GetTableSpecificationsResponse>
    {
        private readonly ITableSpecificationRepository _tableSpecificationRepository;
        private readonly IDistributedCache _cache;
        private const string CacheKeyPrefix = "TableSpecification_";
        private const int CacheTTLMinutes = 10;

        public GetTableSpecificationsHandler(ITableSpecificationRepository tableSpecificationRepository, IDistributedCache cache)
        {
            _tableSpecificationRepository = tableSpecificationRepository;
            _cache = cache;
        }

        public async Task<GetTableSpecificationsResponse> Handle(GetTableSpecificationsQuery query, CancellationToken cancellationToken)
        {
            GetTableSpecificationsResponse response = null;
            string cacheKey = $"{CacheKeyPrefix}{query.TableSpecificationId}";

            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (cachedData != null)
                {
                    response = JsonSerializer.Deserialize<GetTableSpecificationsResponse>(cachedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error: {ex.Message}");
            }

            if (response == null)
            {
                var tableSpecification = await _tableSpecificationRepository.GetByIdAsync(query.TableSpecificationId);

                if (tableSpecification != null)
                {
                    response = new GetTableSpecificationsResponse
                    {
                        TableId = tableSpecification.TableId,
                        ChairNumber = tableSpecification.ChairNumber,
                        TableNumber = tableSpecification.TableNumber,
                        TablePic = tableSpecification.TablePic,
                        TableType = tableSpecification.TableType
                    };

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
                }
            }

            return response;
        }
    }
}
