using Microsoft.EntityFrameworkCore;
using System;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Services;
using Ticketing.DAL.Contracts;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Mapster;
using Ticketing.BAL.Configs;
using NSwag;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options => {
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            Version = "v1",
            Title = "ToDo API",
            Description = "An ASP.NET Core Web API for managing Ticketing items",
            TermsOfService = "https://ticketing.com/terms",
            Contact = new OpenApiContact
            {
                Name = "Example Contact",
                Url = "https://ticketing.com/contact"
            },
            License = new OpenApiLicense
            {
                Name = "Example License",
                Url = "https://ticketing.com/license"
            }
        };
    };
});


builder.Services.RegisterMapsterConfiguration();

builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped(typeof(Repository<>));

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi3();

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
    });
}

app.Run();
public partial class Program { }