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
    public class VeiculosController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public VeiculosController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Veiculos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculo()
        {
            return await _context.Veiculo.ToListAsync();
        }

        // GET: api/Veiculos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Veiculo>> GetVeiculo(string id)
        {
            var veiculo = await _context.Veiculo.FindAsync(id);

            if (veiculo == null)
            {
                return NotFound("O placa digitada não pertence a nenhum veiculo.");
            }

            return veiculo;
        }
        //GET: api/Veiculos/disponiveis
        [HttpGet("disponiveis")]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculosDisponiveis()
        {
            var veiculos = _context.Veiculo.ToListAsync();
            var veiculosDisponiveis = new List<Veiculo>();

            foreach (var veiculo in await veiculos)
            {
                if(veiculo.Disponibilidade)
                {
                    veiculosDisponiveis.Add(veiculo);
                }
            }
            return veiculosDisponiveis;
        }

        // PUT: api/Veiculos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVeiculo(string id, Veiculo veiculo)
        {
            if (id != veiculo.PlacaID)
            {
                return BadRequest("A placa informada nos parametros é diferente do informado no corpo da requisição.");
            }

            if(!placaValida(veiculo.PlacaID))
            {
                return BadRequest("A placa deve seguir o padrão utilizar apenas letras e numeros, com 7 digitos.");
            }

            _context.Entry(veiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VeiculoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Veiculo modificado.");
        }

        // POST: api/Veiculos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Veiculo>> PostVeiculo(Veiculo veiculo)
        {
            if (!placaValida(veiculo.PlacaID))
            {
                return BadRequest("A placa deve seguir o padrão utilizar apenas letras e numeros, no formato 4 letras e 3 numeros ou 4 numeros e 3 letras.");
            }

            _context.Veiculo.Add(veiculo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VeiculoExists(veiculo.PlacaID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVeiculo", new { id = veiculo.PlacaID }, veiculo);
        }

        // DELETE: api/Veiculos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeiculo(string id)
        {
            var veiculo = await _context.Veiculo.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            if(veiculo.Disponibilidade==false)
            {
                return BadRequest("Informações do veiculo não podem ser deletadas, pois ele está alugado.");
            }

            _context.Veiculo.Remove(veiculo);
            await _context.SaveChangesAsync();

            return Ok("Veiculo foi apagado.");
        }

        private bool VeiculoExists(string id)
        {
            return _context.Veiculo.Any(e => e.PlacaID == id);
        }
        private bool placaValida(string Placa)
        {
            Regex placaSemSimbolo = new Regex("^[a-zA-Z0-9]*$");

            if (Placa.Length != 7)
            {
                return false;
            }

            if(!placaSemSimbolo.IsMatch(Placa))
            {
                return false;
            }

            return true;
        }
    }
}
