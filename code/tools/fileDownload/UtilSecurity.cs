using System;
using System.IO;
using System.Security.Cryptography;

public static class UtilSecurity {

	#region RijndaelManaged


	static RijndaelManaged _rijndaelManged = null;
	public static RijndaelManaged rijndaelManged 
	{
		get{

			if (_rijndaelManged == null){
				_rijndaelManged 			= new RijndaelManaged();
				_rijndaelManged.Padding 	= PaddingMode.Zeros;
				_rijndaelManged.Mode    	= APP_CONFIG.SECURITY.CIPHER_MODE;
				_rijndaelManged.KeySize 	= APP_CONFIG.SECURITY.SIZE;
				_rijndaelManged.BlockSize	= APP_CONFIG.SECURITY.SIZE;
			}
			return _rijndaelManged;
		}
	}

	public static ICryptoTransform MakeDecryptorTransform () {

		byte[] key = GetRijndaelBytes (APP_CONFIG.SECURITY.KEY_DECRYPT_STR);

		if (APP_CONFIG.SECURITY.CIPHER_MODE == CipherMode.CBC) {
			byte[] iv = GetRijndaelBytes (APP_CONFIG.SECURITY.DEFAULT_DECRYPT_IV_STR);
			return rijndaelManged.CreateDecryptor (key, iv);
		} else {
			rijndaelManged.Key = key;
			return rijndaelManged.CreateDecryptor ();
		}
	}

	public static ICryptoTransform MakeDecryptorTransform (string strName) {
		//TODO:ファイル名からハッシュ値を取得
		//RijndaelManaged rijndael = MakeGijindaelManaged(
		//return MakeGijindaelManaged(APP_CONFIG.SECURITY.CIPHER_MODE, APP_CONFIG.SECURITY.SIZE, APP_CONFIG.SECURITY.KEY_DECRYPT_STR, GetIV(strName)).CreateDecryptor();

		byte[] key = GetRijndaelBytes(APP_CONFIG.SECURITY.KEY_DECRYPT_STR);

		if (APP_CONFIG.SECURITY.CIPHER_MODE == CipherMode.CBC){	
			byte[] iv = GetRijndaelBytes( GetIV(strName) );
			return rijndaelManged.CreateDecryptor( key, iv );
		}
		else{
			//cbc以外の場合はfileNameは関係ないのでMakeDecryptorTransformで流用する
			return MakeDecryptorTransform();
		}
	}

	private static ICryptoTransform _encryptor = null;
	public static ICryptoTransform MakeEncryptorTransform () {
		if (_encryptor == null) {
			byte[] key = GetRijndaelBytes (APP_CONFIG.SECURITY.KEY_DECRYPT_STR);
	
			if (APP_CONFIG.SECURITY.CIPHER_MODE == CipherMode.CBC) {	
				byte[] iv = GetRijndaelBytes (APP_CONFIG.SECURITY.DEFAULT_DECRYPT_IV_STR);
				_encryptor = rijndaelManged.CreateEncryptor (key, iv);
			} else {
				rijndaelManged.Key = key;
				_encryptor = rijndaelManged.CreateEncryptor ();
			}
		}
		return _encryptor;
	}

	public static ICryptoTransform MakeEncryptorTransform (string strName) {
		//TODO:ファイル名からハッシュ値を取得
		//return MakeGijindaelManaged(APP_CONFIG.SECURITY.CIPHER_MODE, APP_CONFIG.SECURITY.SIZE, APP_CONFIG.SECURITY.KEY_DECRYPT_STR, GetIV(strName)).CreateEncryptor();


		//return System.Text.Encoding.UTF8.GetBytes(str);

		byte[] key = GetRijndaelBytes( APP_CONFIG.SECURITY.KEY_DECRYPT_STR );

		if (APP_CONFIG.SECURITY.CIPHER_MODE == CipherMode.CBC){	
			byte[] iv =  GetRijndaelBytes( GetIV(strName) );
			return rijndaelManged.CreateEncryptor( key, iv );
		}
		else{
			//cbc以外の場合はfileNameは関係ないのでMakeEncryptorTransformで流用する
			return MakeEncryptorTransform();
		}

	}

	public static byte[] GetRijndaelBytes(string str) {

		return System.Text.Encoding.UTF8.GetBytes(str);
//
//		int length = str.Length / 2;
//		byte[] bytes = new byte[length];
//		int j = 0;
//		for (int i = 0; i < length; i++) {
//			bytes[i] = Convert.ToByte(str.Substring(j, 2), 16);
//			j += 2;
//		}
//		return bytes;
	}


	/// <summary>
//	/// セキュリティ用のRijndaelManagedを作成
//	/// </summary>
//	static RijndaelManaged MakeGijindaelManaged (string strName) {
//		//TODO:ファイル名からハッシュ値を取得
//		return MakeGijindaelManaged(APP_CONFIG.SECURITY.CIPHER_MODE, APP_CONFIG.SECURITY.SIZE, APP_CONFIG.SECURITY.KEY_DECRYPT_STR, GetIV(strName));
//	}

