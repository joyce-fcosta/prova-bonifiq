using Microsoft.EntityFrameworkCore;
using ProvaPub.Interfaces;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Services.Payments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentService, PixPaymentService>();
builder.Services.AddScoped<IPaymentService, CreditCardPaymentService>();
builder.Services.AddScoped<IPaymentService, PayPalPaymentService>();

builder.Services.AddScoped<IRandomService, RandomService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddDbContext<TestDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ctx")));
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
