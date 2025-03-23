using Cars.ApiCommon.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add cosmos options.
builder.AddCosmosContainerOptions();
builder.AddCosmosAccountOptions();

// Add services to the DI container.
builder.RegisterServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
