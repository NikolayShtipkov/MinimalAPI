using MagicVilla_CouponAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapGet("/api/couopon", () => 
{
    return Results.Ok(CouponStore.couponList);
});

app.MapGet("/api/couopon{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(c => c.Id == id));
});

app.MapPost("/api/coupon", () =>
{

});

app.MapPut("/api/coupon", () =>
{

});

app.MapDelete("/api/coupon{id:int}", (int id) =>
{

});

app.UseHttpsRedirection();

app.Run();
