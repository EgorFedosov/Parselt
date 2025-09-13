namespace Backend.Application.Validators;

public static class UploadFileValidator
{
    public static bool IsCsv(IFormFile file) =>
        Path.GetExtension(file.FileName).ToLowerInvariant() == ".csv";
}