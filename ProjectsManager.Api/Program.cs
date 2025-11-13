using ProjectsManager.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDevCors(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
    {
        options.Path = "/swagger/v1/swagger.json";
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

if (app.Environment.IsDevelopment())
    app.UseCors("Dev");

app.MapControllers();

app.Run();