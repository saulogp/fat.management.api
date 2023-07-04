using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Data.Repository
{
    public class TableRepository : ITableRepository
    {
        private readonly IMongoCollection<TableEntity> _tableCollection;

        public TableRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _tableCollection = mongoDatabase.GetCollection<TableEntity>(
                databaseSettings.Value.TableCollectionName);
        }

        public async Task<TableEntity> GetTableAsync(Guid id) =>
            await _tableCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(TableEntity table) => 
            await _tableCollection.InsertOneAsync(table);

        public async Task UpdateAsync(Guid id, TableEntity table) =>
            await _tableCollection.ReplaceOneAsync(x => x.Id == id, table);

        public async Task RemoveAsync(Guid id, TableEntity table) {
            table.Active = false;
            await _tableCollection.ReplaceOneAsync(x => x.Id == id, table);
        }
        public async Task<List<TableEntity>> GetTableByUserIdAsync(Guid id) => 
            await _tableCollection.Find(x => x.Owner == id && x.Active).ToListAsync();
    }
}