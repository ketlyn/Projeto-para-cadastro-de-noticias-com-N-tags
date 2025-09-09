using Microsoft.AspNetCore.Mvc;
using NoticiasMvc.Models;
using NoticiasMvc.Repositories;
using NoticiasMvc.Services;

namespace NoticiasMvc.Controllers
{
    [Route("tags")]
    public class TagsController : Controller
    {
        private readonly ITagRepository _repo;
        private readonly ITagService _service;

        public TagsController(ITagRepository repo, ITagService service)
        {
            _repo = repo;
            _service = service;
        }

        /// <summary>
        /// Lista todas as tags ordenadas por descrição.
        /// </summary>
        /// <response code="200">Retorna a View com a listagem de tags.</response>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
            => View(await _repo.ListAllAsync());

        /// <summary>
        /// Exibe os detalhes de uma Tag específica.
        /// </summary>
        /// <param name="id">Identificador da Tag.</param>
        /// <response code="200">Retorna a View de detalhes.</response>
        /// <response code="404">Tag não encontrada.</response>
        [HttpGet("details/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var tag = await _repo.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        /// <summary>
        /// Exibe o formulário de criação de Tag.
        /// </summary>
        /// <response code="200">Retorna a View do formulário.</response>
        [HttpGet("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Create() => View();

        /// <summary>
        /// Cadastra uma nova Tag.
        /// </summary>
        /// <param name="tag">Dados da Tag (somente Descricao).</param>
        /// <response code="302">Redireciona para a listagem em caso de sucesso.</response>
        /// <response code="200">Retorna a View com mensagens de validação em caso de erro.</response>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([Bind("Descricao")] Tag tag)
        {
            if (!ModelState.IsValid) return View(tag);

            var (ok, error) = await _service.CreateAsync(tag);
            if (!ok)
            {
                ModelState.AddModelError(nameof(Tag.Descricao), error!);
                return View(tag);
            }

            TempData["Success"] = "Tag criada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe o formulário de edição da Tag.
        /// </summary>
        /// <param name="id">Identificador da Tag.</param>
        /// <response code="200">Retorna a View de edição.</response>
        /// <response code="404">Tag não encontrada.</response>
        [HttpGet("edit/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var tag = await _repo.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        /// <summary>
        /// Atualiza uma Tag existente.
        /// </summary>
        /// <param name="id">Identificador da Tag.</param>
        /// <param name="tag">Objeto Tag com Id e Descricao.</param>
        /// <response code="302">Redireciona para a listagem em caso de sucesso.</response>
        /// <response code="200">Retorna a View com mensagens de validação em caso de erro.</response>
        /// <response code="404">Tag não encontrada.</response>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id,Descricao")] Tag tag)
        {
            if (id != tag.Id) return NotFound();
            if (!ModelState.IsValid) return View(tag);

            var (ok, error) = await _service.UpdateAsync(tag);
            if (!ok)
            {
                if (string.Equals(error, "Tag não encontrada.", StringComparison.OrdinalIgnoreCase))
                    return NotFound();

                ModelState.AddModelError(nameof(Tag.Descricao), error!);
                return View(tag);
            }

            TempData["Success"] = "Tag atualizada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe a página de confirmação de exclusão da Tag.
        /// </summary>
        /// <param name="id">Identificador da Tag.</param>
        /// <response code="200">Retorna a View de confirmação.</response>
        /// <response code="404">Tag não encontrada.</response>
        [HttpGet("delete/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var tag = await _repo.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        /// <summary>
        /// Exclui a Tag informada.
        /// </summary>
        /// <param name="id">Identificador da Tag.</param>
        /// <response code="302">Redireciona para a listagem após excluir.</response>
        [HttpPost("delete/{id:int}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var (ok, error) = await _service.DeleteAsync(id);
            if (!ok) TempData["Error"] = error;
            else TempData["Success"] = "Tag excluída com sucesso.";

            return RedirectToAction(nameof(Index));
        }
    }
}
