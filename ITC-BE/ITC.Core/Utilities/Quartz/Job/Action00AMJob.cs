using System;
using ITC.Core.Data;
using ITC.Core.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace ITC.Core.Utilities.Quartz.Job
{
    public class Action00AMJob : IJob
    {

        private readonly IServiceProvider _provider;

        public Action00AMJob(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var currentTime = DateTime.Now;
            Console.WriteLine("Excute 00AM : " + currentTime);
            if (currentTime.Hour == 0)
            {
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var _projectService = scope.ServiceProvider.GetService<IProjectService>();
                        var _context = scope.ServiceProvider.GetService<ITCDBContext>();

                        await ChangeMilePhase(_context);
                    }
                }
            }
        }

        private async Task ChangeMilePhase(ITCDBContext _context)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var currentTime = DateTime.Now;
                var psChangeDate = await _context.ProjectPhase.Where(x => x.DateEnd < currentTime).ToListAsync();
                var checkPhase = await _context.ProjectPhase.Where(x => x.DateBegin < currentTime && x.DateEnd > currentTime).ToListAsync();

                foreach (var item in psChangeDate)
                {
                    foreach (var phase in checkPhase)
                    {
                        item.PhaseId = phase.PhaseId;
                        item.Status = true;
                    }
                    _context.ProjectPhase.Update(item);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
            
        }
    }
}

