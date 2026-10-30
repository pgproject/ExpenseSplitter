using ExpenseSplitter.Api.Middlewares;
using ExpenseSplitter.Infrastructure.Extensions;
using ExpenseSplitter.Api.Extensions;
using ExpenseSplitter.Application.Extensions;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();
var corsService = app.Services.GetRequiredService<
    Microsoft.AspNetCore.Cors.Infrastructure.ICorsService>();

Console.WriteLine($"CORS service registered: {corsService != null}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseRouting();
app.UseCors("Frontend");

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
