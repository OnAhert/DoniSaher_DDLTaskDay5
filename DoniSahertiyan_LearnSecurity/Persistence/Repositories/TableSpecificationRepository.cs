using Persistence.DatabaseContext;
using Persistence.Models;

namespace Persistence.Repositories;

public class TableSpecificationRepository : GenericRepository<TableSpecification>, ITableSpecificationRepository
{
    private readonly TableContext _context;

    public TableSpecificationRepository(TableContext context) : base(context)
    {
        _context = context;
    }
    public async Task<TableSpecification> GetByIdAsync(Guid id)
    {
        return await _context.TableSpecifications.FindAsync(id);
    }
    public async Task AddAsync(TableSpecification tableSpecification)
    {
        await _context.TableSpecifications.AddAsync(tableSpecification);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task Remove(TableSpecification tableSpecification)
    {
        _context.TableSpecifications.Remove(tableSpecification);
    }
}