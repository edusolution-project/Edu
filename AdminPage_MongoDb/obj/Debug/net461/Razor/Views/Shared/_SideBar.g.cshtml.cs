#pragma checksum "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\Shared\_SideBar.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a23e9d01add8337864a60490498eafb8234cca68"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__SideBar), @"mvc.1.0.view", @"/Views/Shared/_SideBar.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shared/_SideBar.cshtml", typeof(AspNetCore.Views_Shared__SideBar))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB;

#line default
#line hidden
#line 2 "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a23e9d01add8337864a60490498eafb8234cca68", @"/Views/Shared/_SideBar.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2ff09157a95492200d69974765a25b7d7feaa1c", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__SideBar : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 532, true);
            WriteLiteral(@"<div class=""sidebar"" data-color=""purple"" data-background-color=""black"" data-image=""AdminPage_MongoDB/assets/img/sidebar-2.jpg"">
    <!--
    <img src=""~/assets/img/sidebar-2.jpg"" />
            Tip 1: You can change the color of the sidebar using: data-color=""purple | azure | green | orange | danger""
            Sys,
            Mod,
            Global
            Tip 2: you can also add an image using data-image tag
        -->
    <div class=""logo"">
        <a href=""#"" class=""simple-text logo-normal"">
            ");
            EndContext();
            BeginContext(533, 18, false);
#line 12 "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\Shared\_SideBar.cshtml"
       Write(User.Identity.Name);

#line default
#line hidden
            EndContext();
            BeginContext(551, 183, true);
            WriteLiteral("\r\n        </a>\r\n    </div>\r\n    <div class=\"sidebar-wrapper\">\r\n        <ul class=\"nav\" id=\"loadMenu\">\r\n            <li class=\"nav-item\" id=\"home\">\r\n                <a class=\"nav-link\"");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 734, "\"", 770, 1);
#line 18 "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\Shared\_SideBar.cshtml"
WriteAttributeValue("", 741, Url.Action("Index","CPHome"), 741, 29, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(771, 225, true);
            WriteLiteral(">\r\n                    <i class=\"material-icons\">dashboard</i>\r\n                    <p>Trang chủ</p>\r\n                </a>\r\n            </li>\r\n            <li class=\"nav-item\" id=\"module\">\r\n                <a class=\"nav-link\"");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 996, "\"", 1032, 1);
#line 24 "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\Shared\_SideBar.cshtml"
WriteAttributeValue("", 1003, Url.Action("Index","Module"), 1003, 29, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(1033, 223, true);
            WriteLiteral(">\r\n                    <i class=\"material-icons\">unarchive</i>\r\n                    <p>Quản lý</p>\r\n                </a>\r\n            </li>\r\n            <li class=\"nav-item\" id=\"system\">\r\n                <a class=\"nav-link\"");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 1256, "\"", 1292, 1);
#line 30 "F:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\Shared\_SideBar.cshtml"
WriteAttributeValue("", 1263, Url.Action("Index","System"), 1263, 29, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(1293, 179, true);
            WriteLiteral(">\r\n                    <i class=\"material-icons\">content_paste</i>\r\n                    <p>Hệ thống</p>\r\n                </a>\r\n            </li>\r\n        </ul>\r\n    </div>\r\n</div>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
