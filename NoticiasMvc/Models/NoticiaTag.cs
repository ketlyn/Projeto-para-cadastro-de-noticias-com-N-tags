using System.ComponentModel.DataAnnotations;

namespace NoticiasMvc.Models
{
    public class NoticiaTag
    {
        public int Id { get; set; }

        [Required]
        public int NoticiaId { get; set; }

        [Required]
        public int TagId { get; set; }

        public Noticia? Noticia { get; set; }
        public Tag? Tag { get; set; }
    }
}
