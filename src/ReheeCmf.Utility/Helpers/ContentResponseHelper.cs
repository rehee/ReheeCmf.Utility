using System;
using System.Net;
using ReheeCmf.Commons;

namespace ReheeCmf.Helpers
{
    public static class ContentResponseHelper
    {
        public static void SetContent<T>(this ContentResponse<T> response, object? content, bool? success, HttpStatusCode? code, params Error[] errors)
        {
            response.Success = success;
            
            if (content is T typedContent)
            {
                response.Content = typedContent;
            }
            else if (content == null)
            {
                response.Content = default;
            }
            else
            {
                try
                {
                    response.Content = (T)Convert.ChangeType(content, typeof(T));
                }
                catch
                {
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

        public static void SetSuccess<T>(this ContentResponse<T> response, object? content)
        {
            response.SetContent(content, true, HttpStatusCode.OK);
        }

        public static void SetErrors<T>(this ContentResponse<T> response, HttpStatusCode code, params Error[] errors)
        {
            response.SetContent(null, false, code, errors);
        }
    }
}
