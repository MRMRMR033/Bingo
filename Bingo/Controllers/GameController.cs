using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bingo.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingo.Controllers
{
    public class GameModelsController : Controller
    {
        private readonly DataBaseContext _context;

        public GameModelsController(DataBaseContext context)
        {
            _context = context;
        }

        // GET: GameModels
        public async Task<IActionResult> Index()
        {
            var games = await _context.Games.Include(g => g.Cards).ToListAsync();
            return View(games);
        }
        // GET: GameModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameModel = await _context.Games
                .Include(g => g.Cards) // Incluir cartas relacionadas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameModel == null)
            {
                return NotFound();
            }

            return View(gameModel);
        }

        // GET: GameModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] GameModel gameModel)
        {
            if (ModelState.IsValid)
            {
                var datetime = DateTime.Now;
                var bingoCards = GenerateBingoCards(60);

                var game = new GameModel()
                {
                    StartAt = datetime,
                    EndAt = null,
                    IsActive = true
                };
                _context.Add(game);
                await _context.SaveChangesAsync();

                int gameId = game.Id;

                for (int i = 0; i < 60; i++)
                {
                    var cardString = ConvertCardToString(bingoCards[i]);

                    var card = new CardsModel
                    {
                        Folio = GenerateFolio(datetime),
                        GameId = gameId,
                        Numbers = cardString, // Guarda el string aquí
                        IsActive = true
                    };
                    _context.Cards.Add(card);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(gameModel);
        }

        // GET: GameModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameModel = await _context.Games.FindAsync(id);
            if (gameModel == null)
            {
                return NotFound();
            }
            return View(gameModel);
        }

        // POST: GameModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartAt,EndAt,IsActive")] GameModel gameModel)
        {
            if (id != gameModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameModelExists(gameModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gameModel);
        }

        // GET: GameModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameModel = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameModel == null)
            {
                return NotFound();
            }

            return View(gameModel);
        }

        // POST: GameModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameModel = await _context.Games.FindAsync(id);
            if (gameModel != null)
            {
                _context.Games.Remove(gameModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameModelExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }

        private static readonly Random random1 = new Random();

        static string GenerateFolio(DateTime dateTime)
        {
            string formattedDate = dateTime.ToString("yyyyMMddHHmm");
            var randomDigits = new StringBuilder(10);
            for (int i = 0; i < 10; i++)
            {
                randomDigits.Append(random1.Next(10)); // Corrección del rango de números
            }
            return formattedDate + randomDigits;
        }

        private static int[][] GenerateBingoCards(int numberOfCards)
        {
            int rowsPerCard = 3;
            int columnsPerCard = 9;
            var bingoCards = new int[numberOfCards][];

            Random rng = new Random();

            for (int cardIndex = 0; cardIndex < numberOfCards; cardIndex++)
            {
                var card = new int[rowsPerCard * columnsPerCard];
                var usedNumbers = new HashSet<int>(); // Para rastrear los números ya utilizados en la tarjeta

                for (int col = 0; col < columnsPerCard; col++)
                {
                    // Genera números únicos para la columna actual
                    var columnNumbers = Enumerable.Range(col * 10 + 1, 10)
                                                  .OrderBy(x => rng.Next())
                                                  .ToList();

                    int count = 0;
                    while (count < rowsPerCard)
                    {
                        // Selecciona un número aleatorio de la lista de números de la columna
                        int number = columnNumbers[rng.Next(columnNumbers.Count)];
                        if (!usedNumbers.Contains(number))
                        {
                            card[count * columnsPerCard + col] = number;
                            usedNumbers.Add(number);
                            count++;
                        }
                        // Si el número ya está en uso, selecciona uno nuevo
                    }

                    // Completa los espacios vacíos en la columna actual
                    for (int row = rowsPerCard; row < rowsPerCard; row++)
                    {
                        if (card[row * columnsPerCard + col] == 0)
                        {
                            card[row * columnsPerCard + col] = 0; // Espacios vacíos
                        }
                    }
                }

                bingoCards[cardIndex] = card;
            }

            return bingoCards;
        }

        private static string ConvertCardToString(int[] card)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < card.Length; i++)
            {
                sb.Append(card[i]);
                if (i < card.Length - 1) sb.Append(',');
            }

            return sb.ToString();
        }

        private static int[] ConvertStringToCard(string cardString)
        {
            return cardString.Split(',')
                              .Select(int.Parse)
                              .ToArray();
        }
    }
}
