using System;
using System.IO;
using System.Text;


/*
 * カレントディレクトリ取得
 * http://var.blog.jp/archives/66978870.html
 * 文字コード判別
 * http://namco.hatenablog.jp/entry/2017/02/05/160119
 * バイナリファイルのIO
 * https://qiita.com/miyamoto_works/items/637732d880defd1d437d
 * 暗号化参考
 * http://www.mizuko.okayama-c.ed.jp/gakka/i/H22kaken/H22h-ang.pdf
 * 共通鍵にした裏付け
 * https://www.nic.ad.jp/ja/materials/iw/2008/proceedings/H10/IW2008-H10-01.pdf
 */

namespace encrypt {

	/// <summary>
	///	暗号化を行うメインクラス
	/// </summary>
	public class EncryptMain {


		/// <summary>
		/// 暗号化を行うメイン関数
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		public static void Main (string[] args) {

			// コマンドライン引数で指定されたファイルの暗号化を行う
			BinFileEncrypt(args[0]);
		}


		/// <summary>
		/// バイナリファイルの暗号化を行う
		/// </summary>
		/// <param name="filePath">暗号化する対象のファイルのパス</param>
		static void BinFileEncrypt (string filePath) {

			/*
			 * バイナリファイルの読み込みを行う
			 * 現状はカレントディレクトリのバイナリファイルでやる
			 */
			FileStream fs = File.OpenRead(filePath);
			using (BinaryReader br = new BinaryReader(fs)) {

				// 暗号化ファイルの出力バス
				string exportFilePath = $"{filePath}.enc";
				// すでに暗号化されたファイルが存在している場合は削除する
				if (File.Exists(exportFilePath)) File.Delete(exportFilePath);

				// バイナリファイルとして書き込む
				using (BinaryWriter file = new BinaryWriter(new FileStream(exportFilePath, FileMode.CreateNew))) {

					// 読み込めている間コンソールに出力し続ける
					while (br.BaseStream.Position != br.BaseStream.Length) {

						// 1バイト読み込む
						ulong plane = br.ReadByte();
						// 暗号化する
						ulong encrypt_value = Key.Encrypt(plane);
						// 暗号化された値の書き込みを行う
						file.Write(encrypt_value);
					}
				}
			}
		}
	}
}
