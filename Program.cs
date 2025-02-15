using GrpcSample.Services.v1;
using GrpcSample.Interceptors;
using GrpcSample.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc(options => {
    
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<ExceptionInterceptor>();
});
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<ProtoFileProvider>();

builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(5000);
    options.ListenAnyIP(5001, listenOptions => {
        listenOptions.UseHttps();
    });
});


var app = builder.Build();

app.MapGrpcReflectionService();


app.MapGrpcService<PersonGrpcService>();




app.MapGet("/protos", (ProtoFileProvider protoFileProvider) => {

    return Results.Ok(protoFileProvider.GetAllProtoFiles());
});

app.MapGet("/protos/v{version:int}/{protoName}", (ProtoFileProvider protoFileProvider, int version, string protoName) => {

    string filePath = protoFileProvider.GetPath(version, protoName);

    if (string.IsNullOrEmpty(filePath)) {

        return Results.NotFound();
    }

    return Results.File(filePath);
});

app.MapGet("/protos/v{version:int}/{protoName}/view", async (ProtoFileProvider protoFileProvider, int version, string protoName) => {

    string fileContent = await protoFileProvider.GetContent(version, protoName);

    if (string.IsNullOrEmpty(fileContent)) {

        return Results.NotFound();
    }

    return Results.Text(fileContent);
});

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseHttpsRedirection();
app.Run();
