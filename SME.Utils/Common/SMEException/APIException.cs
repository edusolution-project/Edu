using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class APIException : BaseException
    {
        public APIException()
        {
        }

        public APIException(int ResponseCode, string ResponseMessage, Object data = null)
        {
            this.ResponseObject = new CustomResponseObject(ResponseCode, ResponseMessage, data);
        }
        public APIException(int ResponseCode, string FunctionKey, string ResourceKey, Object data = null)
        {
            this.ResponseObject = new CustomResponseObject(ResponseCode, FunctionKey, ResourceKey, data);
        }
        public APIException(CustomResponseObject _ResponseObject)
        {
            this.ResponseObject = _ResponseObject;
        }
    }
}
