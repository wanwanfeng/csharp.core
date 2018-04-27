using System.Text;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;


public static partial class Util {

    /// <summary>
    /// フォルダを再帰的に作成
    /// </summary>
    /// <returns><c>true</c>, if directory was made, <c>false</c> otherwise.</returns>
    /// <param name="filePath">作成するパス.</param>
    public static bool MakeDirectory(string filePath)
    {
        bool result = true;
        string dir = Path.GetDirectoryName(filePath);
        if (dir != null && !Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                result = false;
            }
        }
        return result;
    }
}

