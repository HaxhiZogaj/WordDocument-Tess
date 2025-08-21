// Services/Interfaces/IDocumentService.cs
using DBEditor.DBModels;
using System.Threading.Tasks;
using static DocumentEditor.Controllers.DocumenteditorController;

namespace DBEditor.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<string> ImportFileAsync(CustomParams param);
        Task<string> GetDataSourceAsync();
        // You can add more method signatures as needed
    }
}
