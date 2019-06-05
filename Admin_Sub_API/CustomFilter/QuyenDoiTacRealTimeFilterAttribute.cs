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
using Microsoft.AspNetCore.Mvc.Filters;
using Data.Access.Object.Entities.Model;

namespace SME.API.CustomFilter
{
    public class QuyenDoiTacRealTimeFilterAttribute : ActionFilterAttribute
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
                PropertyInfo maTruongHocInfo = req.GetType().GetProperty("MaTruongHoc");
                if (maTruongHocInfo == null)
                {
                    throw new InvalidDataInRequestException("Request không có mã trường học");
                }
                var objMaTruongHoc = maTruongHocInfo.GetValue(req, null);
                if (objMaTruongHoc == null)
                {
                    throw new InvalidDataInRequestException("Request không có mã trường học");
                }
                string maTruongHoc = objMaTruongHoc.ToString();
                if (!string.IsNullOrWhiteSpace(maTruongHoc))
                {
                    DoiTacService dtService = new DoiTacService(dbContext);
                    HE_THONG_DOI_TAC dt = dtService.FindPartner(username);
                    if (dt.TAT_CA == null || dt.TAT_CA.GetValueOrDefault() == 0)
                    {
                        if (dt == null)
                        {
                            throw new InvalidDataInRequestException("Không tồn tại đối tác");
                        }
                        DoiTacTruongHocService dtThService = new DoiTacTruongHocService(dbContext);
                        if (!dtThService.IsValidTruongHoc(maTruongHoc, dt.HE_THONG_DOI_TAC_ID))
                        {
                            throw new NotPermissionException("Không có quyền đồng bộ trường học này");
                        }
                    }
                }
                else
                {
                    throw new InvalidDataInRequestException("Mã trường học rỗng");
                }
            }
        }
    }
}