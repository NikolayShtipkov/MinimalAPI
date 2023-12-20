using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository;
using MagicVilla_CouponAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/couopon", async (ICouponRepository _couponRepo, ILogger<Program> _logger) =>
{
    ApiResponse response = new();

    _logger.Log(LogLevel.Information, "Getting all coupons.");

    response.Result = await _couponRepo.GetAllAsync();
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupons").Produces<ApiResponse>(200).Produces(400);

app.MapGet("/api/couopon{id:int}", async (ICouponRepository _couponRepo, int id) =>
{
    ApiResponse response = new();
    response.Result = await _couponRepo.GetAsync(id);
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupon").Produces<ApiResponse>(200).Produces(400);

app.MapPost("/api/coupon", async ([FromBody] CouponCreateDto couponCreateDto, ICouponRepository _couponRepo, 
    IMapper _mapper, IValidator<CouponCreateDto> _validation) =>
{
    ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(couponCreateDto);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (await _couponRepo.GetAsync(couponCreateDto.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon name already exists.");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponCreateDto);
    coupon.Created = DateTime.Now;
    coupon.LastUpdated = DateTime.Now;

    await _couponRepo.CreateAsync(coupon);
    await _couponRepo.SaveAsync();

    CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

    response.Result = couponDto;
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.Created;

    return Results.Ok(response);

    //return Results.Created($"/api/coupon{coupon.Id}", coupon);
    //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDto);
}).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<ApiResponse>(201).Produces(400);

app.MapPut("/api/coupon", async ([FromBody] CouponUpdateDto couponUpdateDto, ICouponRepository _couponRepo, 
    IMapper _mapper, IValidator<CouponUpdateDto> _validation) =>
{
    ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(couponUpdateDto);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    Coupon coupon = await _couponRepo.GetAsync(couponUpdateDto.Id);//FIX put method
    if (coupon == null)
    {
        response.ErrorMessages.Add("Coupon doesn't exist");
        return Results.BadRequest(response);
    }

    await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(couponUpdateDto));
    await _couponRepo.SaveAsync();

    response.Result = _mapper.Map<CouponDto>(await _couponRepo.GetAsync(coupon.Id));
    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDto>("application/json").Produces<ApiResponse>(200).Produces(400);

app.MapDelete("/api/coupon{id:int}", async (ICouponRepository _couponRepo, int id) =>
{
    ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

    Coupon coupon = await _couponRepo.GetAsync(id);
    if (coupon == null)
    {
        response.ErrorMessages.Add("Coupon doesn't exist");
        return Results.BadRequest(response);
    }

    await _couponRepo.RemoveAsync(coupon);
    await _couponRepo.SaveAsync();

    response.isSuccessful = true;
    response.StatusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);
}).WithName("DeleteCoupon").Produces<ApiResponse>(204).Produces(400);

app.UseHttpsRedirection();

app.Run();
