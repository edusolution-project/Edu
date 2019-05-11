#pragma checksum "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a8e4fe5805374517b4e08342650fafc8c5d42523"
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
#line 1 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB;

#line default
#line hidden
#line 2 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB.Models;

#line default
#line hidden
#line 1 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
using BaseMVC.Models;

#line default
#line hidden
#line 2 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
using BaseMongoDB.Database;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a8e4fe5805374517b4e08342650fafc8c5d42523", @"/Views/CPLang/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2ff09157a95492200d69974765a25b7d7feaa1c", @"/Views/_ViewImports.cshtml")]
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
#line 3 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
  
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var model = ViewBag.Model as DefaultModel;
    var data = ViewBag.Data as CPLangEntity;
    if (data == null || string.IsNullOrEmpty(model.ID)) { return; }
    string ID = model != null ? model.ID : string.Empty;

#line default
#line hidden
            BeginContext(336, 116, true);
            WriteLiteral("\r\n<div class=\"card\">\r\n    <div class=\"card-header card-header-icon\">\r\n        <h4 class=\"card-title\">\r\n           <a");
            EndContext();
            BeginWriteAttribute("href", " href=\"", 452, "\"", 479, 1);
#line 14 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 459, Url.Action("Index"), 459, 20, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(480, 299, true);
            WriteLiteral(@" class=""btn btn-sm btn-fab btn-danger""><i class=""material-icons"">reply</i></a>
            <a href=""javascript:void(0)"" onclick=""excute('edit')"" class=""btn btn-sm btn-success pull-right""><i class=""material-icons"">save</i> Save</a>
        </h4>

    </div>
    <div class=""card-body"">
        ");
            EndContext();
            BeginContext(779, 1010, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a8e4fe5805374517b4e08342650fafc8c5d425236671", async() => {
                BeginContext(900, 52, true);
                WriteLiteral("\r\n            <input type=\"hidden\" id=\"ID\" name=\"ID\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 952, "\"", 963, 1);
#line 21 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 960, ID, 960, 3, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(964, 317, true);
                WriteLiteral(@" />
            <input type=""hidden"" id=""ctrl"" value=""CPlang"" />
            <div class=""row"">
                <div class=""col-md-12"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Tên role :</label>
                        <input type=""text"" name=""Name""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1281, "\"", 1299, 1);
#line 27 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 1289, data.Name, 1289, 10, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1300, 306, true);
                WriteLiteral(@" class=""text-light form-control"">
                    </div>
                </div>
                <div class=""col-md-12"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Tên role :</label>
                        <input type=""text"" name=""Code""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1606, "\"", 1624, 1);
#line 33 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Edit.cshtml"
WriteAttributeValue("", 1614, data.Code, 1614, 10, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1625, 157, true);
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
            BeginContext(1789, 20, true);
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
