using Persistence.Models;

namespace Persistence.Repositories;

public interface ITableSpecificationRepository : IGenericRepository<TableSpecification>
{
    Task AddAsync(TableSpecification tableSpecification);
    Task SaveChangesAsync();
    Task<TableSpecification> GetByIdAsync(Guid id);
    Task Remove(TableSpecification tableSpecification);
}