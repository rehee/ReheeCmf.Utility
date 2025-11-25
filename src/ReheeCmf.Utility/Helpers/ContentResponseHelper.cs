using System;
using System.Linq;
using System.Net;
using System.Reflection;
using ReheeCmf.Commons;

namespace ReheeCmf.Helpers
{
    public static class ContentResponseHelper
    {
        public static void SetContent<T>(this ContentResponse<T> response, T? content, bool? success, HttpStatusCode? code, params Error[] errors)
        {
            response.Success = success;
            response.Content = content;

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
            var responseType = response.GetType();
            
            if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != typeof(ContentResponse<>))
            {
                response.Success = false;
                return;
            }

            var genericArgument = responseType.GetGenericArguments()[0];
            
            try
            {
                // Find the generic SetContent<T> method
                var method = typeof(ContentResponseHelper)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => 
                        m.Name == nameof(SetContent) && 
                        m.IsGenericMethod && 
                        m.GetParameters().Length == 5 &&
                        m.GetParameters()[0].ParameterType.IsGenericType);

                if (method == null)
                {
                    response.Success = false;
                    return;
                }

                method = method.MakeGenericMethod(genericArgument);

                object? convertedContent = null;
                if (content != null)
                {
                    try
                    {
                        if (genericArgument.IsAssignableFrom(content.GetType()))
                        {
                            convertedContent = content;
                        }
                        else
                        {
                            var underlyingType = Nullable.GetUnderlyingType(genericArgument);
                            if (underlyingType != null)
                            {
                                convertedContent = Convert.ChangeType(content, underlyingType);
                            }
                            else
                            {
                                convertedContent = Convert.ChangeType(content, genericArgument);
                            }
                        }
                    }
                    catch
                    {
                        response.Success = false;
                        return;
                    }
                }

                method.Invoke(null, new[] { response, convertedContent, success, code, errors });
            }
            catch
            {
                response.Success = false;
            }
        }

        public static void SetSuccess<T>(this ContentResponse<T> response, T? content)
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
