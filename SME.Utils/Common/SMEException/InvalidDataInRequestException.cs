using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class InvalidDataInRequestException : BusinessException
    {
        public InvalidDataInRequestException()
        {

        }

        public InvalidDataInRequestException(string message) : base(message)
        {

        }
    }
}
