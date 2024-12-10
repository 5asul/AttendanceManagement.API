using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AttendanceManagement.API.Services
{
    public class DateOnlySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(DateOnly))
            {
                schema.Type = "string";
                schema.Format = "date";
                schema.Example = new OpenApiString("2023-12-10");
            }
        }
    }
}
