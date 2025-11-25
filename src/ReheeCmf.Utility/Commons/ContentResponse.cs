using System;
using System.Collections.Generic;
using System.Net;

namespace ReheeCmf.Commons
{
    public abstract class ContentResponse
    {
        public abstract object? ObjContent { get; }
        public abstract Type ContentType { get; }
        public bool? Success { get; set; }
        public HttpStatusCode Status { get; set; }
        public IEnumerable<Error>? Errors { get; set; }
    }

    public class ContentResponse<T> : ContentResponse
    {
        public T? Content { get; set; }

        public override object? ObjContent => Content;
        public override Type ContentType => typeof(T);
    }
}
