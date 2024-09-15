using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bingo.Models
{
    public class CardsModel
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public int GameId { get; set; } // Clave foránea para la relación con GameModel
        public string Numbers { get; set; } // este string va a guardar los valores de la matriz
        public bool IsActive { get; set; }

    }
}
