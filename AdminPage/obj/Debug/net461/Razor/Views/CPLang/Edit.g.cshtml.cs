#pragma checksum "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "797b75b1c94a85805e217bf84ccacc6f3ca3192e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CPLang_Edit), @"mvc.1.0.view", @"/Views/CPLang/Edit.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/CPLang/Edit.cshtml", typeof(AspNetCore.Views_CPLang_Edit))]
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
#line 1 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\_ViewImports.cshtml"
using AdminPage;

#line default
#line hidden
#line 2 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\_ViewImports.cshtml"
using AdminPage.Models;

#line default
#line hidden
#line 1 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
using MVCBase.Models;

#line default
#line hidden
#line 2 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
using BaseModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"797b75b1c94a85805e217bf84ccacc6f3ca3192e", @"/Views/CPLang/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4d5234e124a21f3bb637bbfdd01c96e07079ca6d", @"/Views/_ViewImports.cshtml")]
    public class Views_CPLang_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", new global::Microsoft.AspNetCore.Html.HtmlString("myform"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Create", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Roles", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("application/x-www-form-urlencoded"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 3 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
  
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var model = ViewBag.Model as DefaultModel;
    var data = ViewBag.Data as CPLangEntity;
    if (data == null || model.ID <= 0) { return; }
    int ID = model != null ? model.ID : 0;

#line default
#line hidden
            BeginContext(295, 116, true);
            WriteLiteral("\r\n<div class=\"card\">\r\n    <div class=\"card-header card-header-icon\">\r\n        <h4 class=\"card-title\">\r\n           <a");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 411, "\"", 438, 1);
#line 14 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 418, Url.Action("Index"), 418, 20, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(439, 299, true);
            WriteLiteral(@" class=""btn btn-sm btn-fab btn-danger""><i class=""material-icons"">reply</i></a>
            <a href=""javascript:void(0)"" onclick=""excute('edit')"" class=""btn btn-sm btn-success pull-right""><i class=""material-icons"">save</i> Save</a>
        </h4>

    </div>
    <div class=""card-body"">
        ");
            EndContext();
            BeginContext(738, 1010, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "797b75b1c94a85805e217bf84ccacc6f3ca3192e6432", async() => {
                BeginContext(859, 52, true);
                WriteLiteral("\r\n            <input type=\"hidden\" id=\"ID\" name=\"ID\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 911, "\"", 922, 1);
#line 21 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 919, ID, 919, 3, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(923, 317, true);
                WriteLiteral(@" />
            <input type=""hidden"" id=""ctrl"" value=""CPlang"" />
            <div class=""row"">
                <div class=""col-md-12"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Tên role :</label>
                        <input type=""text"" name=""Name""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1240, "\"", 1258, 1);
#line 27 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 1248, data.Name, 1248, 10, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1259, 306, true);
                WriteLiteral(@" class=""text-light form-control"">
                    </div>
                </div>
                <div class=""col-md-12"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Tên role :</label>
                        <input type=""text"" name=""Code""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1565, "\"", 1583, 1);
#line 33 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 1573, data.Code, 1573, 10, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1584, 157, true);
                WriteLiteral(" class=\"text-light form-control\">\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\"clearfix\"></div>\r\n        ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1748, 20, true);
            WriteLiteral("\r\n    </div>\r\n</div>");
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
