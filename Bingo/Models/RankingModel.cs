namespace Bingo.Models
{
    public class RankingModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Order { get; set; }
        public int GameId { get; set; }
        public bool IsActive {  get; set; } 
    }
}
