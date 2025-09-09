using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoticiasMvc.Models.ViewModels
{
    public class NoticiaFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "Informe o campo {0}"), StringLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Texto")]
        [Required(ErrorMessage = "Informe o campo {0}")]
        public string Texto { get; set; } = string.Empty;

        [Display(Name = "Usuário")]
        [Required]
        public int UsuarioId { get; set; }

        [Display(Name = "Tag")]
        [MinLength(1, ErrorMessage = "Informe o campo {0}")]
        public List<int> SelectedTagIds { get; set; } = new();

        public IEnumerable<SelectListItem> Tags { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Usuarios { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
