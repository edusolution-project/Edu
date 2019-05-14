
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using BaseMVC.Globals;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BaseMongoDB.Database;
using MongoDB.Driver;

namespace BaseMVC.MVC
{
    public class ViewPage<TModel> : RazorPage<TModel>
    {
        private readonly SysTemplateService _templateService;
        private readonly SysTemplateDetailService _templateDetailsService;
        private readonly SysTemplatePropertyService _sysTemplateProperty;
        private readonly CPLangEntity _currentLang;
        private readonly SysPageService _pageService;
        private SysPageEntity _pageEntity;
        public ViewPage(SysTemplateService templateService, SysTemplateDetailService templateDetailService
            ,SysPageService pageService, SysTemplatePropertyService sysTemplateProperty)
        {
            _templateService = templateService;
            _templateDetailsService = templateDetailService;
            _currentLang = StartUp.CurrentLang;
            _pageService = pageService;
            _sysTemplateProperty = sysTemplateProperty;
        }
        public TModel PageModel
        {
            get
            {
                return Model;
            }
        }

        public override ViewContext ViewContext {
            get
            {
                return base.ViewContext;
            }
            set => base.ViewContext = value;
        }

        public override TextWriter Output
        {
            get
            {
                return base.Output;
            }
        }
        /// <summary>
        /// Partial View có định như Logo , và Naviation (menu) ... footer
        /// </summary>
        /// <param name="html"></param>
        /// <param name="partialName">ID View - Ex : vswLogo </param>
        /// <returns></returns>
        protected IHtmlContent StaticLayout(IHtmlHelper html, string partialName)
        {
            try
            {
                IgnoreBody();
                var action = new ActionContext(Context, ViewContext.RouteData, ViewContext.ActionDescriptor);
                var controller = new ControllerContext(action);
                _pageEntity = _pageService.GetItemByCtrlandAct(controller.ActionDescriptor.ControllerName, controller.ActionDescriptor.ActionName);

                var template = _templateService.GetByID(_pageEntity.TemplateID);
                if (template == null) return new HtmlString(string.Empty);
                var defaultTemplate = _templateDetailsService.CreateQuery().Find(o => o.TemplateID == template.ID && o.PartialID == partialName && o.IsDynamic == false)?.SingleOrDefault();
                if (defaultTemplate == null) return new HtmlString(string.Empty);
                return DynamicViews.RenderWithData(html, defaultTemplate.PartialView, defaultTemplate.CModule, _sysTemplateProperty.GetItemByParentID(defaultTemplate.ID));
            }
            catch (Exception ex)
            {
                return new HtmlString(ex.ToString());
            }
        }
        /// <summary>
        /// dynamic layout , 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="nameLayout"> id của layout . duy nhất trong 1 template .  ví dụ vswMain </param>
        /// <returns></returns>
        protected IHtmlContent DynamicLayout(IHtmlHelper html,string nameLayout)
        {

            try
            {
                IgnoreBody();
                var action = new ActionContext(Context, ViewContext.RouteData, ViewContext.ActionDescriptor);
                var controller = new ControllerContext(action);

                string keyCache = controller.ActionDescriptor.ActionName + controller.ActionDescriptor.ControllerName + "__View__" + nameLayout;

                var cache = CacheExtends.GetDataFromCache<IHtmlContent>(keyCache);
                if (cache != null) return cache;
                _pageEntity = _pageService.GetItemByCtrlandAct(controller.ActionDescriptor.ControllerName, controller.ActionDescriptor.ActionName);

                var template = _templateService.GetByID(_pageEntity.TemplateID);

                var listDetails = _templateDetailsService.CreateQuery().Find(o => o.TemplateID == template.ID && o.IsDynamic == true && o.ParentID == nameLayout).ToList();
                int limit = listDetails != null ? listDetails.Count : 0;
                var _html = new HtmlContentBuilder();
                for (int i = 0; i < limit; i++)
                {
                    var item = listDetails[i];
                    if (!item.IsBody)
                    {
                        var itemHtml = DynamicViews.RenderWithData(html, item.PartialView, item.CModule, _sysTemplateProperty.GetItemByParentID(item.ID));
                        _html.AppendHtml(itemHtml);
                    }
                    else
                    {
                        _html.AppendHtml(RenderBody());
                    }
                }
                CacheExtends.SetObjectFromCache<IHtmlContent>(keyCache, 240, _html);
                return _html;
            }
            catch(Exception ex)
            {
                return new HtmlString(ex.ToString());
            }
        }
        /// <summary>
        ///  load all, sử dụng cho thằng có 1 layout static ,
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected IHtmlContent DynamicLayout(IHtmlHelper html)
        {
            try
            {
                IgnoreBody();
                var action = new ActionContext(Context, ViewContext.RouteData, ViewContext.ActionDescriptor);
                var controller = new ControllerContext(action);

                string keyCache = controller.ActionDescriptor.ActionName + controller.ActionDescriptor.ControllerName + "__View__ALL";

                var cache = CacheExtends.GetDataFromCache<IHtmlContent>(keyCache);
                if (cache != null) return cache;
                _pageEntity = _pageService.GetItemByCtrlandAct(controller.ActionDescriptor.ControllerName, controller.ActionDescriptor.ActionName);

                var template = _templateService.GetByID(_pageEntity.TemplateID);

                var listDetails = _templateDetailsService.CreateQuery().Find(o => o.TemplateID == template.ID && o.IsDynamic == true).ToList();
                int limit = listDetails != null ? listDetails.Count : 0;
                var _html = new HtmlContentBuilder();
                for (int i = 0; i < limit; i++)
                {
                    var item = listDetails[i];
                    if (!item.IsBody)
                    {
                        var itemHtml = DynamicViews.RenderWithData(html, item.PartialView, item.CModule, _sysTemplateProperty.GetItemByParentID(item.ID));
                        _html.AppendHtml(itemHtml);
                    }
                    else
                    {
                        _html.AppendHtml(RenderBody());
                    }
                }
                CacheExtends.SetObjectFromCache<IHtmlContent>(keyCache, 240, _html);
                return _html;
            }
            catch (Exception ex)
            {
                return new HtmlString(ex.ToString());
            }
        }
        public override ClaimsPrincipal User => base.User;

