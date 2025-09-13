namespace Backend.Infrastructure.Utils;

public static class FilePathGenerator
{
    public static  string GetUniqueFilePath(string uploadPath, string fileName)
    {
        string fullPath = Path.Combine(uploadPath, fileName);
        uint count = 1;
        
        while (File.Exists(fullPath))
        {   
            fullPath = $"{uploadPath}/{Path.GetFileNameWithoutExtension(fileName)}({count++}){Path.GetExtension(fileName)}";
        }
        return fullPath;
    }
}