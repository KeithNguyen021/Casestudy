using HelpdeskViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                RoleViewModel viewmodel = new() { Id = id };
                await viewmodel.GetById();

                if (viewmodel.RoleName == "not found")
                    return NotFound(new { message = $"Role with Id {id} not found." });

                return Ok(viewmodel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Problem in {GetType().Name} {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");

                // Return a more detailed error response
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An internal error occurred.",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                RoleViewModel viewmodel = new();
                List<RoleViewModel> allProblems = await viewmodel.GetAll(); // Fetch all employees
                return Ok(allProblems); // Return the list of employees as JSON
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }


    }
}
