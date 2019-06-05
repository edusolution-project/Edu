using CKSource.CKFinder.Connector.Core;
using CKSource.CKFinder.Connector.Core.Acl;
using CKSource.CKFinder.Connector.Core.Builders;
using CKSource.CKFinder.Connector.Core.ResizedImages;
using CKSource.CKFinder.Connector.Host.Owin;
using CKSource.FileSystem.Local;
using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Owin.Builder;
using Microsoft.Owin.BuilderProperties;
using System.Threading.Tasks;
using Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNet.SignalR;

namespace EntityBaseFW
{
    public static class OwinStart
    {
        public static IApplicationBuilder UseOwinAppBuilder(this IApplicationBuilder app, string pathRoot, string key, string value,string host)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseOwin(setup => setup(next =>
            {
                var connectorBuilder = ConfigureConnector(pathRoot, key, value,host);
                var builder = new AppBuilder();
                var lifetime = (IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));

                var properties = new AppProperties(builder.Properties)
                {
                    AppName = app.ApplicationServices.GetApplicationUniqueIdentifier(),
                    OnAppDisposing = lifetime.ApplicationStopping,
                    DefaultApp = next
                };
                builder.Map("/ckfinder/connector", o => o.UseConnector(connectorBuilder));
                builder.MapSignalR("/hubs", new HubConfiguration());
                return builder.Build<Func<IDictionary<string, object>, Task>>();
            }));
        }
        private static OwinConnector ConfigureConnector(string rootPath, string key, string value,string host)
        {
            var connectorBuilder = new ConnectorBuilder();
            connectorBuilder.SetRequestConfiguration((request, config) =>
            {
                config.AddBackend("CKFinderPrivate", new LocalStorage(rootPath + "/uploads"), null, true);
                config.AddBackend("default", new LocalStorage(rootPath + "/uploads", host+"/uploads"), null, false);
                config.SetCheckDoubleExtension(true);
                config.SetThumbnailBackend("CKFinderPrivate", "thumbs");
                config.AddAclRule(new AclRule(
                    new StringMatcher("*"), new StringMatcher("*"), new StringMatcher("*"),
                    new Dictionary<Permission, PermissionType>
                    {
                         { Permission.FolderView, PermissionType.Allow },
                         { Permission.FolderCreate, PermissionType.Allow },
                         { Permission.FolderRename, PermissionType.Allow },
                         { Permission.FolderDelete, PermissionType.Allow },

                         { Permission.FileView, PermissionType.Allow },
                         { Permission.FileCreate, PermissionType.Allow },
                         { Permission.FileRename, PermissionType.Allow },
                         { Permission.FileDelete, PermissionType.Allow },

                         { Permission.ImageResize, PermissionType.Allow },
                         { Permission.ImageResizeCustom, PermissionType.Allow }
                    }));
                //public SizeAndQuality(int width, int height, ImageQuality quality); ImageQuality(int qualityValue);

                var thumbSize = new SizeAndQuality[]
                {
                    new SizeAndQuality(120, 120, new ImageQuality(80)),
                    new SizeAndQuality(250, 250, new ImageQuality(80)),
                    new SizeAndQuality(300, 300, new ImageQuality(80)),
                    new SizeAndQuality(500, 500, new ImageQuality(80)),
                };
                config.SetThumbnailSizes(thumbSize);

                config.SetMaxImageSize(new Size(1600, 1600));
                config.SetImageResizeThreshold(10, 80);
                config.SetCheckSizeAfterScaling(true);

                var imagesize = new SizeDefinition[]
                {
                    new SizeDefinition("small",480,320,new ImageQuality(80)),
                    new SizeDefinition("medium",600,480,new ImageQuality(80)),
                    new SizeDefinition("large",800,600,new ImageQuality(80)),
                };
                config.SetSizeDefinitions(imagesize);
                config.SetSecureImageUploads(true);
                config.SetDisallowUnsafeCharacters(false);

                //config.AddProxyBackend("local", new LocalStorage(@"MyFiles"));
                config.AddResourceType("Files", resourceBuilder => resourceBuilder.SetBackend("default", "files")
                .SetHideFoldersMatchers(new StringMatcher[] { new StringMatcher("_thumbs"), new StringMatcher("__thumbs"), new StringMatcher("CVS"), new StringMatcher(".*") })
                );
                config.AddResourceType("Documents", resourceBuilder => resourceBuilder.SetBackend("default", "documents")
                .SetAllowedExtensions("7z,aiff,asf,avi,bmp,csv,doc,docx,fla,flv,gif,gz,gzip,jpeg,jpg,mid,mov,mp3,mp4,mpc,mpeg,mpg,ods,odt,pdf,png,ppt,pptx,pxd,qt,ram,rar,rm,rmi,rmvb,rtf,sdc,sitd,swf,sxc,sxw,tar,tgz,tif,tiff,txt,vsd,wav,wma,wmv,xls,xlsx,zip".Split(','))
                //.SetHideFilesMatchers(new StringMatcher[] { new StringMatcher(".*") })
                .SetHideFoldersMatchers(new StringMatcher[] { new StringMatcher("_thumbs"), new StringMatcher("__thumbs"), new StringMatcher("CVS") }));
                config.AddResourceType("Images", resourceBuilder => resourceBuilder.SetBackend("default", "images")
               .SetAllowedExtensions("bmp,gif,jpeg,jpg,png".Split(','))
               .SetHideFoldersMatchers(new StringMatcher[] { new StringMatcher("_thumbs"), new StringMatcher("__thumbs") })

               );

            });
            connectorBuilder.SetAuthenticator(new AuthenticatorCustomer("*")).SetLicense(key, value);
            var connect = connectorBuilder.Build(new OwinConnectorFactory());
            // connectorBuilder.licenseProvider.SetLicense("engcoo.vn", "W3N3FU2L7E11G871SYL1ARS4162D9");
            return connect;
        }
    }
}
