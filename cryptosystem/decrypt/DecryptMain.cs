using System;
using System.IO;
using System.Text;
using System.Linq;

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

namespace decrypt {

	/// <summary>
	///	暗号化を行うメインクラス
	/// </summary>
	public class DecryptMain {


		/// <summary>
		/// 暗号化を行うメイン関数
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		private static void Main (string[] args) {
			// コマンドライン引数で指定されたファイルの暗号化を行う
			BinFileDecrypt(args[0]);
		}


		/// <summary>
		/// 暗号化されたバイナリファイルの復号化を行う
		/// </summary>
		/// <param name="filePath">復号化する対象のファイルパス</param>
		static void BinFileDecrypt (string filePath) {

			// 暗号化ファイルの読み込み
			using (BinaryReader br = new BinaryReader(File.OpenRead(filePath))) {

				// 復号化ファイルの出力バス
				string exportFilePath = $"{filePath}.bin";

				// すでに暗号化されたファイルが存在している場合は削除する
				if (File.Exists(exportFilePath)) File.Delete(exportFilePath);

				// ファイルに書き込む
				FileStream fs = new FileStream(exportFilePath, FileMode.CreateNew);
				using (BinaryWriter file = new BinaryWriter(fs) ) {

					// 読み込めている間コンソールに出力し続ける
					while (br.BaseStream.Position != br.BaseStream.Length) {

						// 先頭から8バイト読み込む
						byte[] buffer = new byte[8];
						br.Read(buffer, 0, buffer.Length);
						// ulong型に変換する
						ulong plane = BitConverter.ToUInt64(buffer, 0);
						// 復号化する
						byte decrypt_value = (byte) Key.Decrypt(plane);
						// ファイルに書き込み
						file.Write(decrypt_value);
					}
				}
			}
		}
	}
}
