using System.ComponentModel.DataAnnotations;

namespace locadora.Models
{
    public class Veiculo
    {
        [Key]
        public string PlacaID { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public int Ano { get; set; }
        public double PrecoDiario { get; set; }
        public double PrecoAtrasoDiario { get; set; } //quando atrasarem a entrega do carro
        public bool Disponibilidade { get; set; }
    }
}
