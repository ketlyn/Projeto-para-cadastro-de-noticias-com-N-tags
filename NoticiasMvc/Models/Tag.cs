using System.ComponentModel.DataAnnotations;

namespace NoticiasMvc.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Informe o campo {0}")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        public ICollection<NoticiaTag>? NoticiasTags { get; set; }
    }
}
