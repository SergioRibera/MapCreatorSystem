using System;
using System.Text;
using System.IO;
using System.IO.Compression;

public static class Utils
{
    public static string ReadFile(string pathFile) => File.ReadAllText(pathFile);
    public static void WriteFile(string pathFile, string content) => File.WriteAllText(pathFile, content);
    public static void MakeDirs(string[] dirs){
        foreach(var d in dirs)
            if(!Directory.Exists(d))
                Directory.CreateDirectory(d);
    }
    public static void DirExistsCreate(string path){
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    public static void ExistsDelete(string path){
        if(File.Exists(path))
            File.Delete(path);
    }
    public static void DeleteFile(string file){
        File.Delete(file);
    }

    public static void ReadIndexs(string main, Action<PackageModel> callback){
        string[] files = Directory.GetFiles(main, "index.json", SearchOption.AllDirectories);
        foreach (string f in files){
            string[] n = f.Split('/');
            callback(PackageModel.Load(f));
        }
    }

    public static void CompressFolder(string input, string output){
        string[] files = Directory.GetFiles(input, "*.*", SearchOption.AllDirectories);
        int dirLen = input[input.Length - 1] == Path.DirectorySeparatorChar ? input.Length : input.Length + 1;

        using (FileStream outFile = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
            using (GZipStream str = new GZipStream(outFile, CompressionMode.Compress))
                foreach (string filePath in files){
                    string relativePath = filePath.Substring(dirLen);
                    CompressFile(input, relativePath, str);
                }
    }
    public static void DecompressToDir(string compressFile, string dir){
        using (FileStream inFile = new FileStream(compressFile, FileMode.Open, FileAccess.Read, FileShare.None))
            using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                while(DecompressFile(dir, zipStream));
    }
    public static void CompressFile(string dir, string relativePath, GZipStream zipStream){
        // Compress file name
        char[] chars = relativePath.ToCharArray();
        zipStream.Write(BitConverter.GetBytes(chars.Length), 0, sizeof(int));
        foreach(var c in chars)
            zipStream.Write(BitConverter.GetBytes(c), 0, sizeof(char));
        
        // Compress file content
        byte[] bytes = File.ReadAllBytes(Path.Combine(dir, relativePath));
        zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
        zipStream.Write(bytes, 0, bytes.Length);
    }
    public static bool DecompressFile(string dir, GZipStream zipStream){
        // Decompress File Name
        byte[] bytes = new byte[sizeof(int)];
        int Readed = zipStream.Read(bytes, 0, sizeof(int));
        if (Readed < sizeof(int))
            return false;
        int nameLen = BitConverter.ToInt32(bytes, 0);
        bytes = new byte[sizeof(char)];
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < nameLen; i++){
            zipStream.Read(bytes, 0, sizeof(char));
            char c = BitConverter.ToChar(bytes, 0);
            sb.Append(c);
        }
        string fileName = sb.ToString();

        // Decompress file content
        bytes = new byte[sizeof(int)];
        zipStream.Read(bytes, 0, sizeof(int));
        int iFileLen = BitConverter.ToInt32(bytes, 0);

        bytes = new byte[iFileLen];
        zipStream.Read(bytes, 0, bytes.Length);

        string filePath = Path.Combine(dir, fileName);
        string finalDir = Path.GetDirectoryName(filePath);
        if(!Directory.Exists(finalDir))
            Directory.CreateDirectory(finalDir);
        using (FileStream outFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            outFile.Write(bytes, 0, iFileLen);
        return true;
    }
}
