using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KodaWeb.Models {

    public class MagInfo {

        /// <summary>
        /// マガジン番号
        /// </summary>
        //[Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "マガジン番号")]
        public string MagNo { get; set; }

        /// <summary>
        /// 18桁ロット番号
        /// </summary>
        [Display(Name = "計画ロット番号")]
        public string LotNo18 { get; set; }

        /// <summary>
        /// 10桁ロット番号
        /// </summary>
        [Display(Name = "実績ロット番号")]
        public string LotNo10 { get; set; }

        /// <summary>
        /// V溝ロット番号
        /// </summary>
        [Display(Name = "V溝ロット番号")]
        public string VLot{ get; set; }

        /// <summary>
        /// 半完成品コード or 完成品コード
        /// </summary>
        [Display(Name = "機種名")]
        public string ProductName { get; set; }

    }
}