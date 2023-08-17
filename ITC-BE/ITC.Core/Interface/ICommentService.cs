using ITC.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Interface
{
    public interface ICommentService
    {
        Task<ResultModel> CreateTaskComment(CommentUploadApiRequest model);
        Task<ResultModel> GetCommentByTaskId(Guid taskId);
        Task<ResultModel> GetTaskComment();
        Task<ResultModel> DeleteTaskComment(Guid Id);
        Task<ResultModel> UpdateTaskComment(Guid Id, CommentUploadApiRequest model);
        Task<ResultModel> DeleteFileAsync(Guid Id);
    }
}
