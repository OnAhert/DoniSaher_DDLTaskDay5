using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.Repositories;

namespace Core.Features.Commands.DeleteTableSpecification
{
    public class DeleteTableSpecificationHandler : IRequestHandler<DeleteTableSpecificationCommand, DeleteTableSpecificationResponse>
    {
        private readonly ITableSpecificationRepository _tableSpecificationRepository;
        private readonly IDistributedCache _cache;
        private const string CacheKeyPrefix = "TableSpecification_";

        public DeleteTableSpecificationHandler(ITableSpecificationRepository tableSpecificationRepository, IDistributedCache cache)
        {
            _tableSpecificationRepository = tableSpecificationRepository;
            _cache = cache;
        }

        public async Task<DeleteTableSpecificationResponse> Handle(DeleteTableSpecificationCommand command, CancellationToken cancellationToken)
        {
            var tableSpecification = await _tableSpecificationRepository.GetByIdAsync(command.TableId);

            if (tableSpecification == null)
            {
                return new DeleteTableSpecificationResponse
                {
                    Success = false,
                    Message = "Table specification not found."
                };
            }

            _tableSpecificationRepository.Remove(tableSpecification);
            await _tableSpecificationRepository.SaveChangesAsync();

            string cacheKey = $"{CacheKeyPrefix}{command.TableId}";
            try
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error: {ex.Message}");
            }

            return new DeleteTableSpecificationResponse
            {
                Success = true,
                Message = "Table specification deleted successfully."
            };
        }
    }
}
