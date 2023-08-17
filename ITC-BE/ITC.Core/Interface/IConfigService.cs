using ITC.Core.Model;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Interface
{
    public interface IConfigService
    {
        Task<ResultModel> ImportConfig(IFormFile formFile);
    }
}