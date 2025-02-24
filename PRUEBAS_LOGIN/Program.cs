using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Habilitar la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login"; // Redirigir a Login si el usuario no está autenticado
        options.LogoutPath = "/Acceso/Logout"; // Ruta para logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración de la cookie
        options.SlidingExpiration = true; // Permitir que la cookie se renueve si el usuario sigue activo
    });

// Habilitar la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo que la sesión puede estar inactiva
    options.Cookie.HttpOnly = true; // Solo accesible por el servidor
    options.Cookie.IsEssential = true; // Necesario para la aplicación
});

// Agregar controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usar la autenticación antes de la autorización
app.UseAuthentication();  // Asegúrate de que la autenticación esté antes que la autorización
app.UseAuthorization();

// Usar la sesión para almacenar los datos del usuario
app.UseSession();  // Asegúrate de que la sesión esté habilitada

// Mapeo de rutas para el controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
