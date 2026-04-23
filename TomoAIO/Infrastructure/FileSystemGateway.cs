namespace TomoAIO.Infrastructure
{
    internal sealed class FileSystemGateway
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public bool FileExists(string path) => File.Exists(path);

        public string[] GetFiles(string path, string pattern) => Directory.GetFiles(path, pattern);

        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);

        public void WriteAllBytes(string path, byte[] data) => File.WriteAllBytes(path, data);

        public void CopyFile(string source, string destination, bool overwrite = true) => File.Copy(source, destination, overwrite);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}
