using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class InvalidCaptchaException : BusinessException
    {
        public InvalidCaptchaException()
        {

        }

        public InvalidCaptchaException(string message) : base(message)
        {

        }
    }
}
