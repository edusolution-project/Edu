#pragma checksum "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ed1e5bcb0582eeef0eac1f1eae5124cf57e3f8c9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CPUser_Index), @"mvc.1.0.view", @"/Views/CPUser/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/CPUser/Index.cshtml", typeof(AspNetCore.Views_CPUser_Index))]
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
#line 1 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
using BaseModels;

#line default
#line hidden
#line 2 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
using MVCBase.Models;

#line default
#line hidden
#line 3 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
using MVCBase.Globals;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ed1e5bcb0582eeef0eac1f1eae5124cf57e3f8c9", @"/Views/CPUser/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4d5234e124a21f3bb637bbfdd01c96e07079ca6d", @"/Views/_ViewImports.cshtml")]
    public class Views_CPUser_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", new global::Microsoft.AspNetCore.Html.HtmlString("myform"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "cpuser", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 4 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
  
    ViewData["Title"] = "Quản lý người dùng";
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var data = ViewBag.Data as List<CPUserEntity>;
    var model = ViewBag.Model as DefaultModel;
    var role = new CPRoleService();
    string SearchText = string.IsNullOrEmpty(model.SearchText) ? string.Empty : model.SearchText;

#line default
#line hidden
            BeginContext(411, 130, true);
            WriteLiteral("<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n\r\n        <div id=\"message\" style=\"display:none; opacity:0; pointer-events:none\"> ");
            EndContext();
            BeginContext(542, 15, false);
#line 15 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                                                           Write(ViewBag.Message);

#line default
#line hidden
            EndContext();
            BeginContext(557, 51, true);
            WriteLiteral(" </div>\r\n\r\n        <div class=\"card\">\r\n            ");
            EndContext();
            BeginContext(608, 6803, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "ed1e5bcb0582eeef0eac1f1eae5124cf57e3f8c96150", async() => {
                BeginContext(710, 18, true);
                WriteLiteral("\r\n                ");
                EndContext();
                BeginContext(729, 23, false);
#line 19 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
           Write(Html.AntiForgeryToken());

#line default
#line hidden
                EndContext();
                BeginContext(752, 1936, true);
                WriteLiteral(@"
                <input type=""hidden"" id=""ArrID"" name=""ArrID"" value="""" />
                <input type=""hidden"" id=""ctrl"" value=""cpuser"" />

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
                BeginContext(2689, 107, false);
#line 53 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                           Write(DynamicViews.GetCommand("create| add,delete|xóa,export| export,active|duyệt,nonactive|bỏ,clear| xóa cache"));

#line default
#line hidden
                EndContext();
                BeginContext(2796, 1442, true);
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
                                    <th>Name</th>
                                    <th>Quyền</th>
                                    <th>Publish</th>
                                    <th>Create</th>
                                    <th>ID</th>
                                </tr>
                            </thead>
                            <tbody>
");
                EndContext();
#line 81 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                  
                                    if (data != null)
                                    {
                                        int count = data.Count;
                                        for (int i = 0; i < count; i++)
                                        {
                                            var item = data[i];
                                            var rule = role.GetItemByID(item.RoleID);
                                            string nameRole = rule != null ? rule.Name : "chưa có quyền";

#line default
#line hidden
                BeginContext(4808, 389, true);
                WriteLiteral(@"                                            <tr>
                                                <td align=""center"">
                                                    <div class=""form-check"">
                                                        <label class=""form-check-label"">
                                                            <input name=""cid"" class=""form-check-input""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 5197, "\"", 5213, 1);
#line 94 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
WriteAttributeValue("", 5205, item.ID, 5205, 8, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5214, 456, true);
                WriteLiteral(@" type=""checkbox"" />
                                                            <span class=""form-check-sign"">
                                                                <span class=""check""></span>
                                                            </span>
                                                        </label>
                                                    </div>
                                                </td>
");
                EndContext();
                BeginContext(6108, 130, true);
                WriteLiteral("                                                <td align=\"left\">\r\n                                                    <a class=\"\"");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 6238, "\"", 6292, 1);
#line 107 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
WriteAttributeValue("", 6245, Url.Action("edit","cpuser",new { id=item.ID }), 6245, 47, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(6293, 59, true);
                WriteLiteral(">\r\n                                                        ");
                EndContext();
                BeginContext(6353, 9, false);
#line 108 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                                   Write(item.Name);

#line default
#line hidden
                EndContext();
                BeginContext(6362, 182, true);
                WriteLiteral("\r\n                                                    </a>\r\n                                                </td>\r\n                                                <td align=\"center\">");
                EndContext();
                BeginContext(6545, 8, false);
#line 111 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                                              Write(nameRole);

#line default
#line hidden
                EndContext();
                BeginContext(6553, 128, true);
                WriteLiteral("</td>\r\n                                                <td align=\"center\">\r\n                                                    ");
                EndContext();
                BeginContext(6682, 46, false);
#line 113 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                               Write(DynamicViews.GetActive(item.ID, item.Activity));

#line default
#line hidden
                EndContext();
                BeginContext(6728, 178, true);
                WriteLiteral("\r\n                                                </td>\r\n                                                <td align=\"center\">\r\n                                                    ");
                EndContext();
                BeginContext(6907, 54, false);
#line 116 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                               Write(string.Format("{0:dd-MM-yyyy HH:mm:ss}", item.Created));

#line default
#line hidden
                EndContext();
                BeginContext(6961, 124, true);
                WriteLiteral("\r\n                                                </td>\r\n                                                <td align=\"center\">");
                EndContext();
                BeginContext(7086, 7, false);
#line 118 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                                              Write(item.ID);

#line default
#line hidden
                EndContext();
                BeginContext(7093, 58, true);
                WriteLiteral("</td>\r\n                                            </tr>\r\n");
                EndContext();
#line 120 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                                        }
                                    }
                                

#line default
#line hidden
                BeginContext(7268, 136, true);
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
            BeginContext(7411, 40, true);
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n");
            EndContext();
#line 132 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
  
    if (ViewBag.Message != null)
    {
        string message = ViewBag.Message;
        

#line default
#line hidden
            DefineSection("Scripts", async() => {
                BeginContext(7564, 263, true);
                WriteLiteral(@"
            <script type=""text/javascript"">
                $(""#message"").notify($(""#message"").html(), {
                    position: 'top left',
                    autoHide: false,
                    clickToHide: true,
                    className : '");
                EndContext();
                BeginContext(7828, 15, false);
#line 142 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
                            Write(ViewBag.TypeMsg);

#line default
#line hidden
                EndContext();
                BeginContext(7843, 55, true);
                WriteLiteral("\'\r\n                });\r\n            </script>\r\n        ");
                EndContext();
            }
            );
#line 145 "E:\Dashbroad\Project_True\DashBoard_Final_v2\AdminPage\Views\CPUser\Index.cshtml"
         
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
