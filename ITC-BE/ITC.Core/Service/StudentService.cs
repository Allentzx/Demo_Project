using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClosedXML.Excel;
using Google.Apis.Auth;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Model.Commom;
using ITC.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace ITC.Core.Service
{
    public class StudentService : IStudentService
    {
        private readonly ITCDBContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AzureBlobStorageService> _logger;
        private readonly IUserContextService _userContextService;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly IAzureBlobStorageService _storage;

        public StudentService(ITCDBContext context, IMapper mapper, IJwtTokenService jwtTokenService, IConfiguration configuration
            , IUserContextService userContextService, ILogger<AzureBlobStorageService> logger, IAzureBlobStorageService storage,
            IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
            _storageConnectionString = configuration["BlobConnectionString"];
            _storageContainerName = configuration["BlobContainerName"];
            _logger = logger;
            _userContextService = userContextService;
            _storage = storage;
            _env = env;
        }



        public async Task<ResultModel> GoogleAuthenticateStudent(GoogleJsonWebSignature.Payload user)
        {
            var result = new ResultModel();
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Student.FirstOrDefaultAsync(x => x.Email == user.Email);

                if (student is null)
                {
                    var newStudent = new Student
                    {
                        Id = Guid.NewGuid(),
                        Email = user.Email,
                        FullName = user.Name,
                        Status = true,
                    };
                    await _context.Student.AddAsync(newStudent);
                    await _context.SaveChangesAsync();
                    student = newStudent;

                }
                result.IsSuccess = true;
                result.Code = 200;
                //result.ResponseSuccess = _jwtTokenService.GenerateTokenStudent(student);
                result.ResponseSuccess = new StudentAuthenModel
                {
                    Id = student.Id,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    Address = student.Address,
                    FullName = student.FullName,
                    Batch = student.Batch,
                    MajorName = student.MajorName,
                    MemberCode = student.MemberCode,
                    OldRollNumber = student.OldRollNumber,
                    RollNumber = student.RollNumber,
                    Status = student.Status,
                    MajorId = student.MajorId,
                    TokenToken = _jwtTokenService.GenerateTokenStudent(student),
                };
                await transaction.CommitAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }
        public async Task<ResultModel> ImportStudent(ImportStudentModel model)
        {
            var result = new ResultModel();
            try
            {
                //var fileextension = Path.GetExtension(model.formFile?.FileName);
                //var filename = Guid.NewGuid().ToString() + fileextension;
                var filepath = Path.Combine(
                _env.ContentRootPath, "Excel", "StudentTemplate.xlsx");
                // using (FileStream fs = System.IO.File.Create(filepath))
                // {
                //     model.formFile?.CopyTo(fs);
                // }
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await model.formFile.CopyToAsync(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }
                int rowno = 1;
                XLWorkbook workbook = XLWorkbook.OpenFromTemplate(filepath);
                var sheets = workbook.Worksheets.First();
                var rows = sheets.Rows().ToList();
                foreach (var row in rows)
                {
                    if (rowno != 1)
                    {
                        var test = row.Cell(3).Value.ToString();
                        if (string.IsNullOrWhiteSpace(test) || string.IsNullOrEmpty(test))
                        {
                            break;
                        }
                        Student? student;
                        student = _context.Student.Where(s => s.Email == row.Cell(9).Value.ToString()
                                                                && s.RollNumber == row.Cell(1).Value.ToString()
                                                                && s.MemberCode == row.Cell(2).Value.ToString()
                                                                && s.PhoneNumber == row.Cell(10).Value.ToString()).FirstOrDefault();
                        var major = await _context.Major.Where(x => x.Name == row.Cell(5).Value.ToString()).FirstOrDefaultAsync();
                        if(major == null)
                        {
                            var newMajor = new Major
                            {
                                Id = Guid.NewGuid(),
                                Name = row.Cell(5).Value.ToString(),
                                Status = true
                            };
                            major = newMajor;
                            await _context.Major.AddAsync(newMajor);
                            await _context.SaveChangesAsync();
                        }
                        if (!Regex.IsMatch(row.Cell(9).Value.ToString(), @"^([a-zA-Z0-9_\-\.]+)@(fpt.edu.vn)$"))
                        {
                            result.Code = 400;
                            result.IsSuccess = false;
                            result.ResponseFailed = "Email must be right format!";
                            return result;
                        }
                        if (student == null)
                        {
                            var checkmail = await _context.Student.Where(x => x.Email == row.Cell(9).Value.ToString()).FirstOrDefaultAsync();
                            var checkPhone = await _context.Student.Where(x => x.PhoneNumber == row.Cell(10).Value.ToString()).FirstOrDefaultAsync();
                            var checkRollNumber = await _context.Student.Where(x => x.RollNumber == row.Cell(1).Value.ToString()).FirstOrDefaultAsync();
                            var checkmember = await _context.Student.Where(x => x.MemberCode == row.Cell(2).Value.ToString()).FirstOrDefaultAsync();

                            if (checkmail != null)
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.ResponseFailed = $"Student with Email: {row.Cell(9).Value.ToString()}  existed!!";
                                return result;
                            };

                            if (checkPhone != null)
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.ResponseFailed = $"Student with PhoneNumber: {row.Cell(10).Value.ToString()} nexisted!!";
                            };
                            if (checkRollNumber != null)
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.ResponseFailed = $"Student with RollNumber: {row.Cell(1).Value.ToString()}  existed!!";
                                return result;
                            };

                            if (checkmember != null)
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.ResponseFailed = $"Student with MemberCode: {row.Cell(2).Value.ToString()}  existed!!";
                            };

                            student = new Student()
                            {
                                Id = Guid.NewGuid(),
                                RollNumber = row.Cell(1).Value.ToString(),
                                MemberCode = row.Cell(2).Value.ToString(),
                                OldRollNumber = row.Cell(3).Value.ToString(),
                                FullName = row.Cell(4).Value.ToString(),
                                MajorName = row.Cell(5).Value.ToString(),
                                Batch = row.Cell(6).Value.ToString(),
                                Semeter = row.Cell(7).Value.ToString(),
                                StudentStatus = row.Cell(8).Value.ToString(),
                                Email = row.Cell(9).Value.ToString(),
                                PhoneNumber = row.Cell(10).Value.ToString(),
                                Address = row.Cell(11).Value.ToString(),
                                Status = true,
                                MajorId = major.Id,
                            };
                            await _context.Student.AddAsync(student);
                            await _context.SaveChangesAsync();
                        }
                        if (student != null)
                        {
                            student.RollNumber = row.Cell(1).Value.ToString();
                            student.MemberCode = row.Cell(2).Value.ToString();
                            student.OldRollNumber = row.Cell(3).Value.ToString();
                            student.FullName = row.Cell(4).Value.ToString();
                            student.MajorName = row.Cell(5).Value.ToString();
                            student.Batch = row.Cell(6).Value.ToString();
                            student.Semeter = row.Cell(7).Value.ToString();
                            student.StudentStatus = row.Cell(8).Value.ToString();
                            student.Email = row.Cell(9).Value.ToString();
                            student.PhoneNumber = row.Cell(10).Value.ToString();
                            student.Address = row.Cell(11).Value.ToString();
                            student.Status = true;
                            student.MajorId = major.Id;
                            _context.Student.Update(student);
                            await _context.SaveChangesAsync();
                        }
                        result.IsSuccess = true;
                        result.Code = 200;
                    }
                    else
                    {
                        rowno++;
                    }
                }

            }
            catch (Exception e)
            {
                //await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            finally
            {
            }

            return result;
        }


        public async Task<ResultModel> GetStudent()
        {
            var result = new ResultModel();
            try
            {
                var std = _context.Student;

                if (std == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Student Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Student.ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetDetailStudent(Guid Id)
        {
            var result = new ResultModel();
            try
            {
                var task = _context.Student.Where(x => x.Id == Id);

                if (task == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Student Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = await _context.Student.Include(x => x.Major).Where(x => x.Id == Id).ToListAsync();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> CreateStudent(CreateStudentModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!Regex.IsMatch(model.Email, @"^([a-zA-Z0-9_\-\.]+)@(fpt.edu.vn)$"))
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Email must be right format fpt.edu.vn!";
                    return result;
                }
                var checkmail = await _context.Student.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                var checkPhone = await _context.Student.Where(x => x.PhoneNumber == model.PhoneNumber).FirstOrDefaultAsync();
                var checkRollNumber = await _context.Student.Where(x => x.RollNumber == model.RollNumber).FirstOrDefaultAsync();
                var checkmember = await _context.Student.Where(x => x.MemberCode == model.MemberCode).FirstOrDefaultAsync();

                if (checkmail != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Student with Email: {model.Email} existed!!";
                    return result;
                };

                if (checkPhone != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Student with PhoneNumber: {model.PhoneNumber} existed!!";
                };
                if (checkRollNumber != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Student with RollNumber: {model.RollNumber} existed!!";
                    return result;
                };

                if (checkmember != null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Student with MemberCode: {model.MemberCode} existed!!";
                };

                var std = new Student
                {
                    Id = Guid.NewGuid(),
                    RollNumber = model.RollNumber,
                    MemberCode = model.MemberCode,
                    OldRollNumber = model.OldRollNumber,
                    FullName = model.FullName,
                    MajorName = model.MajorName,
                    Batch = model.Batch,
                    Semeter = model.Semeter,
                    StudentStatus = model.UpStatus,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Status = true,
                    GradingUrl = model.GradingUrl,
                    MajorId = model.MajorId,
                };

                await _context.Student.AddAsync(std);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Code = 200;

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> DeleteStudent(Guid Id)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var std = await _context.Student.FirstOrDefaultAsync(x => x.Id == Id);

                if (std == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = $"Student with id: {Id} not existed!!";
                    return result;
                }

                _context.Student.Remove(std);
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            await transaction.CommitAsync();
            return result;
        }

        public async Task<ResultModel> UpdateStudent(Guid Id, UpdateStudentModel model)
        {
            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var student = await _context.Student.FindAsync(Id);
                if (!Regex.IsMatch(model.Email, @"^([a-zA-Z0-9_\-\.]+)@(fpt.edu.vn)$"))
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseFailed = "Email must be right format fpt.edu.vn!";
                    return result;
                }
                if (student == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = "Not Found";
                    return result;
                }
                var major = await _context.Major.Where(x => x.Id == model.MajorId).FirstOrDefaultAsync();
                var checkmail = await _context.Student.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                var checkPhone = await _context.Student.Where(x => x.PhoneNumber == model.PhoneNumber).FirstOrDefaultAsync();
                var checkRollNumber = await _context.Student.Where(x => x.RollNumber == model.RollNumber).FirstOrDefaultAsync();
                var checkmember = await _context.Student.Where(x => x.MemberCode == model.MemberCode).FirstOrDefaultAsync();

                if (model.RollNumber != null)
                {
                    student.RollNumber = model.RollNumber;
                }
                if (model.MemberCode != null)
                {
                    student.MemberCode = model.MemberCode;
                }
                if (model.Email != null)
                {
                    student.Email = model.Email;
                }
                if (model.PhoneNumber != null)
                {
                    student.PhoneNumber = model.PhoneNumber;
                }


                student.OldRollNumber = model.OldRollNumber;
                student.FullName = model.FullName;
                if (major != null)
                {
                    student.MajorName = major.Name;
                }
                student.Batch = model.Batch;
                student.StudentStatus = model.UpStatus;
                student.Address = model.Address;
                student.Status = model.Status;
                student.Semeter = model.Semeter;
                student.MajorId = model.MajorId;

                _context.Student.Update(student);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                await transaction.CommitAsync();
                result.ResponseSuccess = await _context.Student.FirstOrDefaultAsync(); ;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }



        public async Task<ResultModel> UploadGrading(Guid studentId, IFormFile? FormFile)
        {
            var result = new ResultModel();
            UploadResponse response = new();
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            var transaction = _context.Database.BeginTransaction();
            try
            {
                //add Entity
                var checkerStudent = await _context.Student.FirstOrDefaultAsync(st => st.Id == studentId);
                if (checkerStudent != null)
                {

                    if (FormFile != null)
                    {
                        var co = from n in _context.FileTracking
                                 where n.FileName == FormFile.FileName
                                 select n;
                        int num = co.Count();
                        await container.CreateIfNotExistsAsync();
                        var fileextension = Path.GetExtension(FormFile?.FileName);
                        var filename = FormFile?.FileName;
                        var newFileName = FormFile?.FileName.Split(".").First() + " " + "(" + (num + 1) + ")" + "." + FormFile?.FileName.Split(".").Last();
                        BlobClient client = container.GetBlobClient(filename);
                        if (client.Exists())
                        {
                            client = container.GetBlobClient(newFileName);
                        }
                        await using (Stream? data = FormFile?.OpenReadStream())
                        {
                            await client.UploadAsync(data);
                        }
                        response.Status = $"File {FormFile?.FileName} Uploaded Successfully";
                        response.Error = false;
                        response.Uri = client.Uri.AbsoluteUri;
                        response.Name = client.Name;
                        var FileTracking = new FileTracking
                        {
                            TraceId = studentId,
                            FileName = response.Name,
                            FileExtension = FormFile?.FileName.Split(".").Last(),
                            FileUrl = response.Uri,
                            Owner = _userContextService.FullName,
                            DateUpLoad = DateTime.Now
                        };
                        await _context.FileTracking.AddAsync(FileTracking);
                    }
                    checkerStudent.GradingUrl = response.Uri;
                    _context.Student.Update(checkerStudent);
                    await _context.SaveChangesAsync();

                    result.Code = 200;
                    result.IsSuccess = true;
                    await transaction.CommitAsync();
                }
            }
            catch (RequestFailedException ex)
               when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"File with name {FormFile?.FileName} already exists in container. Set another name to store the file in the container: '{_storageContainerName}.'");
                response.Status = $"File with name {FormFile?.FileName} already exists. Please use another name to store your file.";
                response.Error = true;
                result.IsSuccess = false;
                result.ResponseFailed = "Document already exists. Please use another name to store your file";
            }
            catch (Exception e)
            {

                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }
        public async Task<ResultModel> GetContent(Guid studentId)
        {

            var result = new ResultModel();
            try
            {
                var student = await _context.Student.FirstOrDefaultAsync(x => x.Id == studentId);

                if (student == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Student Not Found!";
                    return result;
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = student.GradingUrl;

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<FileModel> ExportStudentToExcel()
        {
            // Get the user list 
            var result = new FileModel();
            List<Student> data = await _context.Student.ToListAsync();

            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("ListStudent");
                var namedStyle = xlPackage.Workbook.Styles.CreateNamedStyle("HyperLink");
                namedStyle.Style.Font.UnderLine = true;
                const int startRow = 2;
                var row = startRow;
                byte[] fileContents = null;

                worksheet.Cells["A1"].Value = "RollNumber";
                worksheet.Cells["B1"].Value = "MemberCode";
                worksheet.Cells["C1"].Value = "OldRollNumber";
                worksheet.Cells["D1"].Value = "FullName";
                worksheet.Cells["E1"].Value = "MajorName";
                worksheet.Cells["F1"].Value = "Batch";
                worksheet.Cells["G1"].Value = "Semeter";
                worksheet.Cells["H1"].Value = "StudentStatus";
                worksheet.Cells["I1"].Value = "Email";
                worksheet.Cells["J1"].Value = "Address";

                foreach (var s in data)
                {
                    worksheet.Cells[row, 1].Value = s.RollNumber;
                    worksheet.Cells[row, 2].Value = s.MemberCode;
                    worksheet.Cells[row, 3].Value = s.OldRollNumber;
                    worksheet.Cells[row, 4].Value = s.FullName;
                    worksheet.Cells[row, 5].Value = s.MajorName;
                    worksheet.Cells[row, 6].Value = s.Batch;
                    worksheet.Cells[row, 7].Value = s.Semeter;
                    worksheet.Cells[row, 8].Value = s.StudentStatus;
                    worksheet.Cells[row, 9].Value = s.Email;
                    worksheet.Cells[row, 10].Value = s.PhoneNumber;
                    worksheet.Cells[row, 11].Value = s.Address;
                    row++;
                }

                // set some core property values
                xlPackage.Workbook.Properties.Title = "Student List";
                xlPackage.Save();
                // Response.Clear();
                fileContents = stream.ToArray();
                var blobServiceClient = new BlobServiceClient(_storageConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_storageContainerName);
                string fileName = "data.xlsx";
                var blobClient = containerClient.GetBlobClient(fileName);
                if (fileName != null)
                {
                    BlobResponseDto response = await _storage.DeleteAsync(fileName);
                }
                await blobClient.UploadAsync(new MemoryStream(fileContents));
                AzureBlobStorageModel? file = await _storage.DownloadAsync(fileName);
                file.ContentType = " application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                result.FileName = fileName;
                result.Id = "data";
                result.Data = fileContents;
                result.FileType = file.ContentType;
            }
            return result;
        }

        public async Task<ResultModel> GetGradingStudentId(Guid studentId)
        {
            var result = new ResultModel();
            try
            {

                var students = await _context.Student.Where(x => x.Id == studentId).ToListAsync();
                var rsFile = (from a in _context.Student.Where(x => x.Id == studentId)
                              join c in _context.FileTracking on a.Id equals c.TraceId
                              select new { a.GradingUrl, c.FileName, c.FileExtension });

                if (students == null)
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.ResponseSuccess = $"Any Grading Not Found!";
                    return result;
                }


                result.Code = 200;
                result.IsSuccess = true;
                result.ResponseSuccess = rsFile;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> DeleteGrading(Guid studentId)
        {

            var result = new ResultModel();
            var transaction = _context.Database.BeginTransaction();

            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            try
            {
                var conf = await _context.FileTracking.FirstOrDefaultAsync(x => x.TraceId == studentId);
                var student = await _context.Student.FirstOrDefaultAsync(x => x.Id == studentId);
                var fileName = conf.FileName;
                BlobClient file = client.GetBlobClient(fileName);
                await file.DeleteAsync();
                if (student != null)
                {
                    student.GradingUrl = null;
                    _context.Student.Update(student);
                }
                if (conf != null)
                {
                    _context.FileTracking.Remove(conf);
                }
                await _context.SaveChangesAsync();

                result.Code = 200;
                result.ResponseSuccess = "Succesfull";
                result.IsSuccess = true;
                await transaction.CommitAsync();

            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.IsSuccess = false;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }
    }
}
