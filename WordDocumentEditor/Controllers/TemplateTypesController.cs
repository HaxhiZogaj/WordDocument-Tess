using Microsoft.AspNetCore.Mvc;
using WordDocumentEditor.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WordDocumentEditor.Controllers
{
    public class TemplateTypesController : Controller
    {
        private readonly DocTemplateDBContext _context;

        public TemplateTypesController(DocTemplateDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var templateTypes = _context.DocTemplateTypes.ToList();

          
            return View(templateTypes);
        }

        [HttpPost]
        public IActionResult Add([FromBody] DocTemplateType model)
        {
            if (ModelState.IsValid)
            {
                _context.DocTemplateTypes.Add(model);
                _context.SaveChanges();

                return Json(new { success = true, message = "Template added successfully", templateTypeId = model.TemplateTypeId });
            }

            return Json(new { success = false, message = "There was an error while adding the template." });
        }

        [HttpPost]
        public IActionResult Update([FromBody] DocTemplateType model)
        {
            if (ModelState.IsValid)
            {
                var template = _context.DocTemplateTypes.Find(model.TemplateTypeId);
                if (template != null)
                {
                    template.Name = model.Name;
                    template.Description = model.Description;
                    template.IsActive = model.IsActive;  // Fix for IsActive handling

                    _context.SaveChanges();
                    return Json(new { success = true, message = "Template updated successfully" });
                }

                return Json(new { success = false, message = "Template not found" });
            }

            return Json(new { success = false, message = "There was an error while updating the template." });
        }

        [HttpPost]
        public IActionResult Delete([FromBody] int templateTypeId)
        {
            var template = _context.DocTemplateTypes.Find(templateTypeId);
            if (template != null)
            {
                _context.DocTemplateTypes.Remove(template);
                _context.SaveChanges();
                return Json(new { success = true, message = "Template deleted successfully" });
            }

            return Json(new { success = false, message = "Template not found" });
        }
    }
}
