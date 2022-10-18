using API.Filters;
using API.Middlewares;
using API.Modules;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWorks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Repositories;
using Repository.UnitOfWorks;
using Service.Mapping;
using Service.Services;
using Service.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//NotFoundFilter
builder.Services.AddScoped(typeof(NotFoundFilter<>));
//fluent validation, custom filter response
builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>());
//custom filter response yaparsak (ValidateFilterAttribute gibi) a�a��daki kod yaz�l�r. Yani fluent validator default davran���n� pasif hale getirmeliyiz ki kendi modelimiz geriye d�ns�n
//MVC taraf�nda bu kodu yazmam�za gerek yoktur. MVC'de bask�lama yap�lmaz, orada direkt aktif olur.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//mapping
builder.Services.AddAutoMapper(typeof(MapProfile));

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);//Repository class library'nin ismini ald�k ��nk� AppDbContext orada.
    });
});

//autoFac
builder.Host.UseServiceProviderFactory
    (new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));//birden fazla mod�l varsa bunun kopyas�n� al ve module ad�n� de�i�tir.
//caching
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomException();//hata mekanizmas� oldu�u i�in yukar�da olmas� �nemli

app.UseAuthorization();

app.MapControllers();

app.Run();
