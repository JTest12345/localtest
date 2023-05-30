using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// 修理記録登録クラス・・・Register Repair Record
    /// </summary>
    public class RegisterRR : FormsSQL.RepairRecordData {

        [Required(ErrorMessage = "登録者は必須です。")]
        public override string InsertBy { get; set; }

        [Required(ErrorMessage = "設備番号は必須です。")]
        public override string Plantcd { get; set; }

        [Required(ErrorMessage = "タイトルは必須です。")]
        public override string Title { get; set; }

        [Required(ErrorMessage = "不具合内容は必須です。")]
        public override string Failure { get; set; }

        /// <summary>
        /// POSTされた画像ファイル
        /// </summary>
        public IList<HttpPostedFileBase> PostedFiles { get; set; }       
    }

    public class CompleteRR : FormsSQL.RepairRecordData {

        [Required(ErrorMessage = "登録者は必須です。")]
        public override string UpdateBy { get; set; }

        [Required(ErrorMessage = "原因は必須です。")]
        public override string Cause { get; set; }

        [Required(ErrorMessage = "処置内容は必須です。")]
        public override string Treatment { get; set; }

        /// <summary>
        /// POSTされた画像ファイル
        /// </summary>
        public IList<HttpPostedFileBase> PostedFiles { get; set; }

        public List<string> RegisterImagePathList { get; set; }

        public List<string> CompleteImagePathList { get; set; }
    }

}