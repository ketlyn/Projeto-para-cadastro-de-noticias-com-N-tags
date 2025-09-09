using System.ComponentModel.DataAnnotations;

namespace NoticiasMvc.Models
{
    public class Noticia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o campo {0}"), StringLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o campo {0}")] 
        public string Texto { get; set; } = string.Empty;

        [Required]
        public int UsuarioId { get; set; } 

        public Usuario? Usuario { get; set; }

        public ICollection<NoticiaTag>? NoticiasTags { get; set; }
    }
}
