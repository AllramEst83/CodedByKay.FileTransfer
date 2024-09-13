using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace CodedByKay.FileTransfer.Controllers
{
    public class FileController : Controller
    {
        private readonly string _uploadFolder = "~/App_Data/Uploads/";
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // GET: Upload
        [HttpGet]
        public ActionResult Upload()
        {
            if (HttpContext.Session.GetString("IsAuthenticated") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "App_Data/Uploads");

                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate a dynamic URL
                var downloadUrl = Url.Action("Download", "File", new { fileName = fileName, token = GenerateToken(fileName) }, protocol: Request.Scheme);

                ViewBag.Message = "File uploaded successfully!";
                ViewBag.DownloadUrl = downloadUrl;
            }

            return View();
        }



        // GET: Download
        [HttpGet]
        public IActionResult Download(string fileName, string token)
        {
            if (!ValidateToken(fileName, token))
            {
                return Forbid(); // Use Forbid() instead of HttpStatusCodeResult
            }

            var filePath = Path.Combine(_environment.WebRootPath, "App_Data/Uploads", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }

        private string GenerateToken(string fileName)
        {
            // Simple token generation (hashing the file name + a secret key)
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fileName + "secretKey"));
        }

        private bool ValidateToken(string fileName, string token)
        {
            var validToken = GenerateToken(fileName);
            return token == validToken;
        }
    }
}
