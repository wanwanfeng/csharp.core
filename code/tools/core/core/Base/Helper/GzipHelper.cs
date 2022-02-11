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
		public class ZIP
		{/*
			public static void Serialize(Dictionary<string, FileInfo> sourceFile, string destFileName)
			{
				using (FileStream fileStream = new FileStream(destFileName, FileMode.Create, FileAccess.Write))
				using (ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
				{
					foreach (var item in sourceFile)
					{
						ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry(item.Key);
						using (var fr = File.OpenRead(item.Value.FullName))
						using (var s = zipArchiveEntry.Open())
						{
							fr.CopyTo(s);
						}
					}
				}
			}

			public static Dictionary<string, byte[]> Deserialize(string sourceFileName)
			{
				Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
				using (FileStream fileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
				using (ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read, true))
				{
					foreach (var item in zipArchive.Entries)
					{
						using (MemoryStream ms = new MemoryStream())
						using (var s = item.Open())
						{
							s.CopyTo(ms);
							result[item.FullName] = ms.ToArray();
						}
					}
				}
				return result;
			}*/
		}

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
			}

			public static Dictionary<string, byte[]> Deserialize(string sourceFileName)
			{
				Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
				using (FileStream fileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
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
				return result;
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

			public static byte[] ReadAllBytes(string fileName)
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
				using (MemoryStream ms = new MemoryStream())
				{
					decompressedStream.CopyTo(ms);
					return ms.ToArray();
				}
			}
		}
	}
}
