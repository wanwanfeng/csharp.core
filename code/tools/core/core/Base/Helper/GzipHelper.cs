using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Library.Helper
{
	public partial class GzipHelper : EncryptHelper
	{
		public class GZIP
		{
			public static void Serialize(Dictionary<string, FileInfo> sourceFile, string destFileName)
			{
				using (FileStream fileStream = new FileStream(destFileName, FileMode.Create, FileAccess.Write))
				using (GZipStream compressedzipStream = new GZipStream(fileStream, CompressionMode.Compress, true))
				using (BinaryWriter bw = new BinaryWriter(compressedzipStream))
				{
					bw.Write(sourceFile.Count);
					foreach (var item in sourceFile)
					{
						bw.Write(item.Key);
						var bytes = File.ReadAllBytes(item.Value.FullName);
						bw.Write(bytes.Length);
						bw.Write(bytes);
					}
				}

				File.WriteAllBytes(destFileName, AES.Encrypt(File.ReadAllBytes(destFileName), "567jr766kn64o8[/;,dfs"));

				//TestDeserialize(destFileName);
			}

            [System.Diagnostics.Conditional("UNITY_EDITOR")]
            static void TestDeserialize(string destFileName)
			{
				Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
				IEnumerator enumerator = Deserialize(destFileName, result);
				while (enumerator.MoveNext())
				{
					Console.WriteLine(enumerator.Current);
				}
				foreach (var item in result)
				{
					Console.WriteLine(item.Key);
					Console.WriteLine(item.Value.Length);
				}
			}

			public static IEnumerator Deserialize(string sourceFileName, Dictionary<string, byte[]> result)
			{
				var temp = sourceFileName + ".temp";

				try
				{
					File.WriteAllBytes(temp, AES.Decrypt(File.ReadAllBytes(sourceFileName), "567jr766kn64o8[/;,dfs"));
				}
				catch (Exception)
				{
					yield break;
				}

				yield return null;

				try
				{
					using (FileStream fileStream = new FileStream(temp, FileMode.Open, FileAccess.Read))
					using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
					using (BinaryReader br = new BinaryReader(decompressedStream))
					{
						var count = br.ReadInt32();
						for (int i = 0; i < count; i++)
						{
							var key = br.ReadString();
							var length = br.ReadInt32();
							var value = br.ReadBytes(length);
							result[key] = value;
						}
					}
				}
				catch (Exception)
				{
					yield break;
				}
				finally
				{
					File.Delete(temp);
				}
			}

			public static void WriteAllText(string destFileName, string content)
			{
				WriteAllBytes(destFileName, System.Text.Encoding.UTF8.GetBytes(content));
			}

			public static void WriteAllBytes(string destFileName, byte[] bytes)
			{
				using (FileStream fileStream = File.Create(Path.ChangeExtension(destFileName, ".gz")))
				using (GZipStream compressedzipStream = new GZipStream(fileStream, CompressionMode.Compress, true))
					compressedzipStream.Write(bytes, 0, bytes.Length);
			}

			public static string ReadAllText(string fileName)
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
				using (StreamReader reader = new StreamReader(decompressedStream))
					return reader.ReadToEnd();
			}

			public static IEnumerator<string> ReadLine(string fileName)
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
				using (StreamReader reader = new StreamReader(decompressedStream))
					yield return reader.ReadLine();
			}
		}
	}
}
