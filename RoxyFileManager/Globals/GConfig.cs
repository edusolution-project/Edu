using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileManagerCore.Globals
{
    public class GConfig
    {
        private readonly IConfiguration _configuration;
        public GConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string FILES_ROOT { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___FILES_ROOT}")?.Value; } }
        public string RETURN_URL_PREFIX { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___RETURN_URL_PREFIX}")?.Value; } }
        public string SESSION_PATH_KEY { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___SESSION_PATH_KEY}")?.Value; } }
        public string THUMBS_VIEW_WIDTH { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___THUMBS_VIEW_WIDTH}")?.Value; } }
        public string THUMBS_VIEW_HEIGHT { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___THUMBS_VIEW_HEIGHT}")?.Value; } }
        public string PREVIEW_THUMB_WIDTH { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___PREVIEW_THUMB_WIDTH}")?.Value; } }
        public string PREVIEW_THUMB_HEIGHT { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___PREVIEW_THUMB_HEIGHT}")?.Value; } }
        public string MAX_IMAGE_WIDTH { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___MAX_IMAGE_WIDTH}")?.Value; } }
        public string MAX_IMAGE_HEIGHT { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___MAX_IMAGE_HEIGHT}")?.Value; } }
        public string INTEGRATION { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___INTEGRATION}")?.Value; } }
        public string DIRLIST { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DIRLIST}")?.Value; } }
        public string CREATEDIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___CREATEDIR}")?.Value; } }
        public string DELETEDIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DELETEDIR}")?.Value; } }
        public string MOVEDIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___MOVEDIR}")?.Value; } }
        public string COPYDIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___COPYDIR}")?.Value; } }
        public string RENAMEDIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___RENAMEDIR}")?.Value; } }
        public string FILESLIST { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___FILESLIST}")?.Value; } }
        public string UPLOAD { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___UPLOAD}")?.Value; } }
        public string DOWNLOAD { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DOWNLOAD}")?.Value; } }
        public string DOWNLOADDIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DOWNLOADDIR}")?.Value; } }
        public string DELETEFILE { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DELETEFILE}")?.Value; } }
        public string MOVEFILE { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___MOVEFILE}")?.Value; } }
        public string COPYFILE { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___COPYFILE}")?.Value; } }
        public string RENAMEFILE { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___RENAMEFILE}")?.Value; } }
        public string GENERATETHUMB { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___GENERATETHUMB}")?.Value; } }
        public string DEFAULTVIEW { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DEFAULTVIEW}")?.Value; } }
        public string FORBIDDEN_UPLOADS { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___FORBIDDEN_UPLOADS}")?.Value; } }
        public string ALLOWED_UPLOADS { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___ALLOWED_UPLOADS}")?.Value; } }
        public string FILEPERMISSIONS { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___FILEPERMISSIONS}")?.Value; } }
        public string DIRPERMISSIONS { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DIRPERMISSIONS}")?.Value; } }
        public string LANG { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___LANG}")?.Value; } }
        public string DATEFORMAT { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___DATEFORMAT}")?.Value; } }
        public string OPEN_LAST_DIR { get { return _configuration.GetSection($"{GKeyConfiguration.___KEY}:{GKeyConfiguration.___OPEN_LAST_DIR}")?.Value; } }
    }
}
