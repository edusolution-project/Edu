#pragma checksum "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\Shared\_Partial.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "66cd9ff089da4b857b7a67a61af159c33c32a4c2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__Partial), @"mvc.1.0.view", @"/Views/Shared/_Partial.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shared/_Partial.cshtml", typeof(AspNetCore.Views_Shared__Partial))]
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
#line 1 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB;

#line default
#line hidden
#line 2 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"66cd9ff089da4b857b7a67a61af159c33c32a4c2", @"/Views/Shared/_Partial.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2ff09157a95492200d69974765a25b7d7feaa1c", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__Partial : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\Shared\_Partial.cshtml"
   
    var data = Model;

#line default
#line hidden
            BeginContext(31, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 5 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\Shared\_Partial.cshtml"
   
    if(data != null)
    {

#line default
#line hidden
            BeginContext(67, 11, true);
            WriteLiteral("        <p>");
            EndContext();
            BeginContext(79, 10, false);
#line 8 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\Shared\_Partial.cshtml"
      Write(data.Title);

#line default
#line hidden
            EndContext();
            BeginContext(89, 6, true);
            WriteLiteral("</p>\r\n");
            EndContext();
#line 9 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\Shared\_Partial.cshtml"
    }

#line default
#line hidden
            BeginContext(105, 2, true);
            WriteLiteral("\r\n");
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
