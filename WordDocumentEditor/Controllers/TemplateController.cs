using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WordDocumentEditor.Models;

namespace WordDocumentEditor.Controllers
{
    public class TemplateController : Controller
    {
        private readonly DocTemplateDBContext _context;

        public TemplateController(DocTemplateDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var docTemplates = _context.DocTemplates.ToList();
            ViewBag.TemplateTypes = _context.DocTemplateTypes
                .Select(t => new { t.TemplateTypeId, t.Name })
                .ToList();
            return View(docTemplates);
        }

        [HttpPost]
        public IActionResult Add([FromBody] DocTemplate docTemplate)
        {
            if (docTemplate == null)
                return Json(new { success = false, message = "Invalid data" });

            try
            {
                _context.DocTemplates.Add(docTemplate);
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Template added successfully",
                    templateId = docTemplate.TemplateId
                });
            }
            catch (Exception ex)
            {
                // Log inner exception details
                var innerException = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = $"Error adding template: {innerException}" });
            }
        }

        [HttpPost]
        public IActionResult Update([FromBody] DocTemplate docTemplate)
        {
            if (docTemplate == null || !_context.DocTemplates.Any(t => t.TemplateId == docTemplate.TemplateId))
                return Json(new { success = false, message = "Invalid data" });

            try
            {
                var existingTemplate = _context.DocTemplates.FirstOrDefault(t => t.TemplateId == docTemplate.TemplateId);
                if (existingTemplate != null)
                {
                    existingTemplate.TemplateCode = docTemplate.TemplateCode;
                    existingTemplate.Name = docTemplate.Name;
                    existingTemplate.Description = docTemplate.Description;
                    existingTemplate.TemplateTypeId = docTemplate.TemplateTypeId;
                    existingTemplate.Version = docTemplate.Version;
                    existingTemplate.TemplateFilePath = docTemplate.TemplateFilePath;

                    _context.SaveChanges();
                }

                return Json(new { success = true, message = "Template updated successfully" });
            }
            catch (Exception ex)
            {
                // Log inner exception details
                var innerException = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = $"Error updating template: {innerException}" });
            }
        }

        [HttpPost]
        public IActionResult Delete([FromBody] int templateId)
        {
            try
            {
                var template = _context.DocTemplates.FirstOrDefault(t => t.TemplateId == templateId);
                if (template != null)
                {
                    _context.DocTemplates.Remove(template);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Template deleted successfully" });
                }

                return Json(new { success = false, message = "Template not found" });
            }
            catch (Exception ex)
            {
                // Log inner exception details
                var innerException = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = $"Error deleting template: {innerException}" });
            }
        }
    }
}
