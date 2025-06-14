using HelpdeskViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskWebsite.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class DepartmentController : ControllerBase
        {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                DepartmentViewModel viewmodel = new();
                List<DepartmentViewModel> allDivisions = await viewmodel.GetAll();
                return Ok(allDivisions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }

        }

        } 
}
