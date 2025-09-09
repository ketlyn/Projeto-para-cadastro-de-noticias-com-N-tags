# NoticiasMvc (Prova Técnica)

Projeto ASP.NET Core MVC (.NET 8) com Entity Framework Core, Migrations (Model First) e AJAX para cadastro de Notícias com N Tags.

## Requisitos
- .NET SDK 8.0+
- SQL Server (LocalDB ou instância)
- Visual Studio 2022 17.8+ (ou VS Code)  
- Baixar jQuery/jQuery Validate (usando **cdnjs** provider)

## Configuração
1. Ajuste a `connectionString` em **appsettings.Development.json** (ou **appsettings.json**):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NoticiasMvc;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

2. (Opcional) Confirme os pacotes no `.csproj`:
   - `Microsoft.EntityFrameworkCore.SqlServer`
   - `Microsoft.EntityFrameworkCore.Tools`
   - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (se tiver criado scaffold padrão)
   - `Microsoft.VisualStudio.Web.CodeGeneration.Design` (para scaffolding, opcional)

3. Bibliotecas de front-end (LibMan):
   - Provider: **cdnjs**
   - `jquery@3.7.1` → `wwwroot/lib/jquery`
   - `jquery-validate@1.19.5` → `wwwroot/lib/jquery-validation`
   - `jquery-validation-unobtrusive@3.2.12` → `wwwroot/lib/jquery-validation-unobtrusive`

## Banco de Dados (Migrations)
No **Package Manager Console**:
```powershell
Add-Migration Initial
Update-Database
```
Se precisar *seed* de um usuário padrão (para facilitar testes de Notícia):
```csharp
// ApplicationDbContext.OnModelCreating
modelBuilder.Entity<Usuario>().HasData(new Usuario{
  Id = 1, Nome = "Admin", Email = "admin@local", Senha = "123"
});
// depois
Add-Migration SeedUsuario
Update-Database
```

## Executar
- `F5` no Visual Studio (ou `dotnet run`).
- Navegação principal no menu: **Página Inicial**, **Tags**, **Notícias**, **Usuários**.

## Estrutura (resumo)
- **Models**: `Usuario`, `Tag`, `Noticia`, `NoticiaTag` (+ `NoticiaFormViewModel`).
- **Data**: `ApplicationDbContext` + `OnModelCreating` (FKs/índices) + Unit of Work (`EfUnitOfWork`).
- **Repositories**: `ITagRepository`, `INoticiaRepository`, `IUsuarioRepository` e implementações EF.
- **Services**: `TagService`, `NoticiaService`, `UsuarioService` (regras/validação).
- **Controllers**: `TagsController`, `NoticiasController` (AJAX para Create/Edit), `UsuariosController`.
- **Views**:
  - Tags/Usuários: CRUD completo com Views MVC.
  - Notícias: `Index` (listagem e botões AJAX), `Details`, `Delete` e partial `_NoticiaForm` (Create/Edit via AJAX).
- **wwwroot/js**: `noticias.js` (carrega partial por GET e envia JSON no POST).

## Fluxo AJAX (Notícia)
- **GET** `/noticias/create` → retorna partial `_NoticiaForm` (HTML).
- **POST** `/noticias/create` → recebe **JSON** `{ id, titulo, texto, usuarioId, selectedTagIds }` com anti-forgery header `RequestVerificationToken`.
- **GET** `/noticias/edit/{id}` → retorna partial preenchida.
- **POST** `/noticias/edit/{id}` → recebe **JSON** com os mesmos campos.
- JS (`noticias.js`) exibe mensagens de erro na partial e redireciona para `/noticias` em sucesso.

## Validações
- **Front-end**: DataAnnotations + jQuery Validate/Unobtrusive.
- **Back-end** (Services):
  - `Tag`: impede descrição duplicada.
  - `Usuário`: impede e-mail duplicado.
  - `Notícia`: impede título duplicado e exige **uma ou mais** Tags.

## Anti-Forgery em AJAX
- Em `_NoticiaForm.cshtml`: `@Html.AntiForgeryToken()`.
- `noticias.js`: envia header `RequestVerificationToken` com o valor do token encontrado no form.

## Rotas principais
- **Tags**: `/tags` (Index), `/tags/create`, `/tags/edit/{id}`, `/tags/delete/{id}`, `/tags/details/{id}`
- **Notícias**: `/noticias` (Index), `/noticias/create` (partial), `/noticias/edit/{id}` (partial), `/noticias/delete/{id}`, `/noticias/details/{id}`
- **Usuários**: `/usuarios` (Index), `/usuarios/create`, `/usuarios/edit/{id}`, `/usuarios/delete/{id}`, `/usuarios/details/{id}`

## Licença
Código de exemplo para avaliação técnica.
