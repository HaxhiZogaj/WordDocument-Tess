//using WordDocumentEditor.Models;
//using WordDocumentEditor.Repositories.Interfaces;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

//namespace WordDocumentEditor.Repositories
//{
//    public class TemplateTypesRepository : Repository<DocTemplateType, DocTemplateDBContext>, ITemplateTypesRepository
//    {
//        public TemplateTypesRepository(DocTemplateDBContext context) : base(context)
//        {
//        }

//        public async Task<IEnumerable<DocTemplateType>> GetActiveTemplateTypesAsync()
//        {
//            // Querying only active DocTemplateTypes
//            //   return await dbSet.Where(tt => tt.IsActive).ToListAsync();
//           // return (GetActiveTemplateTypesAsync);
//        }
//    }
//}
