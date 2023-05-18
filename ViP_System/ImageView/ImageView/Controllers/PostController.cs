using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace ImageView.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        static ImageConverter imgconv = new ImageConverter();

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
                else if (btnname == "削除")
                {
                    DeleteImage();
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
            var connectionString = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";
            using (var connection = new SqlConnection(connectionString))
            {
                //接続
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    try
                    {
                        //レコード削除
                        //var sqlDelete = "DELETE FROM Images";

                        //トランザクションを使う場合、第2引数にオブジェクトを渡さないとエラーになる（空でもOK）
                        //connection.Execute(sqlDelete, new { }, tx);

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
            List<byte[]> images = new List<byte[]>();
            List<byte> images2 = new List<byte>();
            var image = new byte[] { 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10 }; ;

            var connectionString = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";
            using (var connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                //接続
                connection.Open();

                cmd.CommandText = "SELECT Image FROM Images";
                try
                {
                    //レコード1件取得
                    //var sql = "SELECT TOP(10) Image FROM Images";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //image = connection.QueryFirst<byte[]>(sql);
                            //image = reader["image"];

                            //image = (byte[])imgconv.ConvertTo(reader["image"], typeof(byte[]));
                            image = (byte[])reader["image"];
                            images.Add(image);
                        }
                    }
                    //foreach (var im in image)
                    //{
                    ////    images2.Add(im);
                    //}


                    //ViewBag.image = images;
                    ViewBag.image = images;
                    cmd.Parameters.Clear();

                    //var sql = "SELECT TOP(10) Image FROM Images";
                    //image = connection.QueryFirst<byte[]>(sql);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void DeleteImage()
        {
            var connectionString = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";
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

                        //コミット
                        tx.Commit();

                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// object to byte 使用したがデータがおかしくなったので、未使用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
