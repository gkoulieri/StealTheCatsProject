using Microsoft.EntityFrameworkCore;
using StealTheCatsAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient for consuming the external Cat API
builder.Services.AddHttpClient("CatApiClient", client =>
{
    client.BaseAddress = new Uri("https://api.thecatapi.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Configure the DbContext with SQL Server
builder.Services.AddDbContext<CatsDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers with JSON options to prevent circular references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Exception Handling for non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseStaticFiles(); // Ensure this is included
app.UseAuthorization();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StealTheCats API V1");
    c.RoutePrefix = "swagger"; // Set Swagger UI to /swagger
});

app.MapControllers();
app.Run();
