using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// 帳票情報クラス
    /// </summary>
    public class FormInfo : FormsSQL.TmFormMasterData {

        /// <summary>
        /// 帳票のJsonSchemaの部分
        /// </summary>
        public List<string> JsonSchema { get; set; }

        /// <summary>
        /// 帳票のUI Schemaの部分
        /// </summary>
        public List<string> UiSchema { get; set; }

        /// <summary>
        /// データ名一覧辞書のリスト
        /// </summary>
        public List<Dictionary<string, string>> DataNameDicList { get; set; }

    }
}