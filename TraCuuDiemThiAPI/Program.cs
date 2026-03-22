var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

// Lắng nghe trên tất cả IP, port 5000
app.Run("http://0.0.0.0:5000");