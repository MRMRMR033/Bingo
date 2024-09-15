using System;
using System.Collections.Generic;

namespace Bingo.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool IsActive { get; set; }
        public string DrawnNumbers { get; set; } // Numeros sorteados
        public ICollection<CardsModel> Cards { get; set; } // Relacion uno a muchos con CardsModel
    }
}
