using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class InvalidSSOUserException : BusinessException
    {
        public InvalidSSOUserException()
        {

        }

        public InvalidSSOUserException(string message) : base(message)
        {

        }
    }
}
