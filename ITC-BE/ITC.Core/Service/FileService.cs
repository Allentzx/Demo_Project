using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mime;
using DocumentFormat.OpenXml.Wordprocessing;
using ITC.Core.Interface;
using ITC.Core.Utilities.FileUti;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spire.Xls.Core.Spreadsheet;

namespace ITC.Core.Service
{
    public class FileService : IFileService
    {
        private const char CUT = '~';
        public void CreateFolder(string pathFolder)
        {
            System.IO.Directory.CreateDirectory(pathFolder);
        }

        public void DeleteFile(string pathFile)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), pathFile);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GenerateFileName(string fileName)
        {
            string result = new Random().Next(0, Int32.MaxValue) + DateTime.Now.ToString("dMyyyyhmmss") + CUT + fileName;
            return result;
        }

        public string GetFileNameWithoutPrevious(string fileName)
        {
            string result = fileName.Split(CUT)[fileName.Split(CUT).Length - 1];
            return result;
        }

        public async Task<FileSupport> GetFileAsync()
        {
            string pathFile ="ABC";
            string path = Path.Combine(Directory.GetCurrentDirectory(), pathFile);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return new FileSupport
            {
                Stream = memory,
                FileName = GetFileNameWithoutPrevious(Path.GetFileName(pathFile)),
                ContentType = FileUtils.GetContentType(path)
            };
        }



        public async Task<string> SaveFile(string pathFolder, IFormFile file)
        {
            string filename = GenerateFileName(file.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(),
                               pathFolder, filename);
            try
            {
                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
                return Path.Combine(pathFolder, filename);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ZipFiles(string pathFolder, string pathDestination, string fileName)
        {
            ZipFile.CreateFromDirectory(pathFolder, pathDestination + @"\" + fileName);
        }





        #region Download File as zip
        public (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory)
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Reason", subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        var theFile = archive.CreateEntry(file);
                        using (var streamWriter = new StreamWriter(theFile.Open()))
                        {
                            streamWriter.Write(File.ReadAllText(file));
                        }

                    });
                }

                return ("application/zip", memoryStream.ToArray(), zipName);
            }

        }
        #endregion

        #region Size Converter
        public string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
        #endregion


        #region Upload File as Sub folder
        public void UploadFile(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(Directory.GetCurrentDirectory(), "Reason", subDirectory);

            Directory.CreateDirectory(target);

            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;
                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            });
        }
        #endregion
    }
}

