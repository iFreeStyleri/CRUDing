using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Common.Response
{
    public interface IBaseResponse<TData> where TData : class
    {
        public HttpStatusCode Code { get; }
        public TData Data { get; }
        public string Message { get; set; }
    }
}
