using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetapp.Code.OAuth
{
    /// <summary>
    /// 表示向SwaggerGen提供的OAuth API中的htt头部字段信息，这会在Swagger创建的API列表界面上显示这些字段
    /// </summary>
    public class OAuthHeaderFilter : IOperationFilter
    {
        // <summary>
        /// 请求
        /// </summary>
        /// <param name="operation">操作</param>
        /// <param name="context">上下文</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "OAUTH-A-HEADER",// app login session id
                In = ParameterLocation.Header,
                //Type = "int",
                Description = "token",
                Required = false // set to false if this is optional
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "OAUTH-B-HEADER",// product id
                In = ParameterLocation.Header,
                //Type = "int",
                Description = "product id",
                Required = false // set to false if this is optional
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "OAUTH-C-HEADER",// clinet id
                In = ParameterLocation.Header,
                //Type = "int",
                Description = "client id",
                Required = false // set to false if this is optional
            });
        }


    }
}
