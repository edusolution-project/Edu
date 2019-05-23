using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class BaseException : Exception
    {
        public CustomResponseObject ResponseObject;
    }
}
