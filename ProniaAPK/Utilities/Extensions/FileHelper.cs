using ProniaAPK.Utilities.Enums;

namespace ProniaAPK.Utilities.Extensions
{
    public static class FileHelper
    {

        public static bool CheckFileType(this IFormFile file, string type)
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }

        public static bool CheckFileSize(this IFormFile file, FileSize sizeType, int size)
        {
            switch (sizeType)
            {
                case FileSize.KB:
                    if (file.Length < size * 1024) return true;
                    break;
                case FileSize.MB:
                    if (file.Length < size * 1024 * 1024) return true;
                    break;
                case FileSize.GB:
                    if (file.Length < size * 1024 * 1024 * 1024) return true;
                    break;
                default:
                    break;
            }
            return false;
        }


        public static async Task<String> CreateFileAsync(this IFormFile Photo, params string[] roots)
        {
            int lastDotIndex = Photo.FileName.LastIndexOf(".");
            string fileName = Guid.NewGuid().ToString().Substring(0, 15) + Photo.FileName.Substring(lastDotIndex, (Photo.FileName.Length - lastDotIndex));
            using (FileStream fileStream = new(fileName.GeneratePath(roots), FileMode.Create))
            {
                await Photo.CopyToAsync(fileStream);
            }
            return fileName;
        }


        public static void DeleteFile(this string fileName, params string[] roots)
        {

            File.Delete(fileName.GeneratePath(roots));
        }
        private static string GeneratePath(this string fileName, params string[] roots)
        {
            string path = String.Empty;
            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }
            path = Path.Combine(path, fileName);
            return path;
        }

    }
}
