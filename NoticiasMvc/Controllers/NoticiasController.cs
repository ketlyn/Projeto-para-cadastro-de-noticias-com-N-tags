using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoticiasMvc.Models;
using NoticiasMvc.Models.ViewModels;
using NoticiasMvc.Repositories;
using NoticiasMvc.Services;

namespace NoticiasMvc.Controllers
{
    [Route("noticias")]
    public class NoticiasController : Controller
    {
        private readonly INoticiaRepository _repo;
        private readonly ITagRepository _tagRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly INoticiaService _service;

        public NoticiasController(
            INoticiaRepository repo,
            ITagRepository tagRepo,
            IUsuarioRepository usuarioRepo,
            INoticiaService service)
        {
            _repo = repo;
            _tagRepo = tagRepo;
            _usuarioRepo = usuarioRepo;
            _service = service;
        }

        /// <summary>
        /// Lista de notícias com usuário e tags.
        /// </summary>
        /// <response code="200">Retorna a View com listagem.</response>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            var noticias = await _repo.ListAllAsync();
            return View(noticias);
        }

        /// <summary>
        /// Detalhes de uma notícia.
        /// </summary>
        /// <param name="id">Id da notícia.</param>
        /// <response code="200">View de detalhes.</response>
        /// <response code="404">Não encontrada.</response>
        [HttpGet("details/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var noticia = await _repo.GetByIdAsync(id);
            if (noticia == null) return NotFound();
            return View(noticia);
        }

        /// <summary>
        /// Carrega partial do formulário de criação (AJAX GET).
        /// </summary>
        [HttpGet("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildFormViewModelAsync();
            ViewData["FormAction"] = Url.Action(nameof(CreatePost), "Noticias");
            return PartialView("_NoticiaForm", vm);
        }

        /// <summary>
        /// Salva criação via AJAX recebendo JSON.
        /// </summary>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> CreatePost([FromBody] NoticiaFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, error = "Dados inválidos." });

            var entity = new Noticia
            {
                Titulo = vm.Titulo,
                Texto = vm.Texto,
                UsuarioId = vm.UsuarioId
            };

            var (ok, error, id) = await _service.CreateAsync(entity, vm.SelectedTagIds ?? Enumerable.Empty<int>());
            if (!ok) return BadRequest(new { ok, error });

            return Ok(new { ok = true, id });
        }

        /// <summary>
        /// Carrega partial do formulário de edição (AJAX GET).
        /// </summary>
        [HttpGet("edit/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var noticia = await _repo.GetByIdWithTagsForUpdateAsync(id);
            if (noticia == null) return NotFound();

            var vm = await BuildFormViewModelAsync(noticia);
            ViewData["FormAction"] = Url.Action(nameof(EditPost), "Noticias", new { id = id });
            return PartialView("_NoticiaForm", vm);
        }

        /// <summary>
        /// Salva edição via AJAX recebendo JSON.
        /// </summary>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> EditPost([FromRoute] int id, [FromBody] NoticiaFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, error = "Dados inválidos." });

            var entity = new Noticia
            {
                Id = vm.Id,
                Titulo = vm.Titulo,
                Texto = vm.Texto,
                UsuarioId = vm.UsuarioId
            };

            var (ok, error) = await _service.UpdateAsync(entity, vm.SelectedTagIds ?? Enumerable.Empty<int>());
            if (!ok) return BadRequest(new { ok, error });

            return Ok(new { ok = true });
        }

        /// <summary>
        /// Confirmação de exclusão (back-end).
        /// </summary>
        [HttpGet("delete/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var noticia = await _repo.GetByIdAsync(id);
            if (noticia == null) return NotFound();
            return View(noticia);
        }

        /// <summary>
        /// Exclui (back-end).
        /// </summary>
        [HttpPost("delete/{id:int}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<NoticiaFormViewModel> BuildFormViewModelAsync(Noticia? n = null)
        {
            var tags = await _tagRepo.ListAllAsync();
            var usuarios = await _usuarioRepo.ListAllAsync();

            var vm = new NoticiaFormViewModel
            {
                Id = n?.Id ?? 0,
                Titulo = n?.Titulo ?? string.Empty,
                Texto = n?.Texto ?? string.Empty,
                UsuarioId = n?.UsuarioId ?? (usuarios.FirstOrDefault()?.Id ?? 1),
                SelectedTagIds = n?.NoticiasTags?.Select(nt => nt.TagId).ToList() ?? new List<int>(),
                Tags = tags.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Descricao }).ToList(),
                Usuarios = usuarios.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nome }).ToList()
            };
            return vm;
        }
    }
}