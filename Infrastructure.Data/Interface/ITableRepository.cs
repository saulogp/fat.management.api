using Infrastructure.Data.Entity;

namespace Infrastructure.Data.Interface
{
    public interface ITableRepository
    {
        Task<TableEntity> GetTableAsync(Guid id);
        Task<List<TableEntity>> GetTableByUserIdAsync(Guid id);
        Task CreateAsync(TableEntity table);
        Task UpdateAsync(Guid id, TableEntity table);
        Task RemoveAsync(Guid id, TableEntity table);
    }
}