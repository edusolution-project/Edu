#pragma checksum "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4c2e34016a9ac52fc03429b187fecb4bd27ffaf3"
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
#line 1 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB;

#line default
#line hidden
#line 2 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB.Models;

#line default
#line hidden
#line 1 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
using BaseMVC.Models;

#line default
#line hidden
#line 2 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
using BaseMVC.Globals;

#line default
#line hidden
#line 3 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
using BaseMongoDB.Database;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4c2e34016a9ac52fc03429b187fecb4bd27ffaf3", @"/Views/CPLang/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2ff09157a95492200d69974765a25b7d7feaa1c", @"/Views/_ViewImports.cshtml")]
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
#line 4 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
  
    ViewData["Title"] = "Quản lý ngôn ngữ";
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var data = ViewBag.Data as List<CPLangEntity>;

    var model = ViewBag.Model as DefaultModel;
    string SearchText = string.IsNullOrEmpty(model.SearchText) ? string.Empty : model.SearchText;

#line default
#line hidden
            BeginContext(387, 130, true);
            WriteLiteral("<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n\r\n        <div id=\"message\" style=\"display:none; opacity:0; pointer-events:none\"> ");
            EndContext();
            BeginContext(518, 15, false);
#line 15 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                                                           Write(ViewBag.Message);

#line default
#line hidden
            EndContext();
            BeginContext(533, 51, true);
            WriteLiteral(" </div>\r\n\r\n        <div class=\"card\">\r\n            ");
            EndContext();
            BeginContext(584, 5683, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "4c2e34016a9ac52fc03429b187fecb4bd27ffaf36330", async() => {
                BeginContext(685, 18, true);
                WriteLiteral("\r\n                ");
                EndContext();
                BeginContext(704, 23, false);
#line 19 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
           Write(Html.AntiForgeryToken());

#line default
#line hidden
                EndContext();
                BeginContext(727, 1936, true);
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
                BeginContext(2664, 113, false);
#line 53 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                           Write(DynamicViews.GetCommand("create| add,active|duyệt,nonactive|bỏ duyệt,export| export,delete|xóa,clear| xóa cache"));

#line default
#line hidden
                EndContext();
                BeginContext(2777, 1387, true);
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
#line 80 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                  
                                    if (data != null)
                                    {
                                        int count = data.Count;
                                        for (int i = 0; i < count; i++)
                                        {
                                            var item = data[i];

#line default
#line hidden
                BeginContext(4540, 389, true);
                WriteLiteral(@"                                            <tr>
                                                <td align=""center"">
                                                    <div class=""form-check"">
                                                        <label class=""form-check-label"">
                                                            <input name=""cid"" class=""form-check-input""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 4929, "\"", 4945, 1);
#line 91 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
WriteAttributeValue("", 4937, item.ID, 4937, 8, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(4946, 523, true);
                WriteLiteral(@" type=""checkbox"" />
                                                            <span class=""form-check-sign"">
                                                                <span class=""check""></span>
                                                            </span>
                                                        </label>
                                                    </div>
                                                </td>
                                                <td align=""center"">");
                EndContext();
                BeginContext(5470, 7, false);
#line 98 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                                              Write(item.ID);

#line default
#line hidden
                EndContext();
                BeginContext(5477, 94, true);
                WriteLiteral("</td>\r\n                                                <td align=\"center\"><a class=\"text-info\"");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 5571, "\"", 5625, 1);
#line 99 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
WriteAttributeValue("", 5578, Url.Action("edit","CPlang",new { id=item.ID }), 5578, 47, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5626, 1, true);
                WriteLiteral(">");
                EndContext();
                BeginContext(5628, 9, false);
#line 99 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                                                                                                                          Write(item.Name);

#line default
#line hidden
                EndContext();
                BeginContext(5637, 78, true);
                WriteLiteral("</a></td>\r\n                                                <td align=\"center\">");
                EndContext();
                BeginContext(5716, 9, false);
#line 100 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                                              Write(item.Code);

#line default
#line hidden
                EndContext();
                BeginContext(5725, 128, true);
                WriteLiteral("</td>\r\n                                                <td align=\"center\">\r\n                                                    ");
                EndContext();
                BeginContext(5854, 45, false);
#line 102 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                               Write(DynamicViews.GetActive(item.ID,item.Activity));

#line default
#line hidden
                EndContext();
                BeginContext(5899, 108, true);
                WriteLiteral("\r\n                                                </td>\r\n                                            </tr>\r\n");
                EndContext();
#line 105 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                                        }
                                    }
                                

#line default
#line hidden
                BeginContext(6124, 136, true);
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
            BeginContext(6267, 40, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");
            EndContext();
#line 117 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
  
    if (ViewBag.Message != null)
    {
        string message = ViewBag.Message;
        

#line default
#line hidden
            DefineSection("Scripts", async() => {
                BeginContext(6420, 263, true);
                WriteLiteral(@"
            <script type=""text/javascript"">
                $(""#message"").notify($(""#message"").html(), {
                    position: 'top left',
                    autoHide: false,
                    clickToHide: true,
                    className : '");
                EndContext();
                BeginContext(6684, 15, false);
#line 127 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
                            Write(ViewBag.TypeMsg);

#line default
#line hidden
                EndContext();
                BeginContext(6699, 55, true);
                WriteLiteral("\'\r\n                });\r\n            </script>\r\n        ");
                EndContext();
            }
            );
#line 130 "D:\Users\Hoang Thai Long\Source\Repos\hoang-thai-long\iShare\AdminPage_MongoDb\Views\CPLang\Index.cshtml"
         
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
