using System;
using System.IO;


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
				using (BinaryWriter file = new BinaryWriter(new FileStream(exportFilePath, FileMode.CreateNew))) {

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
