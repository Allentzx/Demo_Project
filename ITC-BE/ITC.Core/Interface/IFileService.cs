using System;
using ITC.Core.Utilities.FileUti;
using Microsoft.AspNetCore.Http;

namespace ITC.Core.Interface
{
    public interface IFileService
    {
        void CreateFolder(string folderName);
        string GenerateFileName(string fileName); // Random filename de khong trung
        string GetFileNameWithoutPrevious(string fileName); //Cat phan random de lay file ra
        Task<string> SaveFile(string pathFolder, IFormFile file);
        void ZipFiles(string pathFolder, string pathDestination, string fileName);
        Task<FileSupport> GetFileAsync();
        void DeleteFile(string pathFile);


        //special

        void UploadFile(List<IFormFile> files, string subDirectory);
        (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
        string SizeConverter(long bytes);
    }
}

