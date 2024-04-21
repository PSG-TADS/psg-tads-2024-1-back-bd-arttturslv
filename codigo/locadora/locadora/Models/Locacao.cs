using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace locadora.Models
{
    public class Locacao
    {
        [Key]
        public string LocacaoID { get; set; }
        public string ClienteID { get; set; }
        [ForeignKey("ClienteID")]

        public string PlacaID { get; set; }
        [ForeignKey("PlacaID")]

        public DateTime DataInicio { get; set; } //quando a pessoa quer retirar o carro
        public DateTime DataTermino { get; set; } //quando a pessoa quer devolver o carro
        public DateTime? DataDevolucao { get; set; } //data que foi devolvido
        public double? Custo { get; set; }
        public bool? Status { get; set; }
        public string? Descricao { get; set; }

    }
}
