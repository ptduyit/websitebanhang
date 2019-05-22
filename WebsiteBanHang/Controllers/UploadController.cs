using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;
        public UploadController(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        // GET: api/Upload
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Upload/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Upload
        [HttpPost]
        public async Task<IActionResult> Post(IFormFileCollection files)
        {
            
            List<string> imageList = new List<string>();
            imageList = await Files.UploadAsync(files,_environment.ContentRootPath);
            return Ok(new { imageList });
        }
        public class DeleteFile
        {
            public List<string> Files { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteFile files)
        {

            //var flag = await Files.Delete(files.Files, _environment.ContentRootPath);
            return Ok();
        }
        //public async Task<List<string>> UploadAsync(IFormFileCollection files)
        //{
        //    string folderName = Path.Combine("Resources", "Images");
        //    string pathToSave = Path.Combine(_environment.ContentRootPath, folderName);
        //    string test = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //    List<string> imageList = new List<string>();

        //    foreach (var file in files)
        //    {
        //        if (file.Length > 0)
        //        {
        //            int count = 1;
        //            string fileName = Regex.Replace(file.FileName, @"\s+", "-");
        //            string fullPath = Path.Combine(pathToSave, fileName);
        //            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
        //            string extension = Path.GetExtension(fullPath);
        //            string newFullPath = fullPath;

        //            string dbPath = Path.Combine(folderName, fileName);

        //            while (System.IO.File.Exists(newFullPath))
        //            {
        //                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
        //                newFullPath = Path.Combine(pathToSave, tempFileName + extension);

        //                dbPath = Path.Combine(folderName, tempFileName + extension);
        //            }
        //            using (var stream = new FileStream(newFullPath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //                await stream.FlushAsync();
        //            }
        //            imageList.Add(dbPath);
        //        }
        //    }
        //    return imageList;
        //}
        // PUT: api/Upload/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{filename}")]
        public void Delete([FromRoute] string filename)
        {
            
        }
    }
}
