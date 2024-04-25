using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using locadora.Models;
using System.Text.RegularExpressions;

namespace locadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ClientesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Clientes - Retorna todos os clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
        {
            return await _context.Cliente.Include(c => c.Locacoes).ToListAsync();
        }

        // GET: api/Clientes/5 - Retorna o cliente especifico ou NotFound
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(string id)
        {
            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("O cliente não foi encontrado, verifique o CPF informado.");
            }

            return cliente;
        }

        // PUT: api/Clientes/5 - Modifica clientes
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(string id, Cliente cliente)
        {
            cliente.CPF = formatarNumeros(cliente.CPF);
            cliente.Telefone = formatarNumeros(cliente.Telefone);

            if (id != cliente.CPF)
            {
                return BadRequest("Não é possivel modificar o CPF.");
            }
            else if (!numeroValido(cliente.Telefone))
            {
                return BadRequest("Numero de telefone é invalido. Utilize o padrão com DDD [abcd-mcdu] (fixo) ou DDD [abcde-mcdu] (telefone movel)");
            }
            else if (!emailValido(cliente.Email))
            {
                return BadRequest("O email é invalido. Utilize o padrão com 'nome@operador.com'.");
            }
            else if (!nomeValido(cliente.Nome))
            {
                return BadRequest("O nome é invalido. O nome não pode conter caracteres especiais.");
            }

            var locacoesAnteriores = await _context.Locacao.Where(l => l.ClienteID == cliente.CPF).ToListAsync();
            cliente.Locacoes = locacoesAnteriores;

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Cliente modificado. Atenção, devido as regras de negocio, não é possivel modificar as locações pelo cliente.");
        }

        // POST: api/Clientes - Permite adicionar um novo cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            cliente.CPF = formatarNumeros(cliente.CPF);
            cliente.Telefone = formatarNumeros(cliente.Telefone);
            cliente.Locacoes = new List<Locacao>();

            if (!CPFValido(cliente.CPF))
            {
                return BadRequest("CPF invalido. Verifique a quantidade de algarismos.");
            }
            else if (!numeroValido(cliente.Telefone))
            {
                return BadRequest("Numero de telefone é invalido. Utilize o padrão com DDD [abcd-mcdu] (fixo) ou DDD [abcde-mcdu] (telefone movel)");
            }
            else if (!emailValido(cliente.Email))
            {
                return BadRequest("O email é invalido. Utilize o padrão com 'nome@operador.com'.");
            }
            else if (!nomeValido(cliente.Nome))
            {
                return BadRequest("O nome é invalido. O nome não pode conter caracteres especiais.");
            }

            _context.Cliente.Add(cliente);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ClienteExists(cliente.CPF))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCliente", new { id = cliente.CPF }, $"Cliente criado com sucesso. Devido às regras de negócio, as locações foram salvas vazias e só poderão ser preenchidas no endpoint de alugar.\n\nDetalhes do Cliente:\nNome: {cliente.Nome}\nCPF: {cliente.CPF}\nEmail: {cliente.Email}\nTelefone: {cliente.Telefone}\nLocações: []");

        }


        // DELETE: api/Clientes/5 - Permite deletar um cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(string id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            if(cliente.Locacoes != null)
            {
                foreach (var locacao in cliente.Locacoes)
                {
                    if (locacao.Ativa == true)
                    {
                        return BadRequest("Não é possivel apagar dados do cliente, pois ele possui uma ou mais locações ativas.");
                    }
                }
                _context.Locacao.RemoveRange(cliente.Locacoes);
            }

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok("Cliente foi apagado, junto com todas as locações relacionadas a ele.");
        }


        // funções

        private bool ClienteExists(string id)
        {
            return _context.Cliente.Any(e => e.CPF == id);
        }

        private bool CPFValido(string CPF)
        {
            if (CPF.Length != 11)
            {
                return false;
            }
            return true;
        }

        private string formatarNumeros(string ex)
        {
            string apenasNumeros = Regex.Replace(ex, @"[^\d\-.]", "");
            return apenasNumeros;
        }

        private bool numeroValido(string numero)
        {
            if (numero.Length == 11 || numero.Length == 10) // Padrao com DDD [abcd-mcdu] (fixo) ou DDD [abcde-mcdu] (telefone movel)
            {
                return true;
            }

            return false;
        }

        private bool nomeValido(string nome)
        {

            Regex apenasLetraseEspacos = new Regex("^[a-zA-Z ]+$");

            if (apenasLetraseEspacos.IsMatch(nome))
            {
                return true;
            }

            return false;
        }

        private bool emailValido(string email)
        {

            Regex emailValido = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            if (emailValido.IsMatch(email))
            {
                return true;
            }

            return false;
        }
    }
}