        public override void BeginContext(int position, int length, bool isLiteral)
        {
            base.BeginContext(position, length, isLiteral);
        }

        public override void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            base.BeginWriteAttribute(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount);
        }

        public override void DefineSection(string name, RenderAsyncDelegate section)
        {
            base.DefineSection(name, section);
        }

        public override void EndContext()
        {
            base.EndContext();
        }

        public override void EndWriteAttribute()
        {
            base.EndWriteAttribute();
        }

        public override void EnsureRenderedBodyOrSections()
        {
            base.EnsureRenderedBodyOrSections();
        }

        public override Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public override Task<HtmlString> FlushAsync()
        {
            return base.FlushAsync();
        }

        public override string Href(string contentPath)
        {
            return base.Href(contentPath);
        }

        public override HtmlString SetAntiforgeryCookieAndHeader()
        {
            return base.SetAntiforgeryCookieAndHeader();
        }

        public override void Write(object value)
        {
            base.Write(value);
        }

        public override void Write(string value)
        {
            base.Write(value);
        }

        public override void WriteLiteral(object value)
        {
            base.WriteLiteral(value);
        }

        public override void WriteLiteral(string value)
        {
            base.WriteLiteral(value);
        }

        protected override TextWriter PopWriter()
        {
            return base.PopWriter();
        }

        protected override void PushWriter(TextWriter writer)
        {
            base.PushWriter(writer);
        }

        protected override IHtmlContent RenderBody()
        {
            return base.RenderBody();
        }
    }
}
