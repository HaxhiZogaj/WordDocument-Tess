using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using WordDocumentEditor.Models;

namespace WordDocument.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DocTemplateDBContext _context;
        private string connectionString;

        public HomeController(ILogger<HomeController> logger, DocTemplateDBContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            this.connectionString = config.GetConnectionString("DocContext");

        }

        public IActionResult Index()
        {
            var descriptions = _context.DocTemplates
                                        .Select(d => d.Description)
                                        .ToList();

            ViewData["DataSource"] = descriptions;

            return View();
        }

        //[HttpGet]
        //public IActionResult List()
        //{
        //    try
        //    {
        //        // Return the listB.cshtml view
        //        return View("listB");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return StatusCode(500, "Internal Server Error");
        //    }
        //}

        // Controller Action for listB
        public IActionResult List()
        {
            var descriptions = _context.DocTemplates.Select(d => d.Description).ToList();
            ViewData["DataSource"] = descriptions;
            return View("listB");
        }



        public IActionResult DBEditor()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DBEList()
        {
            try
            {
                // Return the listB.cshtml view
                return View("DBEList");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Invalid file.");
                return View("Index");
            }

            MemoryStream stream = new MemoryStream();
            file.CopyTo(stream);
            string fileName = file.FileName;
            stream.Position = 0;
            string tableName = "Document";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string uploadDocument = "Insert into \"" + tableName + "\" (FileName, Blob) VALUES (@fileName,@blob)";
                var command = new SqlCommand(uploadDocument, connection);
                command.Parameters.Add("@fileName", SqlDbType.NVarChar).Value = fileName;
                command.Parameters.Add("@blob", SqlDbType.VarBinary).Value = stream != null ? stream.ToArray() : new byte[0];
                command.ExecuteNonQuery();
            }
            return View("DBEditor");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
