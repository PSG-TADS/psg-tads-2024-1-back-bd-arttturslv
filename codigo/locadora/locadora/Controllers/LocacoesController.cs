using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using locadora.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace locadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocacoesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LocacoesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Locacoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locacao>>> GetLocacao()
        {
            return await _context.Locacao.ToListAsync();
        }

        // GET: api/Locacoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Locacao>> GetLocacao(string id)
        {
            var locacao = await _context.Locacao.FindAsync(id);

            if (locacao == null)
            {
                return NotFound();
            }
            return locacao;
        }

        [HttpGet("ativas")]
        public async Task<ActionResult<IEnumerable<Locacao>>> GetLocacoesAtivas()
        {
            var locacoes = _context.Locacao.ToListAsync();
            var locacoesAtivas = new List<Locacao>();

            foreach (var locacao in await locacoes)
            {
                if(locacao.Status.HasValue && locacao.Status.Value)
                {
                    locacoesAtivas.Add(locacao);
                }
            }
            return locacoesAtivas;
        }

        // PUT: api/Locacoes/devolver/5
        [HttpPut("devolver/{id}")]
        public async Task<ActionResult<Locacao>> DevolverVeiculo(string id, DateTime dataDevolucao)
        {
            var locacao = await _context.Locacao.FindAsync(id);

            if (locacao == null)
            {
                return NotFound("Locação não encontrada.");
            }

            if (locacao.Status == false)
            {
                return BadRequest("Esta locação já foi devolvida.");
            }

            if (!dataValida(dataDevolucao))
            {
                return BadRequest("A data de devolução deve ser fornecida no formato 'yyyy-MM-dd'.");
            }

            if (locacao.DataInicio > dataDevolucao)
            {
                return BadRequest("A data de inicio não pode ser mais recente que a data de devolucao");
            }

            var veiculo = await _context.Veiculo.FindAsync(locacao.PlacaID);

            if (veiculo != null)
            {
                veiculo.Disponibilidade = true;
            }

            TimeSpan tempoPassado = locacao.DataTermino.Subtract(locacao.DataInicio);
            TimeSpan atraso = dataDevolucao.Subtract(locacao.DataTermino);

            double custo = veiculo.PrecoDiario * tempoPassado.Days;
            double custoAtraso = veiculo.AdicionalPrecoAtrasoDiario * atraso.Days;
            double custoTotal = custo + custoAtraso;

            locacao.DataDevolucao = dataDevolucao;
            locacao.Custo = custoTotal;
            locacao.Status = false;
            locacao.Descricao = "O veiculo foi entregue com "+atraso+" dias de atraso e "+tempoPassado+" dias estipulados no contrato. O custo total é de R$"+custo+" pelos dias do contrato e mais R$"+custoAtraso+" pelo atraso, totalizando R$"+custoTotal;
            veiculo.Disponibilidade = true; //veiculo volta a estar disponivel
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocacaoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(locacao.Descricao);
        }


        // POST: api/Locacoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("alugar/")]
        public async Task<ActionResult<Locacao>> AlugarVeiculo(Locacao locacao)
        {
            var clienteExistente = await _context.Cliente.FindAsync(locacao.ClienteID);
            var veiculoExistente = await _context.Veiculo.FindAsync(locacao.PlacaID);

            if (!dataValida(locacao.DataInicio) || !dataValida(locacao.DataTermino))
            {
                return BadRequest("As datas devem ser fornecidas no formato 'yyyy-MM-ddT00:00:00Z'.");
            }

            if (locacao.DataInicio < locacao.DataTermino)
            {
                return BadRequest("A data de termino não pode ser mais recente que a data de inicio");
            }

            if (clienteExistente == null)
            {
                return BadRequest("O cliente não existe.");
            }

            if (veiculoExistente == null)
            {
                return BadRequest("O veiculo não existe.");
            }

            if (veiculoExistente.Disponibilidade == false)
            {
                return BadRequest("O veiculo não está disponivel. Ele já foi alugado.");
            }

            locacao.DataDevolucao = null; //a devolucao deve ser nula até que o veiculo seja devolvido
            locacao.Custo = 0; //o custo é calculado apos a devolucao
            locacao.Status = true; //o status é atualizado apos a devolucao
            locacao.Descricao = ""; //o status é atualizado apos a devolucao
            veiculoExistente.Disponibilidade = false; //atualiza o veiculo, nao deixando ser usado.

            _context.Locacao.Add(locacao);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LocacaoExists(locacao.LocacaoID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLocacao", new { id = locacao.LocacaoID }, locacao);
        }

        // DELETE: api/Locacoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocacao(string id)
        {
            var locacao = await _context.Locacao.FindAsync(id);
            if (locacao == null)
            {
                return NotFound();
            }

            _context.Locacao.Remove(locacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocacaoExists(string id)
        {
            return _context.Locacao.Any(e => e.LocacaoID == id);
        }
        private bool dataValida(DateTime data)
        {
            return data != default && data.Date == data;
        }
    }
}

/*

        // PUT: api/Locacoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocacao(string id, Locacao locacao)
        {
            if (id != locacao.LocacaoID)
            {
                return BadRequest("O id informado nos parametros é diferente do informado no corpo da requisição.");
            }

            _context.Entry(locacao).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocacaoExists(id))
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

   [HttpPost]
        public async Task<ActionResult<Locacao>> PostLocacao(Locacao locacao)
        {
            var clienteExistente = await _context.Cliente.FindAsync(locacao.ClienteID);
            var veiculoExistente = await _context.Veiculo.FindAsync(locacao.PlacaID);

            if (!dataValida(locacao.DataInicio) || !dataValida(locacao.DataTermino))
            {
                return BadRequest("As datas devem ser fornecidas no formato 'yyyy-MM-ddT00:00:00Z'.");
            }

            if (clienteExistente == null)
            {
                return BadRequest("O cliente não existe.");
            }

            if (veiculoExistente == null)
            {
                return BadRequest("O veiculo não existe.");
            }

            if (veiculoExistente.Disponibilidade == false)
            {
                return BadRequest("O veiculo não está disponivel. Ele já foi alugado.");
            }

            locacao.DataDevolucao = null; //a devolucao deve ser nula até que o veiculo seja devolvido
            locacao.Custo = 0; //o custo é calculado apos a devolucao
            locacao.Status = true; //o status é atualizado apos a devolucao
            locacao.Descricao = ""; //o status é atualizado apos a devolucao
            veiculoExistente.Disponibilidade = false; //atualiza o veiculo, nao deixando ser usado.

            _context.Locacao.Add(locacao);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LocacaoExists(locacao.LocacaoID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLocacao", new { id = locacao.LocacaoID }, locacao);
        }

 
 */