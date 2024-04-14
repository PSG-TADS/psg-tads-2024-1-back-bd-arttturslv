using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace locadora.Models
{
    public class Veiculo
    {
        [Key]
        public string? PlacaID { get; set; }
        public string? Modelo { get; set; }
        public string? Marca { get; set; }
        public int Ano { get; set; }
        public double Preco { get; set; }

        public bool Disponibilidade { get; set; }
    }
}
