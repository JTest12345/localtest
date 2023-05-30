using ArmsApi.Model;
using KodaClassLibrary;
using KodaWeb.Models;
using KodaWeb.Models.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace KodaWeb.Controllers.WebApi {

    [RoutePrefix("api/lotinfo")]
    public class LotInfoController : ApiController {

        [HttpGet]
        [Route("18/{lotno}")]
        public HttpResponseMessage Get_from_LotNo18(string lotno) {

            var res = new LotInfoResponse();

            try {
                //ロット情報取得
                ApiLotInfo lotinfo = ApiLotInfo.Get_LotInfo_from_LotNo18(lotno);
                res.LotInfo = lotinfo;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (NotNewSystemException) {
                res.ErrorMessage = "対象ロットはシステムにありません。";
                res.OnlyJissekiSystem = true;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (FjhSystemException fs_ex) {
                res.ErrorMessage = fs_ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (ArmsSystemException as_ex) {
                res.ErrorMessage = as_ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("10/{productname}/{lotno}")]
        public HttpResponseMessage Get_from_LotNo10(string productname, string lotno) {

            var res = new LotInfoResponse();

            try {
                //ロット情報取得
                ApiLotInfo lotinfo = ApiLotInfo.Get_LotInfo_from_LotNo10(productname, lotno);
                res.LotInfo = lotinfo;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (NotNewSystemException) {
                res.ErrorMessage = "対象ロットはシステムにありません。";
                res.OnlyJissekiSystem = true;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (FjhSystemException fs_ex) {
                res.ErrorMessage = fs_ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (ArmsSystemException as_ex) {
                res.ErrorMessage = as_ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}