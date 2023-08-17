using Infrastructure.Crosscutting.Enums;

namespace Domain.Model.Table
{
    public sealed class TableResponseModel
    {
        public Participant Owner { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string SystemGame { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool Active { get; set; }
        public List<Genre>? Genres { get; set; }
        public List<Participant> Participants { get; set; }
    }

    public class Participant
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
    }
}