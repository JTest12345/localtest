﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GEICS.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB101\\INST1;Connect Timeout=0;Database=QCIL;User ID={0};password={1};App" +
            "lication Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_BATCH_SV_AUTO {
            get {
                return ((string)(this["ConnectionString_BATCH_SV_AUTO"]));
            }
            set {
                this["ConnectionString_BATCH_SV_AUTO"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB101\\INST1;Connect Timeout=0;Database=QCIL;User ID={0};password={1};App" +
            "lication Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_BATCH_SV_HIGH {
            get {
                return ((string)(this["ConnectionString_BATCH_SV_HIGH"]));
            }
            set {
                this["ConnectionString_BATCH_SV_HIGH"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=NMCDB2.nmc.local\\INST2;Connect Timeout=0;Database=SC07_NADB01;UID=sla02;PW" +
            "D=2Gquf5d;Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA_NMC {
            get {
                return ((string)(this["ConnectionString_NASCA_NMC"]));
            }
            set {
                this["ConnectionString_NASCA_NMC"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB1.nichia.local\\INST1;Connect Timeout=0;Database=NADB01;User ID={0};pas" +
            "sword={1};Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA {
            get {
                return ((string)(this["ConnectionString_NASCA"]));
            }
            set {
                this["ConnectionString_NASCA"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB1.nichia.local\\INST1;Connect Timeout=0;Database=SC02_NADB01;UID=sla02;" +
            "PWD=2Gquf5d;Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA_AOI {
            get {
                return ((string)(this["ConnectionString_NASCA_AOI"]));
            }
            set {
                this["ConnectionString_NASCA_AOI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB1.nichia.local\\INST1;Connect Timeout=0;Database=SC03_NADB01;UID=sla02;" +
            "PWD=2Gquf5d;Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA_KYO {
            get {
                return ((string)(this["ConnectionString_NASCA_KYO"]));
            }
            set {
                this["ConnectionString_NASCA_KYO"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB1.nichia.local\\INST1;Connect Timeout=0;Database=SC06_NADB01;UID=sla02;" +
            "PWD=2Gquf5d;Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA_MIK {
            get {
                return ((string)(this["ConnectionString_NASCA_MIK"]));
            }
            set {
                this["ConnectionString_NASCA_MIK"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server={0};Connect Timeout=0;Database={1};UID={2};PWD={3};Application Name=装置情報傾向" +
            "管理システム[GEICS3]")]
        public string ConnectionString_QCIL_MAIN {
            get {
                return ((string)(this["ConnectionString_QCIL_MAIN"]));
            }
            set {
                this["ConnectionString_QCIL_MAIN"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server={0}\\SQLEXPRESS;Connect Timeout=0;Database=QCIL;UID={1};PWD={2};Application" +
            " Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_QCIL_SUB {
            get {
                return ((string)(this["ConnectionString_QCIL_SUB"]));
            }
            set {
                this["ConnectionString_QCIL_SUB"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB1.nichia.local\\INST1;Connect Timeout=0;Database=SC10_NADB01;UID=sla05;" +
            "PWD=ZP3wnIC;Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA_CE {
            get {
                return ((string)(this["ConnectionString_NASCA_CE"]));
            }
            set {
                this["ConnectionString_NASCA_CE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB101\\INST1;Connect Timeout=0;Database=QCIL_MAP;User ID={0};password={1}" +
            ";Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_BATCH_MAP_AUTO {
            get {
                return ((string)(this["ConnectionString_BATCH_MAP_AUTO"]));
            }
            set {
                this["ConnectionString_BATCH_MAP_AUTO"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ConnectionString_BATCH_MAP_HIGH {
            get {
                return ((string)(this["ConnectionString_BATCH_MAP_HIGH"]));
            }
            set {
                this["ConnectionString_BATCH_MAP_HIGH"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server={0};Connect Timeout=0;Database={1};User ID={2};password={3};Application Na" +
            "me=装置情報傾向管理システム[GEICS3]")]
        public string ConStrServ {
            get {
                return ((string)(this["ConStrServ"]));
            }
            set {
                this["ConStrServ"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=HQDB1.nichia.local\\INST1;Connect Timeout=0;Database=SC11_NADB01;UID=sla05;" +
            "PWD=ZP3wnIC;Application Name=装置情報傾向管理システム[GEICS3]")]
        public string ConnectionString_NASCA_KMC {
            get {
                return ((string)(this["ConnectionString_NASCA_KMC"]));
            }
            set {
                this["ConnectionString_NASCA_KMC"] = value;
            }
        }
    }
}
