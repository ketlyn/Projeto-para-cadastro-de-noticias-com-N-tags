using Microsoft.EntityFrameworkCore;
using NoticiasMvc.Data;
using NoticiasMvc.Repositories.Abstractions;
using NoticiasMvc.Repositories;          
using NoticiasMvc.Repositories.Ef;       
using NoticiasMvc.Services;              

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// Anti-forgery (AJAX JSON usa header RequestVerificationToken)
builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

// UoW
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// TAG
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// NOTÍCIA + USUÁRIO
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<INoticiaRepository, NoticiaRepository>();
builder.Services.AddScoped<INoticiaService, NoticiaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Mapeia rotas convencionais (Home/Tags) e também as com atributos
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers(); // garante as rotas com [Route("tags")]/[Route("noticias")]

app.Run();
