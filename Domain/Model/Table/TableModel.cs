using Infrastructure.Crosscutting.Enums;

namespace Domain.Model.Table
{
    public sealed class TableModel
    {
        public Guid Owner { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string SystemGame { get; set; } = string.Empty;
        public List<Genre>? Genres { get; set; }
        
    }
}