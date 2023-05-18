using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.IO;

namespace ImageUpload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IFormFile formFile, string btnname)
        {
            try
            {
                ViewBag.message = "";

                if (btnname == "登録")
                {
                    InsertImage(formFile, btnname);
                    ViewBag.message = "登録完了";
                }
                else if (btnname == "画像取得")
                {
                    GetImage();
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                return View();
            }
        }

        private void InsertImage(IFormFile formFile, string value)
        {
            var connectionString = "環境に合わせてください。";
            using (var connection = new SqlConnection(connectionString))
            {
                //接続
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        //レコード削除
                        var sqlDelete = "DELETE FROM Images";

                        //トランザクションを使う場合、第2引数にオブジェクトを渡さないとエラーになる（空でもOK）
                        connection.Execute(sqlDelete, new { }, tx);

                        //データ挿入
                        var sqlInsert = "INSERT INTO Images (Image) VALUES (@Image)";
                        var ms = new MemoryStream();
                        formFile.CopyTo(ms);
                        var param = new { Image = ms.ToArray() };
                        connection.Execute(sqlInsert, param, tx);

                        //コミット
                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        //ロールバック
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
        private void GetImage()
        {
            var connectionString = "環境に合わせてください。";
            using (var connection = new SqlConnection(connectionString))
            {
                //接続
                connection.Open();
                try
                {
                    //レコード1件取得
                    var sql = "SELECT TOP(1) Image FROM Images";
                    var image = connection.QueryFirst<byte[]>(sql);
                    ViewBag.image = image;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
}