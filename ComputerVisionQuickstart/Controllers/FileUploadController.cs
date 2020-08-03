using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ComputerVisionQuickstart.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ComputerVisionQuickstart.Controllers
{
    [Route("FileUpload")]
    [ApiController]
    public class FileUploadController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        public FileUploadController(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _configuration = configuration;
            webHostEnvironment = hostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            //long size = files.Sum(f => f.Length);

            List<string> filePaths = new List<string>();

            List<ResponseModel> Result = new List<ResponseModel>();

            foreach (IFormFile formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    //var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
                    //filePaths.Add(targetPath);

                    string extension;

                    extension = Path.GetExtension(formFile.FileName);

                    string filename;

                    filename = Path.GetFileNameWithoutExtension(formFile.FileName);

                    string newFileName = filename + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Documents", newFileName);

                    string targetPath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", newFileName);

                    filePaths.Add(targetPath);

                    /* -------------------------------------------------------- URL -------------------------------------------------------- */

                    ComputerVisionExtension Obj = new ComputerVisionExtension(_configuration);

                    //var ResultAnalyzeFileByUrl = await Obj.AnalyzeFileByUrl("https://www.panama24horas.com.pa/wp-content/uploads/2018/01/cedula-inteligente-690x449.jpg");

                    /* -------------------------------------------------------- URL -------------------------------------------------------- */

                    /* ------------------------------------------------------ STREAM -------------------------------------------------------- */

                    var ResultAnalyzeFileByStream = await Obj.AnalyzeFileByStream(formFile.OpenReadStream());

                    /* ------------------------------------------------------ STREAM -------------------------------------------------------- */

                    using (var stream = new FileStream(uploadsFolder, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    Result.Add(new ResponseModel() { FileName = newFileName, PathFile = uploadsFolder, ResponseOCR = ResultAnalyzeFileByStream });
                }
            }

            //return Ok(new { count = files.Count, size, filePaths });

            //ViewData["LoadData"] = Result;

            TempData["LoadData"] = JsonConvert.SerializeObject(Result);

            //TempDataExtensions.Put<ResponseModel>(TempData["LoadData"], "LoadData", Result);

            return RedirectToAction("Privacy", "Home"); ;
        }

        public IActionResult FileSave()
        {
            return View();
        }
    }
}