	static string GetIV(string fileName){
        string retVal = GetMD5Value(fileName + APP_CONFIG.SECURITY.IV_DECRYPT_STR);
		//AppDebug.Log ("GetIVKey" + retVal + ":" + fileName);
		return retVal;
	}

	#endregion



	/// <summary>
	/// 加密 (文字列)
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="str">String.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	public static string EncryptionString (string str, ICryptoTransform encryptor){
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
		byte[] encryptedByte = EncryptionBytes(bytes, encryptor);
		return System.Convert.ToBase64String(encryptedByte);
	}


	/// <summary>
	/// 加密 (バイナリ)
	/// </summary>
	/// <returns>The bytes.</returns>
	/// <param name="bytes">Bytes.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	public static byte[] EncryptionBytes (byte[] bytes){
		return EncryptionBytes(bytes, MakeEncryptorTransform());
	}


	/// <summary>
	/// 加密 (バイナリ)
	/// </summary>
	/// <returns>The bytes.</returns>
	/// <param name="bytes">Bytes.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	public static byte[] EncryptionBytes (byte[] bytes, ICryptoTransform encryptor){

		//ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor();
		
		MemoryStream memoryStm = new MemoryStream();
		CryptoStream cryptoStm  = new CryptoStream(memoryStm, encryptor, CryptoStreamMode.Write);

		cryptoStm.Write(bytes, 0, bytes.Length);
		cryptoStm.FlushFinalBlock();

		//AppDebug.Log("EncryptionBytes:" + rijndaelManaged.IV + "\t" + rijndaelManaged.Key);
		
		byte[] encryptedByte = memoryStm.ToArray();

		return encryptedByte;
	}



	/// <summary>
	///  解密 (バイナリ)
	/// </summary>
	/// <returns>The bytes.</returns>
	/// <param name="encryptedByte">Encrypted byte.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	public static byte[] DecryptionBytes (byte[] encryptedByte){
		return DecryptionBytes (encryptedByte, MakeDecryptorTransform ());
	}


	/// <summary>
	/// 解密 (文字列)
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="str">String.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	public static string DecryptionString (string str, ICryptoTransform decryptor){
		byte[] bytes = System.Convert.FromBase64String(str);
		byte[] encryptedByte = DecryptionBytes(bytes, decryptor);
		return System.Text.Encoding.UTF8.GetString(encryptedByte);
	}

	/// <summary>
	///  解密 (バイナリ)
	/// </summary>
	/// <returns>The bytes.</returns>
	/// <param name="encryptedByte">Encrypted byte.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	public static byte[] DecryptionBytes (byte[] encryptedByte, ICryptoTransform decryptor){
		//ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor();
		
		byte[] fromEncrypt    = new byte[encryptedByte.Length];

		MemoryStream memoryStm = new MemoryStream(encryptedByte);
		CryptoStream cryptoStm = new CryptoStream(memoryStm, decryptor, CryptoStreamMode.Read);
		
		cryptoStm.Read(fromEncrypt, 0, fromEncrypt.Length);

		return fromEncrypt;
	}

	public delegate byte[] ConvertBytes(byte[] encryptedByte, ICryptoTransform icryptoTransform);

	/// <summary>
	/// Exchanges the files.
	/// </summary>
	/// <param name="targetDir">Target dir.</param>
	/// <param name="resultDir">Result dir.</param>
	/// <param name="exchangeBytes">Exchange bytes.</param>
	/// <param name="rijndaelManaged">Gijindael managed.</param>
	/// <param name="key">Key.</param>
	/// <param name="iv">Iv.</param>
//    public static void ExchangeFiles(string targetDir, string resultDir, ConvertBytes exchangeBytes, ICryptoTransform icryptoTransform = null) {
////		TimeMeasurement.instance.Reset( "ExchangeFiles" );
////		TimeMeasurement.instance.Log("ExchangeFiles", "start");
//        string[] files = Directory.GetFiles(targetDir.Replace("\\","/"), "*", SearchOption.AllDirectories);
//        //List<string> fileList = new List<string>();
//        //int i=0;
//        foreach (string targetFile in files) {
//            if( System.Text.RegularExpressions.Regex.IsMatch( targetFile, @"^*\.meta$", System.Text.RegularExpressions.RegexOptions.ECMAScript) ) {
//                continue;
//            }
//            if( System.Text.RegularExpressions.Regex.IsMatch( targetFile, @"^*\.DS_Store$", System.Text.RegularExpressions.RegexOptions.ECMAScript) ) {
//                continue;
//            }
//            string sp = System.Text.RegularExpressions.Regex.Replace( targetFile, @"^"+targetDir, "" );
//            string saveFile = resultDir + "/" + sp;
			
