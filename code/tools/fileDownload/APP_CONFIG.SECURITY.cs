///-----------------------------------------------------------------------------------------------------
/// @file	APP_CONFIG.cs
/// @brief	アプリケーション設定ファイル セキュリティ関係
///-----------------------------------------------------------------------------------------------------


/// <summary>
/// アプリケーション設定ファイル セキュリティ関係
/// </summary>
public partial class APP_CONFIG
{
	public static class SECURITY
	{

		public static readonly string DEFAULT_NONCE_KEY		= "mPJpvOrx7Lv15OdnAC6CT23dC7g98fL0";	//nonce加密に使うkey

		public static readonly string KEY_DB_DECRYPT_STR	= "KpyzH6A5rpSeDydpi4Mrc44LsntviqQE";	//dbファイルの加密key

		public static readonly string KEY_DECRYPT_STR		= "kEfGenNmeu4BNEhv";					//ファイル加密に使用するkey値
		public static readonly string IV_DECRYPT_STR		= "f90626e4fe";							//ファイル加密に使用するiv値+ファイル名のhash値を使用する

		public static readonly string DEFAULT_DECRYPT_IV_STR= "588abe1de212c281bf48c3c41a288417";
		public static readonly int SIZE = 128;


		//		public static readonly string KEY_DECRYPT_STR		= "kEfGhnNmeu4YYuhv8CmkSyiNHvXsuPGU";
//		public static readonly string KEY_DECRYPT_IV_STR	= "7tndbMFnwGZM99qA9y3ZHSn729bQRCpn";
//		public static readonly int SIZE = 256;

//		public static readonly System.Security.Cryptography.CipherMode CIPHER_MODE = System.Security.Cryptography.CipherMode.ECB;
		public static readonly System.Security.Cryptography.CipherMode CIPHER_MODE = System.Security.Cryptography.CipherMode.ECB;


		public static readonly string CONVERT_DIR_TARGET	= "/Users/rhashimoto/Project/Pallete/app/image/banner";
		public static readonly string CONVERT_DIR_RESULT	= "/Users/rhashimoto/Project/Pallete/app/image/banner_resul";

//		private static System.Security.Cryptography.RijndaelManaged _rijndaelManged;
//		public static System.Security.Cryptography.RijndaelManaged RIJINDAEL_MANAGED {
//			get {
//				if( _rijndaelManged != null ) return _rijndaelManged;
//				_rijndaelManged = UtilSecurity.MakeGijindaelManaged( CIPHER_MODE, SIZE, KEY_DECRYPT_STR, KEY_DECRYPT_IV_STR);
//				return _rijndaelManged;
//			}
//		}

		public static System.Security.Cryptography.ICryptoTransform DECRYPTOR_TRANSFORM {
			get {
				//if( _rijndaelManged != null ) return _rijndaelManged.CreateDecryptor();
				//_rijndaelManged = UtilSecurity.MakeGijindaelManaged( CIPHER_MODE, SIZE, KEY_DECRYPT_STR, KEY_DECRYPT_IV_STR);
				return UtilSecurity.MakeDecryptorTransform();
				//return _rijndaelManged.CreateDecryptor();
			}
		}
		public static System.Security.Cryptography.ICryptoTransform ENCRYPTOR_TRANSFORM {
			get {
				//if( _rijndaelManged != null ) return _rijndaelManged.CreateEncryptor();
				return UtilSecurity.MakeEncryptorTransform();
				//return _rijndaelManged.CreateEncryptor();
			}
		}
	}
}
