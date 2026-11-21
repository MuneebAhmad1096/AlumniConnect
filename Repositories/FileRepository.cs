namespace AlumniConnect.Repositories
{
    public interface IFileRepository
    {
        Task<string?> SavePdfAsync(IFormFile? pdfFile);
        void DeletePdf(string? relativePath);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IWebHostEnvironment _env;

        public FileRepository(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SavePdfAsync(IFormFile? pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0) return null;

            string uploadsFolder = Path.Combine(_env.WebRootPath, "docs");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(pdfFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await pdfFile.CopyToAsync(stream);

            return "/docs/" + uniqueFileName;
        }

        public void DeletePdf(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            string fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}