using System.ComponentModel.DataAnnotations.Schema;

namespace Bingo.Models
{
    public class CardsModel
    {
        public int Id { get; set; }
        public string Folio { get; set; }

        // Clave foránea para la relación con GameModel
        public int GameId { get; set; }

        // Navegación a la entidad relacionada (GameModel)
        [ForeignKey("GameId")]
        public GameModel Game { get; set; }

        public string NumbersRowOne { get; set; }
        public string NumbersRowTwo { get; set; }
        public string NumbersRowTree { get; set; }
        public bool IsActive { get; set; }
    }
}
