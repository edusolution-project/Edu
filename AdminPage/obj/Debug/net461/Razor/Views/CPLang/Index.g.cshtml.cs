#pragma checksum "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ad87f9e028cbaf262139bcef310ef261d73576f5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CPLang_Index), @"mvc.1.0.view", @"/Views/CPLang/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/CPLang/Index.cshtml", typeof(AspNetCore.Views_CPLang_Index))]
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
#line 1 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\_ViewImports.cshtml"
using AdminPage;

#line default
#line hidden
#line 2 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\_ViewImports.cshtml"
using AdminPage.Models;

#line default
#line hidden
#line 1 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
using MVCBase.Models;

#line default
#line hidden
#line 2 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
using MVCBase.Globals;

#line default
#line hidden
#line 3 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
using BaseModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ad87f9e028cbaf262139bcef310ef261d73576f5", @"/Views/CPLang/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4d5234e124a21f3bb637bbfdd01c96e07079ca6d", @"/Views/_ViewImports.cshtml")]
    public class Views_CPLang_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", new global::Microsoft.AspNetCore.Html.HtmlString("myform"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Roles", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("application/x-www-form-urlencoded"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 4 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
  
    ViewData["Title"] = "Quản lý ngôn ngữ";
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var data = ViewBag.Data as List<CPLangEntity>;

    var model = ViewBag.Model as DefaultModel;
    string SearchText = string.IsNullOrEmpty(model.SearchText) ? string.Empty : model.SearchText;

#line default
#line hidden
            BeginContext(377, 130, true);
            WriteLiteral("<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n\r\n        <div id=\"message\" style=\"display:none; opacity:0; pointer-events:none\"> ");
            EndContext();
            BeginContext(508, 15, false);
#line 15 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                                                           Write(ViewBag.Message);

#line default
#line hidden
            EndContext();
            BeginContext(523, 51, true);
            WriteLiteral(" </div>\r\n\r\n        <div class=\"card\">\r\n            ");
            EndContext();
            BeginContext(574, 5683, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "ad87f9e028cbaf262139bcef310ef261d73576f56112", async() => {
                BeginContext(675, 18, true);
                WriteLiteral("\r\n                ");
                EndContext();
                BeginContext(694, 23, false);
#line 19 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
           Write(Html.AntiForgeryToken());

#line default
#line hidden
                EndContext();
                BeginContext(717, 1936, true);
                WriteLiteral(@"
                <input type=""hidden"" id=""ArrID"" name=""ArrID"" value="""" />
                <input type=""hidden"" id=""ctrl"" value=""CPLang"" />

                <script type=""text/javascript"">
                    var Search = [
                        ""filter_Search"", ""SearchText"",
                        ""filter_StartDate"", ""StartDate"",
                        ""filter_EndDate"", ""EndDate"",
                    ];
                    var SearchDefault = [
                        ""1"", ""PageIndex"",
                        ""1"", ""LangID"",
                        ""20"", ""PageSize""
                    ];
                </script>

                <div class=""card-header card-header-image text-right"">
                    <div class=""container-fluid"">
                        <div class=""row"">
                            <div class=""col-sm-12 col-md-6 col-lg-5"">
                                <div class=""row"">
                                    <div class=""col-sm-12 col-md-5"">
                        ");
                WriteLiteral(@"                <input id=""filter_StartDate"" type='text' class=""form-control datetimepicker"" name=""StartDate"" placeholder=""từ ngày"" />
                                    </div>
                                    <div class=""col-sm-12 col-md-5"">
                                        <input id=""filter_EndDate"" type='text' class=""form-control datetimepicker"" name=""EndDate"" placeholder=""đến ngày"" />
                                    </div>
                                    <div class=""col-md-2"">
                                        <a href=""javascript:void(0)"" class=""btn btn-sm btn-primary btn-fab"" onclick=""redirect('Index')""><i class=""material-icons"">search</i></a>
                                    </div>
                                </div>
                            </div>
                            <div class=""col-sm-12 col-md-6 col-lg-7"">
                                ");
                EndContext();
                BeginContext(2654, 113, false);
#line 53 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                           Write(DynamicViews.GetCommand("create| add,active|duyệt,nonactive|bỏ duyệt,export| export,delete|xóa,clear| xóa cache"));

#line default
#line hidden
                EndContext();
                BeginContext(2767, 1387, true);
                WriteLiteral(@"
                            </div>
                        </div>
                    </div>
                </div>
                <div class=""card-body"">
                    <div class=""table-responsive"">
                        <table class=""table table-bordered"">
                            <thead class=""text-light text-center"">
                                <tr>
                                    <th width=""5%"">
                                        <div class=""form-check"">
                                            <label class=""form-check-label"">
                                                <input class=""form-check-input"" type=""checkbox"" onclick=""checkall(this)"" />
                                                <span class=""form-check-sign"">
                                                    <span class=""check""></span>
                                                </span>
                                            </label>
                                        </div>");
                WriteLiteral(@"
                                    </th>
                                    <th>ID</th>
                                    <th>Name</th>
                                    <th>Code</th>
                                    <th>Publish</th>
                                </tr>
                            </thead>
                            <tbody>
");
                EndContext();
#line 80 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                  
                                    if (data != null)
                                    {
                                        int count = data.Count;
                                        for (int i = 0; i < count; i++)
                                        {
                                            var item = data[i];

#line default
#line hidden
                BeginContext(4530, 389, true);
                WriteLiteral(@"                                            <tr>
                                                <td align=""center"">
                                                    <div class=""form-check"">
                                                        <label class=""form-check-label"">
                                                            <input name=""cid"" class=""form-check-input""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 4919, "\"", 4935, 1);
#line 91 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
WriteAttributeValue("", 4927, item.ID, 4927, 8, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(4936, 523, true);
                WriteLiteral(@" type=""checkbox"" />
                                                            <span class=""form-check-sign"">
                                                                <span class=""check""></span>
                                                            </span>
                                                        </label>
                                                    </div>
                                                </td>
                                                <td align=""center"">");
                EndContext();
                BeginContext(5460, 7, false);
#line 98 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                                              Write(item.ID);

#line default
#line hidden
                EndContext();
                BeginContext(5467, 94, true);
                WriteLiteral("</td>\r\n                                                <td align=\"center\"><a class=\"text-info\"");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 5561, "\"", 5615, 1);
#line 99 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
WriteAttributeValue("", 5568, Url.Action("edit","CPlang",new { id=item.ID }), 5568, 47, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5616, 1, true);
                WriteLiteral(">");
                EndContext();
                BeginContext(5618, 9, false);
#line 99 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                                                                                                                          Write(item.Name);

#line default
#line hidden
                EndContext();
                BeginContext(5627, 78, true);
                WriteLiteral("</a></td>\r\n                                                <td align=\"center\">");
                EndContext();
                BeginContext(5706, 9, false);
#line 100 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                                              Write(item.Code);

#line default
#line hidden
                EndContext();
                BeginContext(5715, 128, true);
                WriteLiteral("</td>\r\n                                                <td align=\"center\">\r\n                                                    ");
                EndContext();
                BeginContext(5844, 45, false);
#line 102 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                               Write(DynamicViews.GetActive(item.ID,item.Activity));

#line default
#line hidden
                EndContext();
                BeginContext(5889, 108, true);
                WriteLiteral("\r\n                                                </td>\r\n                                            </tr>\r\n");
                EndContext();
#line 105 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                                        }
                                    }
                                

#line default
#line hidden
                BeginContext(6114, 136, true);
                WriteLiteral("                            </tbody>\r\n                        </table>\r\n                    </div>\r\n                </div>\r\n            ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(6257, 40, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");
            EndContext();
#line 117 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
  
    if (ViewBag.Message != null)
    {
        string message = ViewBag.Message;
        

#line default
#line hidden
            DefineSection("Scripts", async() => {
                BeginContext(6410, 263, true);
                WriteLiteral(@"
            <script type=""text/javascript"">
                $(""#message"").notify($(""#message"").html(), {
                    position: 'top left',
                    autoHide: false,
                    clickToHide: true,
                    className : '");
                EndContext();
                BeginContext(6674, 15, false);
#line 127 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
                            Write(ViewBag.TypeMsg);

#line default
#line hidden
                EndContext();
                BeginContext(6689, 55, true);
                WriteLiteral("\'\r\n                });\r\n            </script>\r\n        ");
                EndContext();
            }
            );
#line 130 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPLang\Index.cshtml"
         
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
