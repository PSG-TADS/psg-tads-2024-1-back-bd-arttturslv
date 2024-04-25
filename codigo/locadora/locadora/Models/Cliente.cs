using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace locadora.Models
{
    public class Cliente
    {
        [Key]
        public string CPF { get; set; } 
        public string Nome { get; set; } 
        public string Telefone { get; set; }
        public string Email { get; set; }
        public ICollection<Locacao>? Locacoes { get; set; } = new List<Locacao>();

    }
}
