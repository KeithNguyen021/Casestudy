using Microsoft.AspNetCore.Mvc;
using HelpdeskWebsite.Reports;
namespace ExercisesWebsite.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ReportController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [Route("api/employeereport")]
        [HttpGet]
        public IActionResult GetHelloReport()
        {
            EmployeeReport hello = new();
            hello.GenerateReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }
    }


    
}