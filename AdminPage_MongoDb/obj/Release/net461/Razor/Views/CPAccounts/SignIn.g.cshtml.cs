#pragma checksum "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0035c7ceaaaa208b3552dfa96cf55ebd2174c741"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CPAccounts_SignIn), @"mvc.1.0.view", @"/Views/CPAccounts/SignIn.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/CPAccounts/SignIn.cshtml", typeof(AspNetCore.Views_CPAccounts_SignIn))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0035c7ceaaaa208b3552dfa96cf55ebd2174c741", @"/Views/CPAccounts/SignIn.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2ff09157a95492200d69974765a25b7d7feaa1c", @"/Views/_ViewImports.cshtml")]
    public class Views_CPAccounts_SignIn : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("register-form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "SignIn", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "cpaccounts", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("login-form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
  
    ViewData["Title"] = "SignIn";
    Layout = "~/Views/Shared/_LayoutLogin.cshtml";

#line default
#line hidden
#line 6 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
  
    if (User != null && User.Identity.IsAuthenticated)
    {

#line default
#line hidden
            BeginContext(163, 437, true);
            WriteLiteral(@"        <div class=""col-md-6"">
            <div class=""alert alert-danger"">
                <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                    <i class=""material-icons"">close</i>
                </button>
                <span>
                    <b> Danger - </b> This is a regular notification made with "".alert-danger""
                </span>
            </div>
        </div>
");
            EndContext();
#line 19 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
    }
    else
    {
        if (!string.IsNullOrEmpty(ViewBag.Message))
        {

#line default
#line hidden
            BeginContext(688, 369, true);
            WriteLiteral(@"            <div class=""col-md-6"" style=""padding-top:10px"">
                <div class=""alert alert-danger"">
                    <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                        <i class=""material-icons"">close</i>
                    </button>
                    <span>
                        <b> Note - </b> ");
            EndContext();
            BeginContext(1058, 15, false);
#line 30 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
                                   Write(ViewBag.Message);

#line default
#line hidden
            EndContext();
            BeginContext(1073, 75, true);
            WriteLiteral("\r\n                    </span>\r\n                </div>\r\n            </div>\r\n");
            EndContext();
#line 34 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
        }

#line default
#line hidden
            BeginContext(1159, 82, true);
            WriteLiteral("        <div class=\"login-page\">\r\n            <div class=\"form\">\r\n                ");
            EndContext();
            BeginContext(1241, 615, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0035c7ceaaaa208b3552dfa96cf55ebd2174c7417488", async() => {
                BeginContext(1354, 22, true);
                WriteLiteral("\r\n                    ");
                EndContext();
                BeginContext(1377, 23, false);
#line 38 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
               Write(Html.AntiForgeryToken());

#line default
#line hidden
                EndContext();
                BeginContext(1400, 449, true);
                WriteLiteral(@"
                    <input type=""text"" placeholder=""name"" name=""Name"" required />
                    <input type=""password"" name=""Pass"" placeholder=""password"" required />
                    <input type=""email"" name=""Email"" placeholder=""email address"" required />
                    <input type=""submit"" class=""button"" value=""Create"" />
                    <p class=""message"">Already registered? <a href=""#"">Sign In</a></p>
                ");
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
#line 37 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Antiforgery = true;

#line default
#line hidden
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-antiforgery", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Antiforgery, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(1856, 18, true);
            WriteLiteral("\r\n                ");
            EndContext();
            BeginContext(1874, 598, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0035c7ceaaaa208b3552dfa96cf55ebd2174c74110956", async() => {
                BeginContext(1961, 22, true);
                WriteLiteral("\r\n                    ");
                EndContext();
                BeginContext(1984, 23, false);
#line 46 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
               Write(Html.AntiForgeryToken());

#line default
#line hidden
                EndContext();
                BeginContext(2007, 148, true);
                WriteLiteral("\r\n                    <input type=\"email\" name=\"username\" placeholder=\"Email\" required />\r\n                    <input type=\"hidden\" name=\"returnurl\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 2155, "\"", 2181, 1);
#line 48 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
WriteAttributeValue("", 2163, ViewBag.ReturnURL, 2163, 18, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(2182, 283, true);
                WriteLiteral(@"/>
                    <input type=""password"" name=""password"" placeholder=""password"" required />
                    <input type=""submit"" class=""button"" value=""Login"" />
                    <p class=""message"">Not registered? <a href=""#"">Create an account</a></p>
                ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(2472, 38, true);
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n");
            EndContext();
#line 55 "F:\Project_Final\DashBoard_Final_v2\iShare\AdminPage_MongoDb\Views\CPAccounts\SignIn.cshtml"
    }

#line default
#line hidden
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
