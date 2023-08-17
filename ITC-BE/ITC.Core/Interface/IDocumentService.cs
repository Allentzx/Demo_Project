using ITC.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Interface
{
    public interface IDocumentService
    {
        Task<ResultModel> CreateDocument(DocumentUploadApiRequest model);
        Task<ResultModel> GetAllDocument();
        Task<ResultModel> GetDetailDocument(Guid Id);
        Task<DocumentResponse> GetContent(Guid id);
        Task<ResultModel> GetDetailDocumentByProjectId(Guid projectId);
    }
}
