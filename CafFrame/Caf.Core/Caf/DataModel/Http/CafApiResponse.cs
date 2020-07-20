using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DataModel.Http
{
    public class CafApiResponse: ResponseBase
    {
        public CafApiResponse()
        {
            IsSuccess = true;
            Message = "";
        }
        public CafApiResponse(bool isSuccess=true,string message="")
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public CafApiResponse(string message)
        {
            IsSuccess = true;
            Message = message;
        }
    }
    public class CafApiResponse<T> : ResponseBase<T>
    {
        public CafApiResponse()
        {
            IsSuccess = true;
            Message = "";
        }
        public CafApiResponse(T data, bool isSuccess = true, string message = "")
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
        }
        public CafApiResponse(T data, string message)
        {
            Data = data;
            IsSuccess = true;
            Message = message;
        }

        public CafApiResponse(T data)
        {
            Data = data;
            IsSuccess = true;
        }
    }
}
