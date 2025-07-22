using Microsoft.EntityFrameworkCore;
using Mobile_Server_Dioxide.Context;
using DotNetEnv;
using Mobile_Server_Dioxide.Services.TickerServices;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DioxieReadDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DioxieMobileRead"), 
    sqlServerOptions => {
        sqlServerOptions.CommandTimeout(120);
    }));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<ITickerService, TickerService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Home/Error");
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthorization();

app.MapControllers();

app.Run();
