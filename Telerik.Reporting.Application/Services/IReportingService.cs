using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telerik.Reporting.Application.Report;

namespace Telerik.Reporting.Application.Services
{
    public interface IReportingService
    {
        Task<FileInfo> GetFileForAutomatedTest();
    }
}
