using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace locadora.Models
{
    public class Locacao
    {
        [Key] public string LocacaoID { get; set; } //id da locacao
        public string ClienteID { get; set; } //id do cliente que alugou
        [ForeignKey("ClienteID")]

        public string PlacaID { get; set; } //id do veiculo alugado
        [ForeignKey("PlacaID")]

        public DateTime DataInicio { get; set; } //quando o cliente quer pegar o veiculo
        public DateTime DataTermino { get; set; } //quando a pessoa quer devolver o veiculo

        public DateTime? DataDevolucao { get; set; } //data que foi devolvido
        public double? Custo { get; set; } //custo total do veiculo (diaria e possiveis multas)
        public bool Ativa { get; set; } //se a locacao esta ativa, ou já foi concluida
        public string? Descricao { get; set; } //descricao geral

    }
}
