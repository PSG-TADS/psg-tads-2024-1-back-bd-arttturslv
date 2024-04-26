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

        // GET: api/Locacoes - Retorna todas as locações
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locacao>>> GetLocacao()
        {
            return await _context.Locacao.ToListAsync();
        }

        // GET: api/Locacoes/5 - Retorna a locacao especifica ou NotFound
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

        // GET: api/Locacoes/ativas - Retorna todas as locações que estao ativas
        [HttpGet("ativas")]
        public async Task<ActionResult<IEnumerable<Locacao>>> GetLocacoesAtivas()
        {
            var locacoes = _context.Locacao.ToListAsync();
            var locacoesAtivas = new List<Locacao>();

            foreach (var locacao in await locacoes)
            {
                if(locacao.Ativa)
                {
                    locacoesAtivas.Add(locacao);
                }
            }
            return locacoesAtivas;
        }

        // GET: api/Locacoes/nao-ativas - Retorna todas as locações que estao não estao ativas
        [HttpGet("nao-ativas")]
        public async Task<ActionResult<IEnumerable<Locacao>>> GetLocacoesNaoAtivas()
        {
            var locacoes = _context.Locacao.ToListAsync();
            var locacoesNaoAtivas = new List<Locacao>();

            foreach (var locacao in await locacoes)
            {
                if (!locacao.Ativa)
                {
                    locacoesNaoAtivas.Add(locacao);
                }
            }
            return locacoesNaoAtivas;
        }


        // PUT: api/Locacoes/devolver/5 - Usado para devolver um veiculo
        [HttpPut("devolver/{id}")]
        public async Task<ActionResult<Locacao>> DevolverVeiculo(string id, DateTime dataDevolucao)
        {
            var locacao = await _context.Locacao.FindAsync(id);

            if (locacao == null)
            {
                return NotFound("Locação não encontrada.");
            } 
            else if (locacao.Ativa == false)
            {
                return BadRequest("Esta locação já foi devolvida.");
            }
            else if (!dataValida(dataDevolucao))
            {
                return BadRequest("A data de devolução deve ser fornecida no formato 'yyyy-MM-dd'.");
            }
            else if (locacao.DataInicio > dataDevolucao)
            {
                return BadRequest("A data de inicio não pode ser mais recente que a data de devolução.");
            }

            var veiculo = await _context.Veiculo.FindAsync(locacao.PlacaID);

            int tempoPassado = (locacao.DataTermino.Subtract(locacao.DataInicio)).Days;
            int atraso = (dataDevolucao.Subtract(locacao.DataTermino)).Days;

            double custo = veiculo.PrecoDiario * tempoPassado;
            double custoAtraso = veiculo.PrecoAtrasoDiario * atraso;
            double custoTotal = custo + custoAtraso;

            veiculo.Disponibilidade = true; //atualiza a disponibilidade, agora que a alocacao acabou

            locacao.DataDevolucao = dataDevolucao;
            locacao.Custo = custoTotal;
            locacao.Ativa = false; //agora a locacao fica como concluida

            locacao.Descricao += "O veiculo foi entregue com "+atraso+" dias de atraso e "+tempoPassado+" dias estipulados no contrato. O custo de R$"+custo+" pelos dias do contrato e mais R$"+custoAtraso+" pelo atraso, totalizando R$"+custoTotal;
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
                return BadRequest("As datas devem ser fornecidas no formato 'yyyy-MM-dd");
            }
            else if (locacao.DataInicio > locacao.DataTermino)
            {
                return BadRequest("A data de inicio não pode ser mais recente que a data de termino.");
            }
            else if (clienteExistente == null)
            {
                return BadRequest("O cliente não existe.");
            }
            else if (veiculoExistente == null)
            {
                return BadRequest("O veiculo não existe.");
            }
            else if (veiculoExistente.Disponibilidade == false)
            {
                return BadRequest("O veiculo não está disponivel.");
            }

            locacao.DataDevolucao = null; //a devolucao deve ser nula até que o veiculo seja devolvido
            locacao.Custo = 0; //o custo é calculado apos a devolucao
            locacao.Ativa = true; //o status é atualizado apos a devolucao
            locacao.Descricao = "A locacao foi feita para o cliente: " + clienteExistente.Nome + " com duração de " + (locacao.DataTermino.Subtract(locacao.DataInicio)).Days + " dias."; 
            veiculoExistente.Disponibilidade = false; //atualiza o veiculo, nao deixando ser usado.
            
            clienteExistente.Locacoes.Add(locacao);

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

            return CreatedAtAction("GetLocacao", new { id = locacao.LocacaoID }, "Locação criada com sucesso! Devido as regras de negocio, as propriedades DataDevolucao, Custo, Ativa e Descrição foram atualizas com os valores padrão.");
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
            if(locacao.Ativa == true)
            {
                return BadRequest("A locação está ativa e portanto não pode ser apagada.");
            }

            var veiculoExistente = await _context.Veiculo.FindAsync(locacao.PlacaID);
            veiculoExistente.Disponibilidade = true;

            _context.Locacao.Remove(locacao);
            await _context.SaveChangesAsync();

            return Ok("A locação foi apagada com sucesso.");
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
