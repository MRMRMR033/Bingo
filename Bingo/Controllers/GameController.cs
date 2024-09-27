 using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bingo.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            // Incluir los cartones relacionados con el juego
            var gameModel = await _context.Games
                .Include(g => g.Cards)  // Carga los cartones relacionados
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
        public async Task<IActionResult> Create(GameModel gameModel)
        {
            if (ModelState.IsValid)
            {
                var datetime = DateTime.Now;
                var bingoCards = GenerateBingoCards(200); // Genera 200 cartones

                var game = new GameModel
                {
                    StartAt = datetime,
                    EndAt = null,
                    IsActive = true
                };

                _context.Add(game);
                await _context.SaveChangesAsync();

                int gameId = game.Id;

                for (int i = 0; i < 200; i++)
                {
                    var rowOne = string.Join(",", bingoCards[i][0]);
                    var rowTwo = string.Join(",", bingoCards[i][1]);
                    var rowThree = string.Join(",", bingoCards[i][2]);

                    var card = new CardsModel
                    {
                        Folio = GenerateFolio(datetime),
                        GameId = gameId,
                        NumbersRowOne = rowOne,
                        NumbersRowTwo = rowTwo,
                        NumbersRowTree = rowThree,
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
                randomDigits.Append(random1.Next(10));
            }
            return formattedDate + randomDigits;
        }

        // Lógica para generar los cartones de Bingo 90 con 3 filas y 9 columnas
        private static int[][][] GenerateBingoCards(int numberOfCards)
        {
            int rowsPerCard = 3;
            int columnsPerCard = 9;
            var bingoCards = new int[numberOfCards][][];

            Random rng = new Random();

            for (int cardIndex = 0; cardIndex < numberOfCards; cardIndex++)
            {
                var card = new int[rowsPerCard][];
                for (int row = 0; row < rowsPerCard; row++)
                {
                    card[row] = new int[columnsPerCard];
                }

                var usedNumbers = new HashSet<int>();

                // Rango de números para cada columna
                for (int col = 0; col < columnsPerCard; col++)
                {
                    var min = col == 0 ? 1 : col * 10;
                    var max = col == 8 ? 90 : (col * 10) + 9;

                    var columnNumbers = Enumerable.Range(min, max - min + 1).OrderBy(x => rng.Next()).Take(3).ToList();

                    for (int row = 0; row < rowsPerCard; row++)
                    {
                        if (columnNumbers.Count > 0)
                        {
                            card[row][col] = columnNumbers[0];
                            columnNumbers.RemoveAt(0);
                        }
                    }
                }

                // Asegurarse de que cada fila tiene exactamente 5 números
                for (int row = 0; row < rowsPerCard; row++)
                {
                    var numbersInRow = card[row].Where(n => n != 0).ToList();
                    while (numbersInRow.Count > 5)
                    {
                        numbersInRow.RemoveAt(rng.Next(numbersInRow.Count));
                    }
                    for (int col = 0; col < columnsPerCard; col++)
                    {
                        card[row][col] = numbersInRow.Contains(card[row][col]) ? card[row][col] : 0;
                    }
                }

                bingoCards[cardIndex] = card;
            }

            return bingoCards;
        }
    }
}
