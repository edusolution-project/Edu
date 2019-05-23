using System;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Text;
using System.Net.Http;
using System.Linq;
using SME.Utils.Common;
using SME.Bussiness.Lib.Validator;
using SME.Bussiness.Lib.Dto.System;
using SME.Bussiness.Lib.Service;
using System.Net;
using SME.API.Controllers;
using System.Net.Http.Formatting;
using NLog;
using SME.Utils.Common.SMEException;
using System.Reflection;
using SME.Bussiness.Lib.Dto.RequestInfo;
using Data.Access.Object.Entities.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SME.API.CustomFilter
{
    public class QuyenDoiTacDongBoFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string username = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToUpper() == GlobalConstants.HEADER_USERNAME.ToUpper()).Value.SingleOrDefault();
            var req = context.ActionArguments["request"];
            if (req == null)
            {
                throw new InvalidDataInRequestException("Request không có dữ liệu");
            }
            // Neu la kieu du lieu nen thi kiem tra tai ham
            if (req is CompressedRequest)
            {
                return;
            }
            using (var dbContext = new SMEEntities())
            {
                PropertyInfo YcIDInfo = req.GetType().GetProperty("YcDdlTruongHocId");
                if (YcIDInfo == null)
                {
                    throw new InvalidDataInRequestException("Request không có ID yêu cầu đồng bộ trường học");
                }
                var objYcTruongID = YcIDInfo.GetValue(req, null);
                if (objYcTruongID == null)
                {
                    throw new InvalidDataInRequestException("Request không có ID yêu cầu đồng bộ trường học");
                }
                long ycDdlTruongHocId;
                if (long.TryParse(objYcTruongID.ToString(), out ycDdlTruongHocId))
                {
                    YcDdlTruongHocService ycService = new YcDdlTruongHocService(dbContext);
                    YC_DDL_TRUONG_HOC yc = ycService.Find(ycDdlTruongHocId);
                    if (yc == null)
                    {
                        throw new InvalidDataInRequestException("ID yêu cầu đồng bộ trường học không tồn tại");
                    }
                    DoiTacService dtService = new DoiTacService(dbContext);
                    HE_THONG_DOI_TAC dt = dtService.FindPartner(username);
                    if (dt.TAT_CA == null || dt.TAT_CA.GetValueOrDefault() == 0)
                    {
                        if (dt == null)
                        {
                            throw new InvalidDataInRequestException("Không tồn tại đối tác");
                        }
                        DoiTacTruongHocService dtThService = new DoiTacTruongHocService(dbContext);
                        if (!dtThService.IsValidTruongHoc(yc.MA_TRUONG_HOC, dt.HE_THONG_DOI_TAC_ID))
                        {
                            throw new NotPermissionException("Không có quyền đồng bộ trường học này");
                        }
                    }
                    //add ycDllTruongHocId to session, to store in log GIAO_DICH
                    //if (HttpContext.Current.Session == null)
                    //{
                       
                    //}
                    //context.Request.Properties.Add("ycDdlTruongHocId", ycDdlTruongHocId);
                }
                else
                {
                    throw new InvalidDataInRequestException("ID yêu cầu đồng bộ trường học không hợp lệ");
                }
            }
        }
    }
}