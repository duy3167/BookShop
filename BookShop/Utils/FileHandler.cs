namespace BookShop.Utils
{
    public class FileHandler
    {
        private static FileHandler _instance;
        public static FileHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileHandler();
                }
                return _instance;
            }
        }
        private FileHandler() { }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetFileName(file.FileName);
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string filePath = Path.Combine(currentDirectory, "wwwroot", folder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return fileName;
                }
                else
                {
                    return "null";
                }
            }
            catch (Exception)
            {
                return "null";
            }
        }

        public bool DeleteFileAsync(string fileName, string folder)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(currentDirectory, "wwwroot", folder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
