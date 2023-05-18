using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageView.Models
{
    public class ImageModel
    {
        const string ConnectionString = "server=VAUTOM3\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";
        public long Id;

        public ImageModel()
        {
        }

        public ImageModel(long id)
        {
            Id = id;
        }

        public byte[] Show()
        {
            var dt = new DataTable();
            var conn = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("select [Images].[Bin] from [Images] where [Images].[id]=@Id;", conn);
            cmd.Parameters.AddWithValue("@Id", Id);
            var adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            return dt.Rows[0][0] as byte[];
        }

        public long Create()
        {
            var conn = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("insert into [Images]([Images].[Bin]) values(@Bin); select @@identity;", conn);
            cmd.Parameters.AddWithValue("@Bin", File("Images_Bin"));
            conn.Open();
            long.TryParse(cmd.ExecuteScalar().ToString(), out Id);
            conn.Close();
            return Id;
        }

        public static byte[] File(string key)
        {
            //var file = HttpContext.Current.Request.Files[key];
            //var bin = new byte[file.ContentLength];
            //file.InputStream.Read(bin, 0, file.ContentLength);
            //return bin;

            var byteItems = new byte[] { 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10 };
            return byteItems;
        }
    }
}
