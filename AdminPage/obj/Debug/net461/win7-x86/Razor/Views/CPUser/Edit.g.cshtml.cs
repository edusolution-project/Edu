#pragma checksum "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0e828ed99ea63003d91003c9d0da5cefc94e70df"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_CPUser_Edit), @"mvc.1.0.view", @"/Views/CPUser/Edit.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/CPUser/Edit.cshtml", typeof(AspNetCore.Views_CPUser_Edit))]
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
#line 1 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\_ViewImports.cshtml"
using AdminPage;

#line default
#line hidden
#line 2 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\_ViewImports.cshtml"
using AdminPage.Models;

#line default
#line hidden
#line 1 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
using MVCBase.Models;

#line default
#line hidden
#line 2 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
using BaseModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0e828ed99ea63003d91003c9d0da5cefc94e70df", @"/Views/CPUser/Edit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4d5234e124a21f3bb637bbfdd01c96e07079ca6d", @"/Views/_ViewImports.cshtml")]
    public class Views_CPUser_Edit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "0", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", new global::Microsoft.AspNetCore.Html.HtmlString("myform"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "cpuser", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("application/x-www-form-urlencoded"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 3 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
  
    Layout = "~/Views/Shared/_LayoutDefaults.cshtml";
    var model = ViewBag.Model as DefaultModel;
    var item = ViewBag.Data as CPUserEntity;
    var data = ViewBag.RoleData as List<CPRoleEntity>;
    if (item == null || model.ID <= 0) { return; }
    int ID = model != null ? model.ID : 0;

