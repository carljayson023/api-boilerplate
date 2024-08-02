using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace send.api.Shared.Exceptions
{
    public class ExceptionHandling
    {
        readonly RequestDelegate _Next;

        public ExceptionHandling(RequestDelegate next)
        {
            _Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            // Push the user name into the log context so that it is included in all log entries
            var claims = ((ClaimsIdentity)httpContext.User.Identity).Claims.ToDictionary(x => x.Type, x => x.Value);
            if (claims.ContainsKey("preferred_username"))
            {
                LogContext.PushProperty("UserName", claims["preferred_username"]);
            }

            // Getting the request body is a little tricky because it's a stream
            // So, we need to read the stream and then rewind it back to the beginning
            string requestBody = "";
            httpContext.Request.EnableBuffering();
            Stream body = httpContext.Request.Body;
            byte[] buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength)];
            await httpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            requestBody = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            httpContext.Request.Body = body;

            // The reponse body is also a stream so we need to:
            // - hold a reference to the original response body stream
            // - re-point the response body to a new memory stream
            // - read the response body after the request is handled into our memory stream
            // - copy the response in the memory stream out to the original response stream
            using var responseBodyMemoryStream = new MemoryStream();

            var originalResponseBodyReference = httpContext.Response.Body;
            httpContext.Response.Body = responseBodyMemoryStream;

            try
            {
                await _Next(httpContext);

                await HandleFailedAuthorizationAsync(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            Log.ForContext("RequestBody", requestBody)
               .ForContext("RequestHeader", httpContext.Request.Headers)
               .ForContext("ResponseBody", responseBody)
               .ForContext("ResponseHeader", httpContext.Response.Headers)
               .Debug("Request and response information {RequestMethod} {RequestPath} {statusCode}", httpContext.Request.Method, httpContext.Request.Path, httpContext.Response.StatusCode);

            await responseBodyMemoryStream.CopyToAsync(originalResponseBodyReference);
        }

        #region Authorization Response Handler

        private Task HandleFailedAuthorizationAsync(HttpContext context)
        {
            ProblemDetails problemDetails = null;

            if (context.Response.StatusCode == 403)
            {
                problemDetails = Generate403ProblemDetails(context);

                Log.Warning("Forbidden request information {RequestMethod} {RequestPath} {StatusCode} {UserInformation}", context.Request.Method, context.Request.Path, context.Response.StatusCode, context.User);
            }
            else if (context.Response.StatusCode == 401)
            {
                problemDetails = Generate401ProblemDetails(context);

                Log.Warning("Forbidden request information {RequestMethod} {RequestPath} {StatusCode} {UserInformation}", context.Request.Method, context.Request.Path, context.Response.StatusCode, context.User);
            }

            if (problemDetails != null)
            {
                context.Response.ContentType = "application/problem+json";

                var result = JsonConvert.SerializeObject(problemDetails, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy(true, true)
                    }
                });

                return context.Response.WriteAsync(result);
            }
            else
                return Task.CompletedTask;
        }

        private ProblemDetails Generate403ProblemDetails(HttpContext httpContext)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Title = "Invalid privilege.",
                Detail = "You do not have the proper privilege to access this resource.",
                Instance = httpContext.Request.Path,
            };

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        private ProblemDetails Generate401ProblemDetails(HttpContext httpContext)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Access denied.",
                Detail = "You do not have the permission to access this resouce.",
                Instance = httpContext.Request.Path,
            };

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        #endregion

        #region Exception Response Handler

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ProblemDetails problemDetails;

            if (ex is ValidationException validation)
            {
                problemDetails = Generate400ProblemDetails(context, validation);

                Log.Error(ex, "Bad request information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            else if (ex is IdNotFoundException notFound)
            {
                problemDetails = Generate404ProblemDetails(context, notFound);

                Log.Error(ex, "Not found request information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            else if (ex is KeyNotFoundException keyNotFound)
            {
                problemDetails = Generate400ProblemDetails(context, keyNotFound);

                Log.Error(ex, "Bad request information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            else if (ex is DomainException conflict)
            {
                problemDetails = Generate409ProblemDetails(context, conflict);

                Log.Error(ex, "Conflict request information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            else if (ex is InvalidUserAccessException)
            {
                problemDetails = Generate403ProblemDetails(context);

                Log.Warning("Forbidden request information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            else if (string.Equals(ex.Message, "Unsupported media type.", StringComparison.OrdinalIgnoreCase))
            {
                problemDetails = Generate500ProblemDetails(context);

                Log.Fatal(ex, "Unhandled exception information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            else // Unhandled
            {
                problemDetails = Generate500ProblemDetails(context);

                Log.Fatal(ex, "Unhandled exception information {RequestMethod} {RequestPath} {statusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)problemDetails.Status;

            var result = JsonConvert.SerializeObject(problemDetails, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy(true, true)
                }
            });

            return context.Response.WriteAsync(result);
        }

        private ProblemDetails Generate400ProblemDetails(HttpContext httpContext, ValidationException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = exception.Message,
                Instance = httpContext.Request.Path,
            };

            problemDetails.Extensions.Add("Errors", exception.Errors);

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        private ProblemDetails Generate400ProblemDetails(HttpContext httpContext, KeyNotFoundException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = exception.Message,
                Instance = httpContext.Request.Path,
            };

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        private ProblemDetails Generate409ProblemDetails(HttpContext httpContext, DomainException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = exception.Message,
                Instance = httpContext.Request.Path,
            };
            if (exception.Detail != null) problemDetails.Detail = exception.Detail;
            problemDetails.Extensions.Add("Code", exception.Code.ToString());

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        private ProblemDetails Generate404ProblemDetails(HttpContext httpContext, IdNotFoundException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = exception.Message,
                Instance = httpContext.Request.Path,
            };

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        private ProblemDetails Generate500ProblemDetails(HttpContext httpContext)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An unhandled exception has occurred while executing the request.",
                Instance = httpContext.Request.Path,
            };

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null) problemDetails.Extensions["TraceId"] = traceId;

            return problemDetails;
        }

        #endregion
    }
}
