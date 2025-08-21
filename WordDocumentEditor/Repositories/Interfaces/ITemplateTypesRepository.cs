using Syncfusion.EJ2.Schedule;  // If you need this for scheduling functionality
using WordDocumentEditor.Models;  // Assuming TemplateType is part of your models
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordDocumentEditor.Repositories.Interfaces
{
    public interface ITemplateTypesRepository : IRepository<DocTemplateType>
    {
        // Custom method to fetch active template types
        Task<IEnumerable<DocTemplateType>> GetActiveTemplateTypesAsync();
    }
}
