using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.SMEException
{
    public class InvalidUsernameOrPasswordException : BusinessException
    {
        public InvalidUsernameOrPasswordException()
        {

        }

        public InvalidUsernameOrPasswordException(string message) : base(message)
        {

        }
    }
}
