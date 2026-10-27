using Desp2Lab3.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<DocumentoStore>();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
    options.ValueLengthLimit = 10 * 1024 * 1024;
    options.MemoryBufferThreshold = 10 * 1024 * 1024;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
    try
    {
        await next();
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] -> {context.Response.StatusCode}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] EXCEPCION: {ex}");
        throw;
    }
});

app.MapRazorPages();

Console.WriteLine("App lista. No cierres esta ventana mientras pruebas.");
app.Run();
