using System.ComponentModel.DataAnnotations;

namespace NoticiasMvc.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o campo {0}"), StringLength(250)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o campo {0}"), StringLength(50)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o campo {0}"), StringLength(250), EmailAddress]
        public string Email { get; set; } = string.Empty;

        public ICollection<Noticia>? Noticias { get; set; }
    }
}
