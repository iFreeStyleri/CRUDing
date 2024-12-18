using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Common.Response
{
    public class BaseResponse<TEntity> : IBaseResponse<TEntity> where TEntity : class
    {
        public HttpStatusCode Code { get; set; }
        public TEntity Data { get; set; }
        public string Message { get; set; }
    }
}
