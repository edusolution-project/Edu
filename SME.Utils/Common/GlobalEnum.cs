using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public enum GlobalEnum
    {
        ErrorException = 422,
        XssException = 423,
        InvalidUsernameOrPasswordException = 424,
        InvalidAndEnableCaptchaException = 425,
        InvalidCaptchaException = 426,
        InvalidSSOAuthorizeException = 427,
        InvalidSSOUserException = 428,
        ImportErrorException = 430,
    }
}
