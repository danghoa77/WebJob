
namespace Job1670.Services.CVService
{
    public class PdfFileService : IPdfFileService
    {
        private readonly string _pdfFilesPath;

        public PdfFileService(IWebHostEnvironment webHostEnvironment)
        {
            _pdfFilesPath = Path.Combine(webHostEnvironment.ContentRootPath, "UploadedPdfFiles");
        }

        public async Task<string> UploadPdfAsync(Stream inputFileStream, string fileName)
        {
            try
            {
                if (!Directory.Exists(_pdfFilesPath))
                {
                    Directory.CreateDirectory(_pdfFilesPath);
                }

                var filePath = Path.Combine(_pdfFilesPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await inputFileStream.CopyToAsync(fileStream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tại đây nếu cần
                throw ex;
            }
        }
    }
}
