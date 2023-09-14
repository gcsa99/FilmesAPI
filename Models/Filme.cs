using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models;

public class Filme
{
    //Para criar o migration: 
    //Add-Migration *o que esta fazendo*(nome: string) => cria o model
    //Update-Database => Roda o migration, criando a tabela
    //O model será utilizado como regra para o banco de dados, pode ser utilizado para criar as tabelas e também tem as ultimas validações antes de inserir no banco os dados

    [Key]
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título do filme é obrigatório")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O gênero do filme é obrigatório")]
    [MaxLength(50, ErrorMessage = "Tamanho maximo do gênero não pode exceder 50 caracteres")]
    public string Genero { get; set; }

    [Required]
    [Range(70, 600, ErrorMessage = "A duração deve ter entre 70 e 600 minutos")]
    public int Duracao { get; set; }

    public virtual ICollection<Sessao> Sessoes { get; set; }
}
