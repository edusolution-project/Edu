using FileManagerCore.Interfaces;
using FileManagerCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileManagerCore.Globals
{
    public static class Startup
    {
    /// <summary>
      /// khi add AddRoxyFileManger vào service thì file appsetting nên được ghi đầu đủ keys
      /// "FileManagerCore": {
      ///  "FILES_ROOT": "",
      ///  "RETURN_URL_PREFIX": "",
      ///  "SESSION_PATH_KEY": "",
      ///  "THUMBS_VIEW_WIDTH": "140",
      ///  "THUMBS_VIEW_HEIGHT": "120",
      ///  "PREVIEW_THUMB_WIDTH": "300",
      ///  "PREVIEW_THUMB_HEIGHT": "200",
      ///  "MAX_IMAGE_WIDTH": "1000",
      ///  "MAX_IMAGE_HEIGHT": "1000",
      ///  "INTEGRATION": "custom",
      ///  "DIRLIST": "/RoxyFile/ListDirTree",
      ///  "CREATEDIR": "/RoxyFile/CREATEDIR",
      ///  "DELETEDIR": "/RoxyFile/DELETEDIR",
      ///  "MOVEDIR": "/RoxyFile/MOVEDIR",
      ///  "COPYDIR": "/RoxyFile/COPYDIR",
      ///  "RENAMEDIR": "/RoxyFile/RENAMEDIR",
      ///  "FILESLIST": "/RoxyFile/ListFiles",
      ///  "UPLOAD": "/RoxyFile/upload",
      ///  "DOWNLOAD": "/RoxyFile/DownloadFile",
      ///  "DOWNLOADDIR": "/RoxyFile/DownloadDir",
      ///  "DELETEFILE": "/RoxyFile/DELETEFILE",
      ///  "MOVEFILE": "/RoxyFile/MOVEFILE",
      ///  "COPYFILE": "/RoxyFile/COPYFILE",
      ///  "RENAMEFILE": "RoxyFile/RENAMEFILE",
      ///  "GENERATETHUMB": "/RoxyFile/ShowThumbnail",
      ///  "DEFAULTVIEW": "list",
      ///  "FORBIDDEN_UPLOADS": "zip js jsp jsb mhtml mht xhtml xht php phtml php3 php4 php5 phps shtml jhtml pl sh py cgi exe application gadget hta cpl msc jar vb jse ws wsf wsc wsh ps1 ps2 psc1 psc2 msh msh1 msh2 inf reg scf msp scr dll msi vbs bat com pif cmd vxd cpl htpasswd htaccess",
      ///  "ALLOWED_UPLOADS": "",
      ///  "FILEPERMISSIONS": "0644",
      ///  "DIRPERMISSIONS": "0755",
      ///  "LANG": "auto",
      ///  "DATEFORMAT": "dd/MM/yyyy HH:mm",
      ///  "OPEN_LAST_DIR": "yes"
      ///}
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public static IServiceCollection AddRoxyFileManger(this IServiceCollection service)
        {
            service.AddSingleton<GConfig>();
            service.AddSingleton<IRoxyFilemanHandler, RoxyFilemanHandler>();
            return service;
        }
        public static IApplicationBuilder UseRoxyFileManger(this IApplicationBuilder app)
        {
            
            return app;
        }
    }
}
