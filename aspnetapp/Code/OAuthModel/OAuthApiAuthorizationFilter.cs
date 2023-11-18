using aspnetapp.Code.Abstract;
using aspnetapp.Code.ApiModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetapp.Code.OAuth
{
    /// <summary>
    /// 表示接口权限验证类
    /// </summary>
    public class OAuthApiAuthorizationFilter : IAuthorizationFilter
    {
        /// <summary>
        /// OnAuthorization
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata
                .Any(x => x is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
            {
                return;
            }

            var headers = context.HttpContext.Request.Headers;

            if (headers.TryGetValue("OAUTH-A-HEADER", out var bind) == false)
            {
                ApiResult<string> r = new ApiResult<string>();
                r.Error("缺少http header:OAUTH-A-HEADER", ServiceErrorCode.HttpHeaderError);
                context.Result = new JsonResult(r);
                return;
            }

        }
    }
}
