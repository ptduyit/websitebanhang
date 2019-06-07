using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebsiteBanHang.Helpers
{
    public class Files
    {
        public static async Task<List<string>> UploadAsync(List<IFormFile> files, string pathServer)
        {
            string folderName = Path.Combine("Resources", "Images");
            string pathToSave = Path.Combine(pathServer, folderName);
            List<string> imageList = new List<string>();
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    int count = 1;
                    string fileName = Regex.Replace(file.FileName, @"\s+", "-");
                    string fullPath = Path.Combine(pathToSave, fileName);
                    string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                    string extension = Path.GetExtension(fullPath);
                    string newFullPath = fullPath;

                    string dbPath = Path.Combine(folderName, fileName);

                    while (System.IO.File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(pathToSave, tempFileName + extension);

                        dbPath = Path.Combine(folderName, tempFileName + extension);
                    }
                    using (var stream = new FileStream(newFullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }
                    imageList.Add(dbPath);
                }
            }
            return imageList;
        }
        public static bool Delete(string pathfileName, string pathServer)
        {
            string fileDelete = Path.Combine(pathServer, pathfileName);
            if (File.Exists(fileDelete))
            {
                File.Delete(fileDelete);
                return true;
            }
            return false;
        }
    }
}
