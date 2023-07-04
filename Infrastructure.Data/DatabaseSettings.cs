namespace Infrastructure.Data
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UserProfileCollectionName { get; set; } = null!;

        public string TableCollectionName { get; set; } = null!;
    }
}