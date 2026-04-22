using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExpenseTracker.Swagger
{
    public class RequiredHeadersOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            // Correlation Id
            var corrIdExample = Guid.NewGuid().ToString();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Correlation-Id",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Correlation id for tracing requests",
                Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString(corrIdExample) },
               // Example = new OpenApiString(corrIdExample)
            });

            // Caller Id
            var callerExample = "unknown-client";
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Caller-Id",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Caller id (client identifier)",
                Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString(callerExample) },
               // Example = new OpenApiString(callerExample)
            });

            // Timestamp
            var tsExample = DateTime.UtcNow.ToString("o");
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Timestamp",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Request timestamp in UTC ISO-8601 format",
                Schema = new OpenApiSchema { Type = "string", Format = "date-time", Default = new OpenApiString(tsExample) },
               // Example = new OpenApiString(tsExample)
            });
        }
    }
}
