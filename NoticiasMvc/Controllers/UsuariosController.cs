using Microsoft.AspNetCore.Mvc;
using NoticiasMvc.Models;
using NoticiasMvc.Repositories;
using NoticiasMvc.Services;

namespace NoticiasMvc.Controllers
{
    /// <summary>
    /// Controller responsável pelo CRUD de <see cref="Usuario"/> usando Views (MVC).
    /// Leitura via repositório; escrita via serviço (regras/validações).
    /// </summary>
    [Route("usuarios")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _repo;
        private readonly IUsuarioService _service;

        /// <summary>
        /// Injeta repositório e serviço de Usuário.
        /// </summary>
        public UsuariosController(IUsuarioRepository repo, IUsuarioService service)
        {
            _repo = repo;
            _service = service;
        }

        /// <summary>
        /// Lista todos os usuários ordenados por nome.
        /// </summary>
        /// <response code="200">View com a listagem de usuários.</response>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
            => View(await _repo.ListAllAsync());

        /// <summary>
        /// Exibe detalhes de um usuário específico.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="200">View de detalhes.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("details/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        /// <summary>
        /// Exibe o formulário de criação de Usuário.
        /// </summary>
        /// <response code="200">View do formulário.</response>
        [HttpGet("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Create() => View();

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="usuario">Dados do usuário (Nome, Email, Senha).</param>
        /// <response code="302">Redireciona à listagem em sucesso.</response>
        /// <response code="200">Reexibe View com mensagens de validação.</response>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([Bind("Nome,Email,Senha")] Usuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);

            var (ok, error) = await _service.CreateAsync(usuario);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error!);
                return View(usuario);
            }

            TempData["Success"] = "Usuário criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe o formulário de edição do Usuário.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="200">View de edição.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("edit/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">Identificador (rota).</param>
        /// <param name="usuario">Objeto com Id, Nome, Email e Senha.</param>
        /// <response code="302">Redireciona à listagem em sucesso.</response>
        /// <response code="200">Reexibe View com mensagens de validação.</response>
        /// <response code="404">Usuário não encontrado ou Id divergente.</response>
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit([FromRoute] int id, [Bind("Id,Nome,Email,Senha")] Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();
            if (!ModelState.IsValid) return View(usuario);

            var (ok, error) = await _service.UpdateAsync(usuario);
            if (!ok)
            {
                if (string.Equals(error, "Usuário não encontrado.", StringComparison.OrdinalIgnoreCase))
                    return NotFound();

                ModelState.AddModelError(string.Empty, error!);
                return View(usuario);
            }

            TempData["Success"] = "Usuário atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe a página de confirmação de exclusão.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="200">View de confirmação.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("delete/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        /// <summary>
        /// Exclui o usuário informado.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <response code="302">Redireciona à listagem após excluir.</response>
        [HttpPost("delete/{id:int}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var (ok, error) = await _service.DeleteAsync(id);
            if (!ok) TempData["Error"] = error;
            else TempData["Success"] = "Usuário excluído com sucesso.";

            return RedirectToAction(nameof(Index));
        }
    }
}
