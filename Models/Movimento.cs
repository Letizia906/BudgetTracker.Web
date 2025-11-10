using System;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Web.Models
{
    public class Movimento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La data è obbligatoria")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        public string Descrizione { get; set; }

        [Required(ErrorMessage = "L'importo è obbligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Inserisci un importo valido")]
        public decimal Importo { get; set; }

        public bool Entrata { get; set; } // true = Entrata, false = Uscita

        // Questi campi ora NON sono obbligatori
        public string? Categoria { get; set; }
        public string? Note { get; set; }
        public string? Tipo { get; set; }
    }
}
