using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace LENS2_Api
{
	public class Directory
	{
		/// <summary>
		/// 正規表現で指定したパス下のファイル一覧を取得する。
		/// extentionSearchPatternには拡張子の.(ピリオド)は含む必要無し
		/// Directory.GetFiles()ではファイル名に任意の文字を含むファイル一覧の取得が出来ない為、自作
		/// </summary>
		/// <param name="path">Directory.GetFiles()のpathと同じ</param>
		/// <param name="fileNameSearchPattern">ファイル名部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <param name="extentionSearchPattern">拡張子部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <returns></returns>
		public static List<string> GetFiles(string path, string fileNameSearchPattern, string extentionSearchPattern)
		{
			string[] pathArray = System.IO.Directory.GetFiles(path, "*.*");

			Regex regex = new Regex(fileNameSearchPattern + "[.]" + extentionSearchPattern);

			return pathArray.Where(p => regex.IsMatch(p)).ToList();
		}

		/// <summary>
		/// 正規表現で指定したパス下のファイル一覧を取得する。
		/// .は任意の一文字 *は手前の文字の0文字以上の +は手前の文字の1文字以上の、?は手前の文字の0または1文字以上の繰り返し
		/// Directory.GetFiles()ではファイル名に任意の文字を含むファイル一覧の取得が出来ない為、自作
		/// </summary>
		/// <param name="path">Directory.GetFiles()のpathと同じ</param>
		/// <param name="fileNameSearchPattern">ファイル名部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <param name="extentionSearchPattern">拡張子部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <returns></returns>
		public static List<string> GetFiles(string path, string searchPattern)
		{
			string[] pathArray = System.IO.Directory.GetFiles(path, "*.*");

			Regex regex = new Regex(searchPattern);

			return pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
		}
	}
}