#line default
#line hidden
            BeginContext(351, 470, true);
            WriteLiteral(@"
<div class=""card"">
    <div class=""card-header card-header-icon"">
        <h4 class=""card-title"">
            <a href=""javascript:void(0)"" onclick=""redirect('index')"" class=""btn btn-sm btn-fab btn-danger""><i class=""material-icons"">reply</i></a>
            <a href=""javascript:void(0)"" onclick=""excute('edit')"" class=""btn btn-sm btn-success pull-right""><i class=""material-icons"">save</i> Save</a>
        </h4>

    </div>
    <div class=""card-body"">
        ");
            EndContext();
            BeginContext(821, 5060, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0e828ed99ea63003d91003c9d0da5cefc94e70df6558", async() => {
                BeginContext(941, 176, true);
                WriteLiteral("\r\n            <input type=\"hidden\" id=\"ArrID\" name=\"ArrID\" value=\"\" />\r\n            <input type=\"hidden\" id=\"ctrl\" value=\"cpuser\" />\r\n            <input type=\"hidden\" name=\"ID\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1117, "\"", 1128, 1);
#line 24 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 1125, ID, 1125, 3, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1129, 249, true);
                WriteLiteral(" />\r\n            <div class=\"row\">\r\n                <div class=\"col-md-6\">\r\n                    <div class=\"form-group\">\r\n                        <label class=\"bmd-label-floating\">Tên :</label>\r\n                        <input type=\"text\" name=\"Name\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1378, "\"", 1396, 1);
#line 29 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 1386, item.Name, 1386, 10, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1397, 300, true);
                WriteLiteral(@" class=""text-light form-control"" required>
                    </div>
                </div>
                <div class=""col-md-6"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Email :</label>
                        <input type=""email""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1697, "\"", 1716, 1);
#line 35 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 1705, item.Email, 1705, 11, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1717, 285, true);
                WriteLiteral(@" class=""text-light form-control"" disabled>
                    </div>
                </div>
                <div class=""col-md-6"">
                    <label class=""""> Chuyên mục : </label>
                    <select class=""form-control"" name=""RoleID"">
                        ");
                EndContext();
                BeginContext(2002, 31, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0e828ed99ea63003d91003c9d0da5cefc94e70df9248", async() => {
                    BeginContext(2020, 4, true);
                    WriteLiteral("Root");
                    EndContext();
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_0.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(2033, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 42 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                          
                            if (data != null)
                            {
                                int count = data.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    var role = data[i];
                                    if (item.RoleID == role.ID)
                                    {

#line default
#line hidden
                BeginContext(2459, 40, true);
                WriteLiteral("                                        ");
                EndContext();
                BeginContext(2499, 53, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0e828ed99ea63003d91003c9d0da5cefc94e70df11407", async() => {
                    BeginContext(2534, 9, false);
#line 51 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                                                     Write(role.Name);

#line default
#line hidden
                    EndContext();
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#line 51 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                           WriteLiteral(role.ID);

#line default
#line hidden
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                BeginWriteTagHelperAttribute();
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __tagHelperExecutionContext.AddHtmlAttribute("selected", Html.Raw(__tagHelperStringValueBuffer), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.Minimized);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(2552, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 52 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                    }
                                    else
                                    {

#line default
#line hidden
                BeginContext(2674, 40, true);
                WriteLiteral("                                        ");
                EndContext();
                BeginContext(2714, 44, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0e828ed99ea63003d91003c9d0da5cefc94e70df14188", async() => {
                    BeginContext(2740, 9, false);
#line 55 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                                            Write(role.Name);

#line default
#line hidden
                    EndContext();
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#line 55 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                           WriteLiteral(role.ID);

#line default
#line hidden
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(2758, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 56 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                    }
                                }
                            }
                        

#line default
#line hidden
                BeginContext(2892, 277, true);
                WriteLiteral(@"                    </select>
                </div>
                <div class=""col-md-6"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">PassWord :</label>
                        <input type=""password"" name=""Pass""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 3169, "\"", 3187, 1);
#line 65 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 3177, item.Pass, 3177, 10, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(3188, 357, true);
                WriteLiteral(@" class=""text-light form-control"" required autocomplete=""off"">
                    </div>
                </div>
                <div class=""col-md-6"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">BirthDay :</label>
                        <input type='text' class=""form-control datetimepicker""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 3545, "\"", 3567, 1);
#line 71 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 3553, item.BirthDay, 3553, 14, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(3568, 114, true);
                WriteLiteral(" name=\"BirthDay\" />\r\n                    </div>\r\n                </div>\r\n                <div class=\"col-md-12\">\r\n");
                EndContext();
#line 75 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                      
                        if (item.Activity)
                        {

#line default
#line hidden
                BeginContext(3777, 82, true);
                WriteLiteral("                            <input type=\"hidden\" name=\"Activity\" value=\"true\" />\r\n");
                EndContext();
#line 79 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                        }
                        else
                        {

#line default
#line hidden
                BeginContext(3943, 83, true);
                WriteLiteral("                            <input type=\"hidden\" name=\"Activity\" value=\"false\" />\r\n");
                EndContext();
#line 83 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                        }
                    

#line default
#line hidden
                BeginContext(4076, 160, true);
                WriteLiteral("\r\n                    <div class=\"form-group\">\r\n                        <div class=\"form-check\">\r\n                            <label class=\"form-check-label\">\r\n");
                EndContext();
#line 89 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                  
                                    if (item.Activity)
                                    {

#line default
#line hidden
                BeginContext(4367, 138, true);
                WriteLiteral("                                        <input class=\"form-check-input\" type=\"checkbox\" onclick=\"thatActive(this,\'Activity\')\" checked />\r\n");
                EndContext();
#line 93 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                    }
                                    else
                                    {

#line default
#line hidden
                BeginContext(4625, 130, true);
                WriteLiteral("                                        <input class=\"form-check-input\" type=\"checkbox\" onclick=\"thatActive(this,\'Activity\')\" />\r\n");
                EndContext();
#line 97 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
                                    }
                                

#line default
#line hidden
                BeginContext(4829, 507, true);
                WriteLiteral(@"                                <span class=""form-check-sign"">
                                    <span class=""check""></span>
                                </span>
                            </label>
                        </div>
                    </div>
                </div>
                <div class=""col-md-6"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Phone :</label>
                        <input type=""tel"" name=""Phone""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 5336, "\"", 5355, 1);
#line 109 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 5344, item.Phone, 5344, 11, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5356, 322, true);
                WriteLiteral(@" class=""text-light form-control"" autocomplete=""off"">
                    </div>
                </div>
                <div class=""col-md-6"">
                    <div class=""form-group"">
                        <label class=""bmd-label-floating"">Skype :</label>
                        <input type=""text"" name=""Skype""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 5678, "\"", 5697, 1);
#line 115 "E:\Dashbroad\Project_True\DashBoard_Final\AdminPage\Views\CPUser\Edit.cshtml"
WriteAttributeValue("", 5686, item.Skype, 5686, 11, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(5698, 176, true);
                WriteLiteral(" class=\"text-light form-control\" autocomplete=\"off\">\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\"clearfix\"></div>\r\n        ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(5881, 20, true);
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
