
namespace SvnVersion
{
    public class SvnFileInfo
    {
        public string path;
        public string action;
        public string version;
        public string content_hash;
        public string content_size;
        public string encrypt_hash;
        public string encrypt_size;

        public string path_md5
        {
            get { return Library.Encrypt.MD5.Encrypt(path); }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", path_md5, action, version, content_hash,
                content_size,
                encrypt_hash, encrypt_size, path);
        }
    }

    public class SvnPatchInfo
    {
        public string group;
        public int firstVersion;
        public int lastVersion;
        public string path;
        public string content_hash;
        public string content_size;
        public string zip_hash;
        public string zip_size;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", group, firstVersion, lastVersion, path, content_hash,
                content_size, zip_hash, zip_size);
        }
    }
}