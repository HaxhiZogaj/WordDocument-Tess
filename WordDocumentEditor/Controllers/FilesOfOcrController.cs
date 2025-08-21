using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace WordDocumentEditor.Controllers
{
    public class FilesOfOcrController : Controller
    {
        [HttpPost]
        public IActionResult ExtractText(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                ViewBag.Message = "No file uploaded.";
                return View("Tess");
            }

            // Path to save the uploaded file
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TessSaved");
            Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

            // Save the uploaded file
            string filePath = Path.Combine(uploadsFolder, uploadedFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                uploadedFile.CopyTo(stream);
            }

            // Path to the tessdata folder
            string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Tessdata");

            try
            {
                // Initialize the Tesseract engine
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                {
                    // Load the uploaded image
                    using (var img = Pix.LoadFromFile(filePath))
                    {
                        // Process the image and extract text
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();
                            ViewBag.ExtractedText = extractedText;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error initializing Tesseract Engine: {ex.Message}";
                return View("Tess");
            }

            ViewBag.Message = "Text extracted successfully!";
            return View("Tess");
        }


        public IActionResult Tess()
        {
            return View();
        }
    }
}

