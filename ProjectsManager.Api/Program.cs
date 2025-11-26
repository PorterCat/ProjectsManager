using ProjectsManager.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDevCors(builder.Configuration);
builder.Services.AddAuthConfiguration(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var adminInitializer = scope.ServiceProvider.GetRequiredService<DefaultAdminInitializer>();
    await adminInitializer.InitializeAsync();
}

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();