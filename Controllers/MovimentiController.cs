using Microsoft.AspNetCore.Mvc;
using BudgetTracker.Web.Models;
using BudgetTracker.Web.Services; // ✅ per usare ExchangeService
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading.Tasks; // ✅ per async/await

namespace BudgetTracker.Web.Controllers
{
    public class MovimentiController : Controller
    {
        private readonly string xmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "movimenti.xml");
        private readonly ExchangeService _exchangeService;

        // ✅ Iniettiamo o creiamo il servizio per i tassi di cambio
        public MovimentiController()
        {
            _exchangeService = new ExchangeService();
        }

        public IActionResult Index()
        {
            var movimenti = CaricaMovimenti();
            return View(movimenti);
        }

        // GET: Movimenti/Create
        public IActionResult Create()
        {
            return View(new Movimento { Data = DateTime.Now });
        }

        // POST: Movimenti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movimento movimento)
        {
            if (!ModelState.IsValid)
                return View(movimento);

            var movimenti = CaricaMovimenti();
            movimento.Id = movimenti.Any() ? movimenti.Max(m => m.Id) + 1 : 1;
            movimenti.Add(movimento);
            SalvaMovimenti(movimenti);

            return RedirectToAction(nameof(Index));
        }

        // ✅ DELETE
        public IActionResult Elimina(int id)
        {
            var movimenti = CaricaMovimenti();
            var movimento = movimenti.FirstOrDefault(m => m.Id == id);
            if (movimento != null)
            {
                movimenti.Remove(movimento);
                SalvaMovimenti(movimenti);
            }
            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: Movimenti/Converti — mostra la pagina del form
        [HttpGet]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Converti(decimal importo, string valuta)
        {
            // ✅ Ricarica sempre la lista di valute (così il menu rimane visibile anche dopo la conversione)
            ViewBag.Valute = await _exchangeService.GetSupportedCurrenciesAsync();

            if (importo <= 0 || string.IsNullOrEmpty(valuta))
            {
                ViewBag.Errore = "Inserisci un importo valido e seleziona una valuta.";
                return View();
            }

            try
            {
                var tasso = await _exchangeService.GetExchangeRateAsync("EUR", valuta);
                var risultato = importo * tasso;

                // ✅ Mostra il risultato formattato
                ViewBag.Risultato = $"{importo} EUR = {risultato:F2} {valuta.ToUpper()} (tasso: {tasso})";
            }
            catch (Exception ex)
            {
                ViewBag.Errore = $"Errore nella conversione: {ex.Message}";
            }

            return View();
        }


        // ---------------- METODI DI SUPPORTO ----------------

        private List<Movimento> CaricaMovimenti()
        {
            if (!System.IO.File.Exists(xmlPath))
                return new List<Movimento>();

            var doc = XDocument.Load(xmlPath);
            return doc.Descendants("Movimento")
                .Select(x => new Movimento
                {
                    Id = (int)x.Element("Id"),
                    Data = DateTime.Parse(x.Element("Data")?.Value ?? DateTime.Now.ToString()),
                    Descrizione = (string)x.Element("Descrizione"),
                    Importo = decimal.Parse(x.Element("Importo")?.Value ?? "0"),
                    Entrata = bool.Parse(x.Element("Entrata")?.Value ?? "true"),
                    Categoria = (string?)x.Element("Categoria"),
                    Note = (string?)x.Element("Note"),
                    Tipo = (string?)x.Element("Tipo")
                })
                .ToList();
        }

        private void SalvaMovimenti(List<Movimento> movimenti)
        {
            var doc = new XDocument(
                new XElement("Movimenti",
                    movimenti.Select(m =>
                        new XElement("Movimento",
                            new XElement("Id", m.Id),
                            new XElement("Data", m.Data.ToString("s")),
                            new XElement("Descrizione", m.Descrizione),
                            new XElement("Importo", m.Importo),
                            new XElement("Entrata", m.Entrata),
                            new XElement("Categoria", m.Categoria ?? ""),
                            new XElement("Note", m.Note ?? ""),
                            new XElement("Tipo", m.Tipo ?? "")
                        )
                    )
                )
            );
            doc.Save(xmlPath);
        }
    }
}
