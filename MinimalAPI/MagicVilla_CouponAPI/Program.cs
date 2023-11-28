using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
    ApiResponse response = new();

    _logger.Log(LogLevel.Information, "Getting all coupons.");

    response.Result = CouponStore.couponList;
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupons").Produces<ApiResponse>(200).Produces(400);

app.MapGet("/api/couopon{id:int}", (int id) =>
{
    ApiResponse response = new();
    response.Result = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupon").Produces<ApiResponse>(200).Produces(400);

app.MapPost("/api/coupon", async ([FromBody] CouponCreateDto couponCreateDto, IMapper _mapper, 
    IValidator<CouponCreateDto> _validation) =>
{
    ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(couponCreateDto);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (CouponStore.couponList.FirstOrDefault(c => c.Name.ToLower() == couponCreateDto.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon name already exists.");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponCreateDto);

    coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(coupon);
    CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

    response.Result = couponDto;
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.Created;

    return Results.Ok(response);

    //return Results.Created($"/api/coupon{coupon.Id}", coupon);
    //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDto);
}).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<ApiResponse>(201).Produces(400);

app.MapPut("/api/coupon", () =>
{

});

app.MapDelete("/api/coupon{id:int}", (int id) =>
{

});

app.UseHttpsRedirection();

app.Run();
