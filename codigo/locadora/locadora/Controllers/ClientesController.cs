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

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
        {
            return await _context.Cliente.ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(string id)
        {
            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // GET: api/Clientes/atrasos
        [HttpGet("atrasos")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientesAtrasados()
        {
            var clientes = _context.Cliente.ToListAsync();
            var clientesAtrasados = new List<Cliente>();

            foreach (var cliente in await clientes)
            {
                foreach(var locacao in cliente.Locacoes)
                {
                    if(locacao.DataInicio < locacao.DataDevolucao)
                    {
                        clientesAtrasados.Add(cliente);
                        break;
                    }
                }
            }
            return clientesAtrasados;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(string id, Cliente cliente)
        {
            if (id != cliente.CPF)
            {
                return BadRequest("O CPF informado nos parametros é diferente do informado no corpo da requisição.");
            }

            if (!CPFValido(cliente.CPF))
            {
                return BadRequest("CPF é invalido.");

            }
            else if (!numeroValido(cliente.Telefone))
            {
                return BadRequest("Numero de telefone é invalido.");

            }
            else if (!emailValido(cliente.Email))
            {
                return BadRequest("O email é invalido.");

            }
            else if (!nomeValido(cliente.Nome))
            {
                return BadRequest("O nome é invalido.");
            }

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

            return NoContent();
        }

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {

            if(!CPFValido(cliente.CPF))
            {
                return BadRequest("CPF é invalido.");

            } else if (!numeroValido(cliente.Telefone))
            {
                return BadRequest("Numero de telefone é invalido.");

            } else if (!emailValido(cliente.Email))
            {
                return BadRequest("O email é invalido.");

            } else if (!nomeValido(cliente.Nome))
            {
                return BadRequest("O nome é invalido.");
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

            return CreatedAtAction("GetCliente", new { id = cliente.CPF }, cliente);
        }

        
        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(string id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // funções

        private bool ClienteExists(string id)
        {
            return _context.Cliente.Any(e => e.CPF == id);
        }

        private bool CPFValido(string CPF)
        {
            string newCPF = CPF.Replace(".", "");
            newCPF = CPF.Replace("-", "");
            Regex apenasNumeros = new Regex("^[0-9]+$");

            if (CPF.Length != 11)
            {
                return false;
            }

            if (!apenasNumeros.IsMatch(CPF))
            {
                return false;
            }

            return true;
        }

        private bool numeroValido (string numero)
        {
            string newNumero = numero.Replace("(", "");
            newNumero = numero.Replace(")", "");
            newNumero = numero.Replace("-", "");
            newNumero = numero.Replace(" ", "");
            Regex contemLetras = new Regex("[a-zA-Z]");

            if (contemLetras.IsMatch(numero))
            {
                return false;
            }

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
