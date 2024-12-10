using AttendanceManagement.API.Data;
using AttendanceManagement.API.Repository.Implementations;
using AttendanceManagement.API.Repository.Interfaces;
using AttendanceManagement.API.Services;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IAdminRepository, AdminRepository>();
    builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();



    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

    builder.Services.AddSwaggerGen(options =>
    {
        options.SchemaFilter<DateOnlySchemaFilter>();
    });





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

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception: {ex.Message}");
    throw;
}

