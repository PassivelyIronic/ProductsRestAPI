using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Mappers;
using ProductAPI.Persistance;
using ProductAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Dodanie us³ug do kontenera DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IForbiddenWordRepository, ForbiddenWordRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAutoMapper(typeof(ProductProfile));


// Dodanie kontrolerów i Swaggera
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Uruchomienie aplikacji
app.Run();
