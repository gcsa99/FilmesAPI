using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme(
        [FromBody] CreateFilmeDto filmeDto)
    {
        //passo o Dto para ser feito o map para o model filme
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);//Adiciona os dados
        _context.SaveChanges();//Salva no BD
        return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filme); //retorna o nome, id e o caminho
    }

    /// <summary>
    /// Retorna os filmes presentes no banco de dados
    /// </summary>
    /// <param name="skip">Id inicial que deseja consultar</param>
    /// <param name="take">Quantidade de resultados que deseja visualizar</param>
    /// <param name="nomeCinema">Nome do cinema no qual deseja pesquisar</param>
    /// <returns>IEnumerable</returns>
    /// <response code="200"></response>

    [HttpGet]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0, 
                                                    [FromQuery] int take = 10, 
                                                    [FromQuery] string? nomeCinema = null)
    {
        if (nomeCinema == null)
        { 
            //recebo na query o skip (onde quero iniciar) e o take (quantos quero exibir) ambos opcionais
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take).ToList());
        }

        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take)
                                                              .Where(filme => filme.Sessoes
                                                              .Any(sessao => sessao.Cinema.Nome == nomeCinema)).ToList());

    }

    /// <summary>
    /// Retorna o filme com id indicado na query
    /// </summary>
    /// <param name="id">Id que deseja consultar</param>
    /// <returns>IActionResult</returns>
    /// <response code="200"></response>

    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);//procura se existe aquele id
        if (filme == null) return NotFound(); //retorna o status code 404
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto); //retorna o status code 200
    }

    /// <summary>
    /// Permite a atualização de todos os dados de um filme
    /// </summary>
    /// <param name="id">Id do filme que deseja alterar</param>
    /// <param name="filmeDto">Objeto que deverá ser enviado</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Sucesso</response>
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id,
        [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null)
            return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Permite a atualização de todos os dados de um filme
    /// </summary>
    /// <param name="id">Id do filme que deseja alterar</param>
    /// <param name="patch">Objeto que deverá ser enviado no seguinte padrão:
    /// {
    ///  "op": "replace",
    ///  "path": "/atributo",
    ///  "value": "string"
    /// }
    /// </param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Sucesso</response>
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id,
        JsonPatchDocument<UpdateFilmeDto> patch)//recebe json com parte do objeto
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null)
            return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);
        if (!TryValidateModel(filmeParaAtualizar))
            return ValidationProblem(ModelState);

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Exclusão de um filme do sistema
    /// </summary>
    /// <param name="id">Id do filme que deseja excluir</param>
    /// <returns code="204">Sucesso</returns>
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null)
            return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
