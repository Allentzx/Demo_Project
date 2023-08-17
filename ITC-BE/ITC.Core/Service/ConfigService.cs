using System;
using System.Text.RegularExpressions;
using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using ITC.Core.Data;
using ITC.Core.Interface;
using ITC.Core.Model;
using ITC.Core.Model.Commom;
using ITC.Core.Utilities;
using ITC.Core.Utilities.ExcelEx;
using ITC.Data.Entities;
using ITC.Data.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ITC.Core.Service
{
    public class ConfigService : IConfigService
    {
        private readonly ITCDBContext _context;
        private readonly IMapper _mapper;
        public ConfigService(ITCDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResultModel> ImportConfig(IFormFile formFile)
        {
            var result = new ResultModel();
            try
            {
                var fileextension = Path.GetExtension(formFile.FileName);
                var filename = Guid.NewGuid().ToString() + fileextension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "ConfigProject", filename);
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    formFile.CopyTo(fs);
                }
                int rowno = 1;
                XLWorkbook workbook = XLWorkbook.OpenFromTemplate(filepath);
                var sheets = workbook.Worksheets.First();
                var rows = sheets.Rows().ToList();
                foreach (var row in rows)
                {
                    if (rowno != 1)
                    {
                       
                        ConfigProject configProject;
                        configProject = _context.ConfigProject.Where(s => s.Id == null).FirstOrDefault();
                        if (configProject != null)
                        {
                            configProject = new ConfigProject()
                            {
                                Id = Guid.NewGuid(),
                                MinStudent = row.Cell(1).Value.ToString(),
                                MaxStudent = row.Cell(2).Value.ToString(),
                                MinStaff = row.Cell(3).Value.ToString(),
                                MaxStaff = row.Cell(4).Value.ToString(),
                                TermTime = row.Cell(5).Value.ToString(),
                                TotalProjectLeader = row.Cell(6).Value.ToString(),
                            };
                            await _context.ConfigProject.AddAsync(configProject);
                        }
                        else
                        {
                            configProject.MinStudent = row.Cell(1).Value.ToString();
                            configProject.MaxStudent = row.Cell(2).Value.ToString();
                            configProject.MinStaff = row.Cell(3).Value.ToString();
                            configProject.MaxStaff = row.Cell(4).Value.ToString();
                            configProject.TermTime = row.Cell(5).Value.ToString();
                            configProject.TotalProjectLeader = row.Cell(6).Value.ToString();
                            _context.ConfigProject.Update(configProject);
                        }
                        await _context.SaveChangesAsync();

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
    }
}

