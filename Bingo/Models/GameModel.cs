using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bingo.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        [DisplayName("Creacion de Juego:")]
        public DateTime StartAt { get; set; }
        [DisplayName("Juego Finalizado:")]
        public DateTime? EndAt { get; set; }
        public bool IsActive { get; set; }
        public string DrawnNumbers { get; set; } // Numeros sorteados
        public ICollection<CardsModel> Cards { get; set; } // Relacion uno a muchos con CardsModel
    }
}
