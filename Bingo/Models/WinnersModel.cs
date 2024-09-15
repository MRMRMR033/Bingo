namespace Bingo.Models
{
    public class WinnersModel
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int CardId { get; set; }
        public int PremioId { get; set; }
        public bool IsActve {  get; set; }
    }
}
