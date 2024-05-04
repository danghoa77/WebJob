namespace Job1670.Services.CVService
{
    public interface IPdfFileService
    {
        Task<string> UploadPdfAsync(Stream fileStream, string fileName);
    }
}
