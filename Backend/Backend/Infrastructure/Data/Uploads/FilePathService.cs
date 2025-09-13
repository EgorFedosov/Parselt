namespace Backend.Infrastructure.Data.Uploads;

public static class FilePathService
{
    public static string UploadPath { get; private set; } = null!;

    public static void Init(string rootPath, string relativePath)
    {
        UploadPath = Path.Combine(rootPath, relativePath);
        Directory.CreateDirectory(UploadPath);
    }
}