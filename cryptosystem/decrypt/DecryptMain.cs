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

			// 文字コードの設定
			//Console.OutputEncoding = Encoding.UTF8;

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
				using (BinaryWriter file = new BinaryWriter(new FileStream(exportFilePath, FileMode.CreateNew))) {

					//Console.WriteLine($"[br.BaseStream.Length] : {br.BaseStream.Length}");

					// 読み込めている間コンソールに出力し続ける
					while (br.BaseStream.Position != br.BaseStream.Length) {

						long length = 8;
						byte[] buffer = new byte[length];
						if (length != 8) Console.Write("【!!!】");
						int result = br.Read(buffer, 0, buffer.Length);
						ulong plane = BitConverter.ToUInt64(buffer, 0);
						ulong decrypt_value = Key.Decrypt(plane);
						Console.Write($"Pos : {br.BaseStream.Position.ToString().PadLeft(3, '0')}, IN : {plane.ToString().PadLeft(20, '0')}({plane.ToString("x").PadLeft(16, '0')}), Dec : {decrypt_value.ToString().PadLeft(20, '0')}, 0x : {decrypt_value.ToString("x").PadLeft(16, '0')}, Export : ");
						foreach (byte b in BitConverter.GetBytes(decrypt_value)) {
							Console.Write(Convert.ToString(b, 16).PadLeft(2, '0'));
							file.Write(b);
						}
						Console.WriteLine("");
					}
				}
			}
		}
	}
}
