using Telerik.Reporting.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FileInfo = Telerik.Reporting.Application.Report.FileInfo;

namespace Telerik.Reporting.Api.Controllers
{
    [ApiController]
    //[Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public ReportController(IReportingService reportingService)
        {
            _reportingService = reportingService;

        }


        [Route("api/Report/PrintAutomatedTest")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PrintAutomatedTest()
        {

            FileInfo report = await _reportingService.GetFileForAutomatedTest();
            return File(report.RenderedBytes, "application/pdf", report.FileName);
        }

    }
}
