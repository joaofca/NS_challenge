using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CollisionEvents.Api.Configurations.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AddRequiredOperatorIdParameterAttribute : Attribute, IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAttribute =
            context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AddRequiredOperatorIdParameterAttribute>().Any() == true ||
            context.MethodInfo.GetCustomAttributes(true).OfType<AddRequiredOperatorIdParameterAttribute>().Any();

            if (!hasAttribute)
                return;

            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "OperatorId",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
