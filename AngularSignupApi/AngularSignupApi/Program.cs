using AngularSignupApi.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));
});


builder.Services.AddCors(options => options.AddPolicy(name: "NewAngularSignUpApi", policy =>
{
    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();

}));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NewAngularSignUpApi");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
