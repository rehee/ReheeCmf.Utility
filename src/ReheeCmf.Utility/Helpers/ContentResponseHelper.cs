using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Reflection;
using ReheeCmf.Commons;

namespace ReheeCmf.Helpers
{
    public static class ContentResponseHelper
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> _methodCache = new ConcurrentDictionary<Type, MethodInfo>();

        public static void SetContent<T>(this ContentResponse<T> response, object? content, bool? success, HttpStatusCode? code, params Error[] errors)
        {
            if (content is T typedContent)
            {
                response.Success = success;
                response.Content = typedContent;
            }
            else if (content == null)
            {
                response.Success = success;
                response.Content = default;
            }
            else
            {
                // Try to convert the content
                try
                {
                    var targetType = typeof(T);
                    var underlyingType = Nullable.GetUnderlyingType(targetType);
                    
                    if (underlyingType != null)
                    {
                        response.Content = (T)Convert.ChangeType(content, underlyingType);
                        response.Success = success;
                    }
                    else
                    {
                        response.Content = (T)Convert.ChangeType(content, targetType);
                        response.Success = success;
                    }
                }
                catch
                {
                    response.Success = false;
                    response.Content = default;
                }
            }

            if (code.HasValue)
            {
                response.Status = code.Value;
            }

            if (errors != null && errors.Length > 0)
            {
                response.Errors = errors;
            }
        }

        public static void SetContent(this ContentResponse response, object? content, bool? success, HttpStatusCode? code, params Error[] errors)
        {
            var contentType = response.ContentType;
            
            try
            {
                // Get or create cached method
                var method = _methodCache.GetOrAdd(contentType, type =>
                {
                    var genericMethod = typeof(ContentResponseHelper)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m => 
                            m.Name == nameof(SetContent) && 
                            m.IsGenericMethod && 
                            m.GetParameters().Length == 5 &&
                            m.GetParameters()[0].ParameterType.IsGenericType);

                    return genericMethod?.MakeGenericMethod(type)!;
                });

                if (method == null)
                {
                    response.Success = false;
                    return;
                }

                method.Invoke(null, new[] { response, content, success, code, errors });
            }
            catch
            {
                response.Success = false;
            }
        }

        public static void SetSuccess<T>(this ContentResponse<T> response, object? content)
        {
            response.SetContent(content, true, HttpStatusCode.OK);
        }

        public static void SetSuccess(this ContentResponse response, object? content)
        {
            response.SetContent(content, true, HttpStatusCode.OK);
        }

        public static void SetErrors<T>(this ContentResponse<T> response, HttpStatusCode code, params Error[] errors)
        {
            response.SetContent(default, false, code, errors);
        }

        public static void SetErrors(this ContentResponse response, HttpStatusCode code, params Error[] errors)
        {
            response.SetContent(null, false, code, errors);
        }
    }
}
