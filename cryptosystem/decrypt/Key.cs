using System;
using System.IO;

/// <summary>
/// 鍵のシングルトンなクラス
/// </summary>
public class Key {

	#region プロパティ
	// 鍵のインスタンスプール
	private static Key[] keys;

	// 鍵の値
	private ulong value;
	#endregion


	#region 関数

	/// <summary>
	///	静的コンストラクタ
	///	メインメソッドよりも先に実行される
	/// </summary>
	static Key () {

		// 鍵の配列を生成
		keys = new Key[] {
				// デフォルトのキー(値は任意)
				new Key() {value = 12485590531114110080},
				// 残りは未生成時に作成される
				new Key() {value = 0},
				new Key() {value = 0}
			};

		// 鍵が保管されているファイルパス
		string dirPath = $"{ Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) }/.nk_cryptosystem";
		string filePath = $"{ dirPath }/enc_keys.txt";

		// ファイルが存在するとき
		if (File.Exists(filePath)) {

			// 読み込み文字列を格納する変数
			string str = null;

			// ファイルを1行ずつ読み込んで鍵の生成を行う
			using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8"))) {

				// 添え字
				int index = 1;

				// ひたすら読み込み続ける
				while ((str = sr.ReadLine()) != null) {
					// 念のため添え字が2を超えたら終了する
					if (index > 2) break;
					// 1つ目の添え字の鍵を用いて鍵自体の復号化を行う
					keys[index] = DecryptKey(
						new Key() { value = ulong.Parse(str) },
						keys[index - 1]
					);
					index++;
				}
			}
		}
		// 存在しないとき
		else {

			// ディレクトリの生成
			Directory.CreateDirectory(dirPath);

			// 鍵の生成
			for (int i = 1; i <= 2; i++) {
				// 鍵
				ulong key = 0;
				// 64bitの鍵の生成
				for (int j = 0; j < 4; j++) {
					// 確実に64bitを確保するために範囲を限定する32768 ~ 65535
					ulong temp = (ulong) new Random().Next(32768, 65536);
					// 16ビットずつ左シフトした値を加算する
					key += temp << j * 16;
				}
				keys[i].value = key;
			}

			// 生成した鍵をファイルに書き込む
			using (StreamWriter file = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("UTF-8"))) {
				// 1つ前の鍵を元に、鍵自体を暗号化して1行ずつ書き込んでいく
				for (int i = 1; i <= 2; i++) file.WriteLine(EncryptKey(keys[i], keys[i - 1]).value);
			}
		}
	}


	/// <summary>
	/// 鍵の暗号化を行う
	/// </summary>
	/// <param name="enc_key">暗号化する鍵</param>
	/// <param name="use_key">暗号化に用いる鍵</param>
	/// <returns></returns>
	private static Key EncryptKey (Key enc_key, Key use_key) {

		// 鍵同士の排他的論理和の反転を取る
		ulong enc_value = ~(enc_key.value ^ use_key.value);
		// 新しい鍵として返す
		return new Key() { value = enc_value };
	}


	/// <summary>
	/// 鍵の復号化を行う
	/// </summary>
	/// <param name="dec_key">復号化する鍵</param>
	/// <param name="use_key">復号化に用いる鍵</param>
	/// <returns></returns>
	private static Key DecryptKey (Key dec_key, Key use_key) {

		// dec_keyの反転を行って、鍵同士の排他的論理和を取る
		ulong dec_value = ~dec_key.value ^ use_key.value;
		// 新しい鍵として返す
		return new Key() { value = dec_value };
	}


	/// <summary>
	/// 復号化を行う関数
	/// </summary>
	/// <param name="encrypt_value">暗号化された値</param>
	/// <returns>復号化された値が返される</returns>
	public static ulong Decrypt (ulong encrypt_value) {

		// 複合化された値を代入する変数
		ulong decrypt_value = encrypt_value;

		// 3つ目の鍵の値の排他的論理和を取る
		decrypt_value ^= keys[2].value;
		// 反転させる
		decrypt_value = ~decrypt_value;
		// 反転させた値と2つ目の鍵の値の排他的論理和を取る
		decrypt_value ^= keys[1].value;
		// 反転させる
		decrypt_value = ~decrypt_value;
		// 反転させた値とデフォルトの鍵の値の排他的論理和を取る
		decrypt_value ^= keys[0].value;
		// 反転させる
		decrypt_value = ~decrypt_value;

		return decrypt_value;
	}

	#endregion
}