using GrpcSample.Services.v1;
using GrpcSample.Interceptors;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc(options => {
    
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<ExceptionInterceptor>();
});
builder.Services.AddGrpcReflection();


var app = builder.Build();

app.MapGrpcReflectionService();


app.MapGrpcService<PersonGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
