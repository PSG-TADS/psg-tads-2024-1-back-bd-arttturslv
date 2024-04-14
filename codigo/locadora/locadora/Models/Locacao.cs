using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace locadora.Models
{
    public class Locacao
    {
        [Key]
        public string? LocacaoID { get; set; }
        public string? ClienteID { get; set; }
        [ForeignKey("ClienteID")]

        public string? PlacaID { get; set; }
        [ForeignKey("PlacaID")]

        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }

    }
}
