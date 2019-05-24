
using Newtonsoft.Json;
using SME.Utils.Common.SMEException;
using SME.API.CustomFilter;
using System;
using NLog;
using Microsoft.AspNetCore.Mvc;
using Data.Access.Object.Entities.Model;

using System.Collections.Generic;
using BaseMongoDB.Database;

using Business.Dto.Form;

namespace SME.API.Controllers
{

    //[ApiController]
    // [SMEExceptionFilter]
    public class AccountController : ControllerBase
    {



        CPUserSubService _userService;
        AccessTokenService _accessTokenService;
        public AccountController(
        CPUserSubService userService, AccessTokenService accessTokenService
       )
        {
            _userService = userService;
            _accessTokenService = accessTokenService;
        }

        [HttpPost]
        public IEnumerable<CPUserSubEntity> getSubUser([FromBody]AuthenticationForm form)
        {

            return _userService.GetAll();
        }


    }
}
