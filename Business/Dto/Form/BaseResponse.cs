using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dto.Form
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            Data = new List<T>();
        }
        public long TotalPage { get; set; }
        public List<T> Data { get; set; }
    }
}
