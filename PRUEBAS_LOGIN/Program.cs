using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Habilitar la autenticaci�n con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login"; // Redirigir a Login si el usuario no est� autenticado
        options.LogoutPath = "/Acceso/Logout"; // Ruta para logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n de la cookie
        options.SlidingExpiration = true; // Permitir que la cookie se renueve si el usuario sigue activo
    });

// Habilitar la sesi�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo que la sesi�n puede estar inactiva
    options.Cookie.HttpOnly = true; // Solo accesible por el servidor
    options.Cookie.IsEssential = true; // Necesario para la aplicaci�n
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

// Usar la autenticaci�n antes de la autorizaci�n
app.UseAuthentication();  // Aseg�rate de que la autenticaci�n est� antes que la autorizaci�n
app.UseAuthorization();

// Usar la sesi�n para almacenar los datos del usuario
app.UseSession();  // Aseg�rate de que la sesi�n est� habilitada

// Mapeo de rutas para el controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
