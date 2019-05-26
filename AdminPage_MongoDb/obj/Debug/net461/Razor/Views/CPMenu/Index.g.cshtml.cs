#pragma checksum "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "70c1199fcf8194e0d572df182a0f74b21b3bd784"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CPMenu_Index), @"mvc.1.0.view", @"/Views/CPMenu/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/CPMenu/Index.cshtml", typeof(AspNetCore.Views_CPMenu_Index))]
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
#line 1 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB;

#line default
#line hidden
#line 2 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\_ViewImports.cshtml"
using AdminPage_MongoDB.Models;

#line default
#line hidden
#line 1 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
using BaseMongoDB.Database;

#line default
#line hidden
#line 2 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
using BaseMVC.Models;

#line default
#line hidden
#line 3 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
using BaseMVC.Globals;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"70c1199fcf8194e0d572df182a0f74b21b3bd784", @"/Views/CPMenu/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2ff09157a95492200d69974765a25b7d7feaa1c", @"/Views/_ViewImports.cshtml")]
    public class Views_CPMenu_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", new global::Microsoft.AspNetCore.Html.HtmlString("myform"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "cpmenu", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 4 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
  
    ViewData["Title"] = "Quản lý chuyên mục";
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var data = ViewBag.Data as List<CPMenuEntity>;
    var model = ViewBag.Model as DefaultModel;
    string SearchText = string.IsNullOrEmpty(model.SearchText) ? string.Empty : model.SearchText;

#line default
#line hidden
            BeginContext(384, 130, true);
            WriteLiteral("<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n\r\n        <div id=\"message\" style=\"display:none; opacity:0; pointer-events:none\"> ");
            EndContext();
            BeginContext(515, 15, false);
#line 14 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                                                           Write(ViewBag.Message);

#line default
#line hidden
            EndContext();
            BeginContext(530, 51, true);
            WriteLiteral(" </div>\r\n\r\n        <div class=\"card\">\r\n            ");
            EndContext();
            BeginContext(581, 6298, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "70c1199fcf8194e0d572df182a0f74b21b3bd7846115", async() => {
                BeginContext(683, 18, true);
                WriteLiteral("\r\n                ");
                EndContext();
                BeginContext(702, 23, false);
#line 18 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
           Write(Html.AntiForgeryToken());

#line default
#line hidden
                EndContext();
                BeginContext(725, 1936, true);
                WriteLiteral(@"
                <input type=""hidden"" id=""ArrID"" name=""ArrID"" value="""" />
                <input type=""hidden"" id=""ctrl"" value=""cpmenu"" />

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
                BeginContext(2662, 107, false);
#line 52 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                           Write(DynamicViews.GetCommand("create| add,delete|xóa,export| export,active|duyệt,nonactive|bỏ,clear| xóa cache"));

#line default
#line hidden
                EndContext();
                BeginContext(2769, 1489, true);
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
                                        </div>");
                WriteLiteral(@"
                                    </th>
                                    <th>#</th>
                                    <th>Name</th>
                                    <th>Type</th>
                                    <th>Publish</th>
                                    <th>Create</th>
                                    <th>ID</th>
                                </tr>
                            </thead>
                            <tbody>
");
                EndContext();
#line 81 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                  
                                    if (data != null)
                                    {
                                        int count = data.Count;
                                        for (int i = 0; i < count; i++)
                                        {
                                            var item = data[i];

#line default
#line hidden
                BeginContext(4634, 329, true);
                WriteLiteral(@"                                <tr>
                                    <td align=""center"">
                                        <div class=""form-check"">
                                            <label class=""form-check-label"">
                                                <input name=""cid"" class=""form-check-input""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 4963, "\"", 4979, 1);
#line 92 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
WriteAttributeValue("", 4971, item.ID, 4971, 8, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(4980, 531, true);
                WriteLiteral(@" type=""checkbox"" />
                                                <span class=""form-check-sign"">
                                                    <span class=""check""></span>
                                                </span>
                                            </label>
                                        </div>
                                    </td>
                                    <td align=""center"">
                                        <a class=""btn btn-sm btn-primary btn-round p-sm-1""");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 5511, "\"", 5565, 1);
#line 100 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
WriteAttributeValue("", 5518, Url.Action("edit","cpmenu",new { id=item.ID }), 5518, 47, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5566, 278, true);
                WriteLiteral(@">
                                            <i class=""material-icons"">edit</i>
                                        </a>
                                    </td>
                                    <td align=""left"">
                                        <a class=""""");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 5844, "\"", 5903, 1);
#line 105 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
WriteAttributeValue("", 5851, Url.Action("index","cpmenu",new { Record=item.ID }), 5851, 52, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5904, 47, true);
                WriteLiteral(">\r\n                                            ");
                EndContext();
                BeginContext(5952, 9, false);
#line 106 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                       Write(item.Name);

#line default
#line hidden
                EndContext();
                BeginContext(5961, 146, true);
                WriteLiteral("\r\n                                        </a>\r\n                                    </td>\r\n                                    <td align=\"center\">");
                EndContext();
                BeginContext(6108, 9, false);
#line 109 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                                  Write(item.Type);

#line default
#line hidden
                EndContext();
                BeginContext(6117, 104, true);
                WriteLiteral("</td>\r\n                                    <td align=\"center\">\r\n                                        ");
                EndContext();
                BeginContext(6222, 46, false);
#line 111 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                   Write(DynamicViews.GetActive(item.ID, item.Activity));

#line default
#line hidden
                EndContext();
                BeginContext(6268, 142, true);
                WriteLiteral("\r\n                                    </td>\r\n                                    <td align=\"center\">\r\n                                        ");
                EndContext();
                BeginContext(6411, 54, false);
#line 114 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                   Write(string.Format("{0:dd-MM-yyyy HH:mm:ss}", item.Created));

#line default
#line hidden
                EndContext();
                BeginContext(6465, 100, true);
                WriteLiteral("\r\n                                    </td>\r\n                                    <td align=\"center\">");
                EndContext();
                BeginContext(6566, 7, false);
#line 116 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                                  Write(item.ID);

#line default
#line hidden
                EndContext();
                BeginContext(6573, 46, true);
                WriteLiteral("</td>\r\n                                </tr>\r\n");
                EndContext();
#line 118 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                                        }
                                    }
                                

#line default
#line hidden
                BeginContext(6736, 136, true);
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
            BeginContext(6879, 40, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");
            EndContext();
#line 130 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
  
    if (ViewBag.Message != null)
    {
        string message = ViewBag.Message;
        

#line default
#line hidden
            DefineSection("Scripts", async() => {
                BeginContext(7032, 263, true);
                WriteLiteral(@"
            <script type=""text/javascript"">
                $(""#message"").notify($(""#message"").html(), {
                    position: 'top left',
                    autoHide: false,
                    clickToHide: true,
                    className : '");
                EndContext();
                BeginContext(7296, 15, false);
#line 140 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
                            Write(ViewBag.TypeMsg);

#line default
#line hidden
                EndContext();
                BeginContext(7311, 55, true);
                WriteLiteral("\'\r\n                });\r\n            </script>\r\n        ");
                EndContext();
            }
            );
#line 143 "C:\Users\laptop2\source\repos\Edu\AdminPage_MongoDb\Views\CPMenu\Index.cshtml"
         
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
