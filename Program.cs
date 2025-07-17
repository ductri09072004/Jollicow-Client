using Jollicow.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Service Mã hóa
// builder.Services.AddDataProtection(); // Tạm thời tắt mã hóa để kiểm tra lỗi
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ToppingService>();
builder.Services.AddScoped<OrderService>();


var app = builder.Build();

// Middleware
// app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Menu/Error");
}
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/menu/generate?id_table=B02&restaurant_id=CHA1001");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Menu}/{action=Menu}/{id?}",
    defaults: new { id_table = "B02", restaurant_id = "CHA1001" });

app.Run();
