using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BudgetTracker.Web.Models;
using System.IO;

namespace BudgetTracker.Web.Services
{
    public class MovimentiXmlService
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "movimenti.xml");

        public List<Movimento> GetAll()
        {
            if (!File.Exists(_filePath))
                return new List<Movimento>();

            var doc = XDocument.Load(_filePath);
            return doc.Root.Elements("Movimento").Select(x => new Movimento
            {
                Id = (int)x.Element("Id"),
                Descrizione = (string)x.Element("Descrizione"),
                Importo = (decimal)Convert.ToDouble((string)x.Element("Importo")),
                Entrata = (bool)x.Element("Entrata")
            }).ToList();
        }

        public void Add(Movimento nuovo)
        {
            var movimenti = GetAll();
            nuovo.Id = movimenti.Count > 0 ? movimenti.Max(m => m.Id) + 1 : 1;
            movimenti.Add(nuovo);
            SaveAll(movimenti);
        }

        private void SaveAll(List<Movimento> movimenti)
        {
            var doc = new XDocument(
                new XElement("Movimenti",
                    movimenti.Select(m =>
                        new XElement("Movimento",
                            new XElement("Id", m.Id),
                            new XElement("Descrizione", m.Descrizione),
                            new XElement("Importo", m.Importo),
                            new XElement("Entrata", m.Entrata)
                        )
                    )
                )
            );
            doc.Save(_filePath);
        }
    }
}
