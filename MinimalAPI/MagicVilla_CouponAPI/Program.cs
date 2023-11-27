using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/couopon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all coupons.");

    return Results.Ok(CouponStore.couponList);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200).Produces(400);

app.MapGet("/api/couopon{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(c => c.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(200).Produces(400);

app.MapPost("/api/coupon", ([FromBody] CouponCreateDto couponCreateDto, IMapper _mapper, 
    IValidator<CouponCreateDto> _validation) =>
{
    var validationResult = _validation.ValidateAsync(couponCreateDto).GetAwaiter().GetResult();
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }
    if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == couponCreateDto.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon Name Already Exists.");
    }

    Coupon coupon = _mapper.Map<Coupon>(couponCreateDto);

    coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(coupon);
    CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

    //return Results.Created($"/api/coupon{coupon.Id}", coupon);

    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDto);
}).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<CouponDto>(201).Produces(400);

app.MapPut("/api/coupon", () =>
{

});

app.MapDelete("/api/coupon{id:int}", (int id) =>
{

});

app.UseHttpsRedirection();

app.Run();
