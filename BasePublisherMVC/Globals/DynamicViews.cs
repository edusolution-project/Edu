
using BasePublisherModels.Database;
using BasePublisherModels.Factory;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BasePublisherMVC.Globals
{
    public class ObjectModel
    {
        public object Properties { get; set; }
        public dynamic Data { get; set; }
    }
    public static class DynamicViews
    {
        //<div class="mdc-select">
        //    <i class="mdc-select__dropdown-icon"></i>
        //    <select class="mdc-select__native-control">
        //        <option value = "" disabled selected></option>
        //        <option value = "grains" >
        //            Bread, Cereal, Rice, and Pasta
        //        </option>
        //        <option value = "vegetables" >
        //            Vegetables
        //        </ option >
        //        < option value="fruit">
        //            Fruit
        //        </option>
        //    </select>
        //    <label class="mdc-floating-label">Pick a Food Group</label>
        //    <div class="mdc-line-ripple"></div>
        //</div>
        public static IHtmlContent GetMenuForWeb(string type, string menuID, string langID)
        {
            string keyCache = type + "_ChuyenMuc_" + langID;
            var cache = CacheExtends.GetDataFromCache<IHtmlContent>(keyCache);
            if (cache != null) return cache;
            else
            {
                var chuyenmuc = Instance.CreateInstanceCPMenu("CPMenus");
                var data = chuyenmuc.GetItemByType(type, langID);
                string Html = "<div class=\"mdc-select\"><i class=\"mdc-select__dropdown-icon\"></i><select name=\"MenuID\" class=\"mdc-select__native-control\"><option value=\"\">------ Chọn chuyên mục -----</option>";
                for (int i = 0; data != null && i < data.Count; i++)
                {
                    var item = data[i];
                    if (menuID == item.ID)
                    {
                        Html += "<option value =" + item.ID + " selected>" + item.Name + "</option>";
                    }
                    else
                    {
                        Html += "<option value =" + item.ID + ">" + item.Name + "</option>";
                    }
                }
                Html += "</select></div>";
                return new HtmlString(Html);
            }
        }

        /// <summary>
        /// Render html partialView with data from db
        /// </summary>
        /// <param name="partialView"></param
        /// <param name="ctrl"></param>
        /// <param name="properties"> key | value \r\n key | value \r\n  key | value</param>
        /// <returns></returns>

        //public static IHtmlContent RenderWithData(this IHtmlHelper htmlHelper, string partialView,string ctrl,List<SysTemplatePropertyEntity> properties)
        //{
        //    string keyCache = ctrl + "_partialView_"+partialView;
        //    var cache = CacheExtends.GetDataFromCache<IHtmlContent>(keyCache);
        //    if (cache == null)
        //    {
        //        var customer = properties;

        //        var data = GetDynamicDataByActName(ctrl, customer);
        //        var html = htmlHelper.PartialAsync(partialView, data);

        //        CacheExtends.SetObjectFromCache(keyCache, 10 * 60 * 24, html.Result);

        //        return html.Result;
        //    }
        //    else
        //    {
        //        return cache;
        //    }
        //}
        public static Task<HtmlString> RenderTask()
        {
            HtmlString html = new HtmlString("");
            return Task.FromResult(html);
        }
        #region method

        //public static object GetDynamicDataByActName(string ctrlFullName,List<SysTemplatePropertyEntity> customer)
        //{
        //    try
        //    {
        //        // get controller
        //        Type thisType = Type.GetType(ctrlFullName);

        //        if (thisType == null) return null;
        //        // lấy method => mặc định là onload()
        //        MethodInfo theMethod = thisType.GetMethod("OnLoad");
        //        var fields = thisType.GetFields();
        //        if (theMethod == null) return null;
        //        // xử lý controller
        //        object classInstance = null;
        //        if (customer == null || fields == null)
        //        {
        //            classInstance = Activator.CreateInstance(thisType, null);
        //        }
        //        else
        //        {

        //            var parameters = customer;
        //            int count = parameters.Count;
        //            if (fields != null)
        //            {
        //                ParameterInfo[] infos = new ParameterInfo[] { };
        //                dynamic listFiels = new object[count];
        //                int i=0;
        //                foreach (var field in fields)
        //                {
        //                    for(int x = 0; x < count; x++)
        //                    {
        //                        var pa = parameters[x];
        //                        if (pa.Name == field.Name)
        //                        {
        //                            var type = field.FieldType;
        //                            var data = Convert.ChangeType(pa.Value, type);
        //                            listFiels[i] = data;
        //                            break;
        //                        }
        //                        i++;
        //                    }
        //                }
        //                classInstance = Activator.CreateInstance(thisType, listFiels);
        //            }
        //            else
        //            {
        //                classInstance = Activator.CreateInstance(thisType,null);
        //            }

        //        }
        //        return theMethod.Invoke(classInstance, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ObjectModel() { Data = ex,Properties = null };
        //    }
        //}
        private static T ConvertTo<T>(object data)
        {
            return (T)data;
        }

        #endregion
        #region reder html
        public static HtmlString GetCommand(string cmd)
        {
            var arr = cmd.Split(',');
            string html = "<div id=\"command\">";
            foreach (var item in arr)
            {

                if (item == "space")
                {
                    html += "<a class=\"divider\"></a>";
                    continue;
                }
                var _arr = item.Split('|');

                var key = _arr[0];
                var name = _arr[1];
                switch (key.ToLower())
                {
                    case "createlesson":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-success\" onclick=\"formCreate.chooseTemplateLesson()\">" +
                                "<i class=\"material-icons\">add_circle</i>" + name + "<div class=\"ripple-container\"></div>" +
                                "</a>";
                        break;
                    case "create":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-success\" onclick=\"redirect('" + key + "')\">" +
                                "<i class=\"material-icons\">add_circle</i>" + name + "<div class=\"ripple-container\"></div>" +
                                "</a>";
                        break;
                    case "createsub":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-success\" onclick=\"redirectsub('create')\">" +
                                "<i class=\"material-icons\">add_circle</i>" + name + "<div class=\"ripple-container\"></div>" +
                                "</a>";
                        break;
                    case "export":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-primary\" onclick=\"execute('" + key + "')\">" +
                                "<i class=\"material-icons\">airplay</i>" + name + "<div class=\"ripple-container\"></div>" +
                                "</a>";
                        break;
                    case "import":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-info\" onclick=\"redirect('" + key + "')\"><i class=\"material-icons\">add_to_queue</i> import</a>";
                        break;
                    case "delete":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-fab btn-danger\" " +
                            "onclick=\"javascript:if(confirm('Bạn chắc là mình muốn xóa chứ !')){execute('delete')}\"><i class=\"material-icons\">delete_sweep</i></a>";
                        break;
                    case "nonactive":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-fab btn-dark\" onclick=\"execute('" + key + "')\"><i class=\"material-icons\">lock</i></a>";
                        break;
                    case "active":
                        html += "<a href=\"javascript:void(0)\" title=\"" + name + "\" class=\"btn btn-sm btn-fab btn-success\" onclick=\"execute('" + key + "')\"><i class=\"material-icons\">lock_open</i></a>";
                        break;
                    case "clear":
                        html += "<a href=\"javascript:void(0)\" title=" + name + " class=\"btn btn-sm btn-link\" onclick=\"execute('clear')\"><i class=\"material-icons\">block</i> clear</a>";
                        break;
                    default:
                        html += "<a href=\"javascript:void(0);\" onclick=\"execute('" + key + "')\" class=\"btn btn-default btn-" + key.ToLower() + "\"></a>";
                        break;
                }
            }
            html += "</div>";

            return new HtmlString(html);
        }
        public static HtmlString GetActive(string ID, bool active)
        {
            string html = "";

            html = active
                ? "<a href=\"javascript:void(0)\" title=\"bỏ duyệt\" class=\"btn btn-sm btn-fab btn-success\" onclick=\"execute('nonactive','" + ID + "')\"><i class=\"material-icons\">lock_open</i></a>"
                : "<a href=\"javascript:void(0)\" title=\"duyệt\" class=\"btn btn-sm btn-fab btn-dark\" onclick=\"execute('active','" + ID + "')\"><i class=\"material-icons\">lock</i></a>";

            return new HtmlString(html);
        }
        public static HtmlString GetCheckbox(string ID, int index)
        {
            return new HtmlString("<input type=\"checkbox\" id=\"cb" + index + "\" name=\"cid\" value=\"" + ID + "\" onclick=\"isChecked(this.checked);\" />");
        }

        public static HtmlString GetPageList(string action, int TotalRecord, int PageIndex, int PageSize)
        {
            int Count = Arround(TotalRecord, PageSize);
            string html = "<ul class=\"ul_page\">";
            if (TotalRecord > PageSize)
            {
                if (PageIndex > 0)
                {
                    html += "<li><a href=\"javascript:void(0);\" onclick=\"Edit('" + action + "','PageIndex','" + (PageIndex) + "')\">Back</a></li>";
                }
                else
                {
                    html += "<li><a href=\"javascript:void(0);\" style=\"color:#ccc;border:1px solid #ccc; cursor: not-allowed;\">Back</a></li>";
                }
                for (int i = 0; i < Count; i++)
                {
                    string isActive = "";
                    if (PageIndex == i)
                    {
                        isActive = "active";
                    }
                    html += "<li class=" + isActive + "><a href=\"javascript:void(0);\" onclick=\"Edit('" + action + "','PageIndex','" + (i + 1) + "')\">" + (i + 1) + "</a></li>";
                }
                if (PageIndex < Count - 1)
                {
                    html += "<li><a href=\"javascript:void(0);\" onclick=\"Edit('" + action + "','PageIndex','" + (PageIndex + 2) + "')\">Next</a></li>";
                }
                else
                {
                    html += "<li><a href=\"javascript:void(0);\" style=\"color:#ccc;border:1px solid #ccc; cursor: not-allowed;\">Next</a></li>";
                }
            }

            html += "</ul>";

            return new HtmlString(html);
        }

        public static HtmlString GetPageSize(string action, int PageSize)
        {
            List<int> ListSize = new List<int>()
            {
                1,5,10,15,20,25,30,50,100
            };
            string html = "<select class=\"selectSize\" onchange=\"Edit('" + action + "','PageSize',this.value)\" name=\"PageSize\">";
            foreach (int i in ListSize)
            {
                if (i == PageSize)
                {
                    html += "<option value=" + i + " selected>" + i + "</option>";
                }
                else
                {
                    html += "<option value=" + i + ">" + i + "</option>";
                }
            }
            html += "</select>";
            return new HtmlString(html);
        }
        public static int Arround(int a, int b)
        {
            if (a % b > 0)
            {
                return (int)(a / b) + 1;
            }
            else
            {
                return (a / b);
            }
        }

        public static HtmlString GetSelect(this CPLangEntity currentLang, List<CPLangEntity> data)
        {
            if (data == null) return new HtmlString(string.Empty);

            int count = data.Count;
            if (count == 0) return new HtmlString(string.Empty);
            string html = "<div>";
            for (int i = 0; i < count; i++)
            {
                var item = data[i];
                if (currentLang.Code == item.Code)
                {
                    html += "<a href=\"javascript:void(0)\" class=\"btn btn-sm btn-success\">" + item.Name + "</a>";
                }
                else
                {
                    html += "<a href=\"javascript:void(0)\" onclick=\"changeLang('" + item.Code + "')\" class=\"btn btn-sm btn-dark\">" + item.Name + "</a>";
                }
            }
            html += "</div>";
            return new HtmlString(html);
        }
        public static HtmlString GetSelectMini(this CPLangEntity currentLang, List<CPLangEntity> data)
        {
            if (data == null) return new HtmlString(string.Empty);

            int count = data.Count;
            if (count == 0) return new HtmlString(string.Empty);
            string html = "<div>";
            for (int i = 0; i < count; i++)
            {
                var item = data[i];
                if (currentLang != null && currentLang.Code == item.Code)
                {
                    html += "<a href=\"javascript:void(0)\" class=\"btn btn-sm btn-success\" style=\"padding:3px\">" + item.Code.ToUpper() + "</a>";
                }
                else
                {
                    html += "<a href=\"javascript:void(0)\" onclick=\"changeLang('" + item.Code + "')\" class=\"btn btn-sm btn-dark\" style=\"padding:3px\">" + item.Code.ToUpper() + "</a>";
                }
            }
            html += "</div>";
            return new HtmlString(html);
        }
        #endregion

        //get controller
    }
    public class HtmlDynamicRender
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        public HtmlDynamicRender(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }
        public async Task<HtmlString> ToHtmlAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }
                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);

                return new HtmlString(sw.ToString());
            }
        }

        public void Test(string ctrl, string act)
        {
            Type thisType = Type.GetType(ctrl);
            MethodInfo theMethod = thisType.GetMethod(act);
            var data = theMethod.Invoke(this, null);
        }
    }
}