//            try {
//                byte[] targetBytes = File.ReadAllBytes( targetFile );
//                byte[] resultBytes = exchangeBytes( targetBytes, icryptoTransform);
//                Util.MakeDirectory( saveFile );
//                File.WriteAllBytes( saveFile, resultBytes );
//            //}catch( SystemException e ){
//            }catch( SystemException ){	// TODO warning
////				AppDebug.LogError( e.Message, AppDebug.LogType.File );
//                continue;
//            }
////			TimeMeasurement.instance.Log("ExchangeFiles", sp);
//        }
//    }

//		rijndaelManged.Key = System.Text.Encoding.UTF8.GetBytes( keyDecrypt );
//		if( cipherMode.Equals( CipherMode.CBC ) ) {
//
//			AppDebug.Log("DBC.IV:" + rijndaelManged.BlockSize + ":" + System.Text.Encoding.UTF8.GetBytes( keyDecryptIv ).Length);
//
//			//AppDebug.Log("16:" + System.Text.Encoding.UTF32.GetBytes( keyDecryptIv ).Length);
//			//AppDebug.Log("7:" + System.Text.Encoding.UTF7.GetBytes( keyDecryptIv ).Length);
//			//rijndaelManged.IV = System.Text.Encoding.UTF8.GetBytes( keyDecryptIv );
//			rijndaelManged.IV = System.Text.Encoding.UTF8.GetBytes( keyDecryptIv );
//
//		}
//	}

	/* ~~~開発中~~~
	/// <summary>
	/// 加密
	/// </summary>
	private static byte[] MaskBinary(byte[] bytes, int mask, int loop = 1) {//Encoding.ASCII.GetBytes(kye);
		for( int i=0; i<loop; i++ ) {
			bytes = XorMaskBitrotation( bytes, mask );
		}
		return bytes;
	}

	/// <summary>
	/// XorMask
	/// </summary>
	private static byte[] XorMaskBitrotation(byte[] bytes, int mask) {//Encoding.ASCII.GetBytes(kye);
		//int j = 0;
		//string str = "";
		//bytesの要素分繰り返す
		for (int i = 0; i < bytes.Length; i++) {
			//keyがbytesより要素数が少ないときのため
//			if (j < key.Length) {
//				j++;
//			} else {
//				j = 1;
//			}
			//xorで加密
			bytes[i] = (byte)(bytes[i] ^ mask);
		}
		return bytes;
	}
	*/


	public static string GetNonceSecretValue(string nonce){
		return GetSHA512Value ("nonce=" + nonce + "&secret=" + APP_CONFIG.SECURITY.DEFAULT_NONCE_KEY);
	}


	/// <summary>
	/// 文字列をSHA512に変換
	/// </summary>
	public static string GetSHA512Value(string str){
		//DEFAULT_NONCE_KEY
		if( string.IsNullOrEmpty(str) ) return null;
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

		return GetSHA512Value (bytes);
	}

	/// <summary>
	/// bytesをmd5値に変換
	/// </summary>
	/// <returns>md5 value.</returns>
	/// <param name="bytes">Bytes.</param>
	public static string GetSHA512Value(byte[] bytes) {
		if( bytes == null || bytes.Length.Equals(0)) return null;

		SHA512Managed sha = new SHA512Managed();
		//SHA512 sha = new SHA512();
		byte[] bs = sha.ComputeHash(bytes);

		System.Text.StringBuilder result = new System.Text.StringBuilder();
		foreach (byte b in bs)
		{
			result.Append(b.ToString("x2"));
		}
		return result.ToString();
	}


	/// <summary>
	/// 文字列をmd5に変換
	/// </summary>
	/// <returns>md5 value.</returns>
	/// <param name="str">String.</param>
	public static string GetMD5Value(string str) {
		
		if( string.IsNullOrEmpty(str) ) return null;
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
		
		return GetMD5Value( bytes );
	}
	
	/// <summary>
	/// bytesをmd5値に変換
	/// </summary>
	/// <returns>md5 value.</returns>
	/// <param name="bytes">Bytes.</param>
	public static string GetMD5Value(byte[] bytes) {
		if( bytes == null || bytes.Length.Equals(0)) return null;
		
		var md5 = new MD5CryptoServiceProvider();
		byte[] bs = md5.ComputeHash(bytes);
		
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		foreach (byte b in bs)
		{
			result.Append(b.ToString("x2"));
		}
		
		return result.ToString();
		
	}

	/// <summary>
	/// Creates the token.
	/// </summary>
	/// <returns>The token.</returns>
	/// <param name="tokenLength">Length.</param>
	public static string CreateToken(int tokenLength = 32){
		byte[] token = new byte[tokenLength];
		
		RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();
		gen.GetBytes(token);
		
		System.Text.StringBuilder buf = new System.Text.StringBuilder();
		
		for (int i = 0; i < token.Length; i++)
		{
			buf.AppendFormat("{0:x2}", token[i]);
		}
		
		return buf.ToString();
	}	
}
