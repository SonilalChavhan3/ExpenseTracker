using ExpenseTracker;
using ExpenseTracker.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// configure logging (log4net)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddLog4Net("log4net.config");

// Add services to the container.//



builder.Services.AddControllers();

// Register EF Core DbContext for SQL Server using connection string from configuration
builder.Services.AddDbContext<ExpenseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<ExpenseTracker.Services.IExpenseService, ExpenseTracker.Services.ExpenseService>();
builder.Services.AddScoped<ExpenseTracker.Services.ICategoryService, ExpenseTracker.Services.CategoryService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Expense Tracker API",
        Description = "An ASP.NET Core Web API for managing expenses and categories",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }

    });

    //// using System.Reflection;
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    else
    {
        // If you have a logger available, log a warning; otherwise leave silent
        // logger?.LogWarning("XML documentation file not found: {XmlPath}", xmlPath);
    }
    // Require headers on all operations in Swagger UI
    options.OperationFilter<RequiredHeadersOperationFilter>();
});

//Build your appwith builder
var app = builder.Build();

// Correlation and response header middleware
app.Use(async (context, next) =>
{
    // Accept an incoming correlation id or generate one
    var correlationId = context.Request.Headers.ContainsKey("X-Correlation-Id")
        ? context.Request.Headers["X-Correlation-Id"].ToString()
        : Guid.NewGuid().ToString();

    // Caller id from incoming request (optional)
    var callerId = context.Request.Headers.ContainsKey("X-Caller-Id")
        ? context.Request.Headers["X-Caller-Id"].ToString()
        : string.Empty;

    // Timestamp from incoming request or generate current UTC timestamp
    var requestTimestamp = context.Request.Headers.ContainsKey("X-Timestamp")
        ? context.Request.Headers["X-Timestamp"].ToString()
        : DateTime.UtcNow.ToString("o");

    // Ensure request headers contain the values so controllers/endpoints see them
    if (!context.Request.Headers.ContainsKey("X-Correlation-Id"))
    {
        context.Request.Headers["X-Correlation-Id"] = correlationId;
    }
    if (!context.Request.Headers.ContainsKey("X-Caller-Id"))
    {
        // If no caller id was provided, set to empty string so header exists
        context.Request.Headers["X-Caller-Id"] = callerId;
    }
    if (!context.Request.Headers.ContainsKey("X-Timestamp"))
    {
        context.Request.Headers["X-Timestamp"] = requestTimestamp;
    }

    // Store for downstream consumption
    context.Items["CorrelationId"] = correlationId;
    context.Items["CallerId"] = callerId;

    // Add headers just before the response is sent
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        // Echo the request timestamp back in the response
        context.Response.Headers["X-Timestamp"] = requestTimestamp;
        if (!string.IsNullOrEmpty(callerId)) context.Response.Headers["X-Caller-Id"] = callerId;
        return Task.CompletedTask;
    });

    await next();
});

// Request/response logging middleware - single incoming and single outgoing log per request
app.Use(async (context, next) =>
{
    var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
    var logger = loggerFactory?.CreateLogger("RequestLogging") ?? context.RequestServices.GetRequiredService<ILogger<Program>>();

    var correlationId = context.Items["CorrelationId"] as string
        ?? (context.Request.Headers.ContainsKey("X-Correlation-Id") ? context.Request.Headers["X-Correlation-Id"].ToString() : string.Empty);

    var callerId = context.Items["CallerId"] as string
        ?? (context.Request.Headers.ContainsKey("X-Caller-Id") ? context.Request.Headers["X-Caller-Id"].ToString() : string.Empty);

    var requestTimestamp = context.Request.Headers.ContainsKey("X-Timestamp")
        ? context.Request.Headers["X-Timestamp"].ToString()
        : DateTime.UtcNow.ToString("o");

    // Begin a logging scope so downstream logs include correlation info
    using (logger.BeginScope(new Dictionary<string, object>
    {
        ["CorrelationId"] = correlationId,
        ["CallerId"] = callerId
    }))
    {
        var sw = Stopwatch.StartNew();

    // Read and capture request body for logging (only for methods that typically have a body)
    string requestBody = string.Empty;
    try
    {
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put || context.Request.Method == HttpMethods.Patch)
        {
            // Limit size to avoid reading huge bodies into memory
            const long maxReadBytes = 1024 * 100; // 100 KB
            var contentLength = context.Request.ContentLength ?? 0;
            if (contentLength == 0 || contentLength <= maxReadBytes)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                // Truncate long bodies for logging
                const int maxDisplay = 4096; // 4 KB
                if (!string.IsNullOrEmpty(requestBody) && requestBody.Length > maxDisplay)
                {
                    requestBody = requestBody.Substring(0, maxDisplay) + "...(truncated)";
                }
            }
            else
            {
                requestBody = $"(payload too large: {contentLength} bytes)";
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to read request body for logging");
        requestBody = "(unavailable)";
    }

    // Log incoming request once
    logger.LogInformation("Incoming request {Method} {Path} CorrelationId={CorrelationId} CallerId={CallerId} ReqTs={RequestTimestamp} Body={RequestBody}",
        context.Request.Method,
        context.Request.Path,
        correlationId,
        callerId,
        requestTimestamp,
        requestBody);

        try
        {
            await next();
        }
        finally
        {
            sw.Stop();
            var responseTimestamp = DateTime.UtcNow.ToString("o");
            var statusCode = context.Response?.StatusCode;

            // Log outgoing response once
            logger.LogInformation("Outgoing response {Method} {Path} StatusCode={StatusCode} CorrelationId={CorrelationId} CallerId={CallerId} ReqTs={RequestTimestamp} RespTs={ResponseTimestamp} DurationMs={Duration}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                correlationId,
                callerId,
                requestTimestamp,
                responseTimestamp,
                sw.ElapsedMilliseconds);
        }
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler - logs and returns a generic 500 response
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exFeature = context.Features.Get<IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        var correlationId = context.Items["CorrelationId"] as string
            ?? (context.Request.Headers.ContainsKey("X-Correlation-Id") ? context.Request.Headers["X-Correlation-Id"].ToString() : Guid.NewGuid().ToString());

        var callerId = context.Items["CallerId"] as string
            ?? (context.Request.Headers.ContainsKey("X-Caller-Id") ? context.Request.Headers["X-Caller-Id"].ToString() : string.Empty);

        var timestamp = DateTime.UtcNow.ToString("o");

        if (exFeature?.Error != null)
        {
            logger.LogError(exFeature.Error, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);
        }

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        context.Response.Headers["X-Timestamp"] = timestamp;
        if (!string.IsNullOrEmpty(callerId)) context.Response.Headers["X-Caller-Id"] = callerId;

        var errorResponse = new
        {
            error = new
            {
                code = "ERR-500",
                message = "An unexpected error occurred.",
                correlationId,
                timestamp
            }
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
