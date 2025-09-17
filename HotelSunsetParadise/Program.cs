var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar el servicio de sesiones
builder.Services.AddDistributedMemoryCache(); // Necesario para usar sesiones en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Usar el middleware de sesiones
app.UseSession();

// Middleware para verificar si el usuario está logueado
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/Login") || path == "/")
    {
        await next();
        return;
    }

    var isLoggedIn = context.Session.GetString("IsLoggedIn");
    if (string.IsNullOrEmpty(isLoggedIn))
    {
        context.Response.Redirect("/Login/Login");
        return;
    }
    await next();
});

// Ruta por defecto para que la página de inicio sea la de login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
