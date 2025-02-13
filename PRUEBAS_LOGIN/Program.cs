var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar soporte para sesiones
builder.Services.AddDistributedMemoryCache();  // Esto es necesario para almacenar la sesi�n en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Tiempo de expiraci�n de la sesi�n
    options.Cookie.HttpOnly = true;  // La cookie de sesi�n ser� accesible solo por HTTP
    options.Cookie.IsEssential = true;  // Necesario para que funcione la sesi�n en ciertos contextos (como en el GDPR)
});

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Agregar middleware de sesi�n
app.UseSession();  // Esto habilita el uso de sesiones en la aplicaci�n

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
