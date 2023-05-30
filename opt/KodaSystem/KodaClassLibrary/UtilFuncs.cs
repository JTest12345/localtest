using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Security.AccessControl;
using System.Security.Principal;

namespace KodaClassLibrary {

    public static class UtilFuncs {

        /// <summary>
        /// ファイル内のテキストを丸ごと読み込む
        /// </summary>
        public static string ReadText(string path) {

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.UTF8)) {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// ファイルを新しい内容で上書きする
        /// </summary>
        public static void OverWriteText(string path, string text) {

            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8)) {
                sw.Write(text);
            }
        }

        /// <summary>
        /// 半角文字列を返す
        /// </summary>
        public static string GetHankaku(string s) {
            return Strings.StrConv(s, VbStrConv.Narrow);
        }

        /// <summary>
        /// フォルダのアクセスコントロールを変更する
        /// <para>↓ユーザー</para>
        /// <para>SYSTEM・・・何もしない</para>
        /// <para>他・・・読み取り、所有、アクセス権編集可能</para>
        /// <para>その他・・・読み取りのみ</para>
        /// </summary>
        public static void ChangeDirectoryAccessControl(string path) {

            //フォルダのセキュリティオプション変更
            var security = Directory.GetAccessControl(path);

            //継承状態は解除するが、権限は継承
            security.SetAccessRuleProtection(true, true);

            //アクセス権更新
            Directory.SetAccessControl(path, security);

            //再度取得
            security = Directory.GetAccessControl(path);

            //フォルダセキュリティに既に登録されているユーザーのアクセスコントロールを設定する
            foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(NTAccount))) {

                string user = (rule.IdentityReference as NTAccount).Value;
                if (user.Contains("SYSTEM")) {
                    //何もしない(たぶんFull Control)
                }
                else {
                    var new_rule = new FileSystemAccessRule(
                                 rule.IdentityReference,
                                 FileSystemRights.ReadAndExecute | FileSystemRights.TakeOwnership | FileSystemRights.ChangePermissions,
                                 InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                 PropagationFlags.None,
                                 AccessControlType.Allow);

                    security.SetAccessRule(new_rule);
                }
                //else {
                //    var new_rule = new FileSystemAccessRule(
                //                 rule.IdentityReference,
                //                 FileSystemRights.Read,
                //                 InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                //                 PropagationFlags.None,
                //                 AccessControlType.Allow);

                //    security.SetAccessRule(new_rule);
                //}
            }
            //アクセス権更新
            Directory.SetAccessControl(path, security);
        }
    }
}
