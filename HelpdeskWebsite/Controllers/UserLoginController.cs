using HelpdeskViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace HelpdeskWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            int? roleId = await model.AuthenticateUser();

            if (roleId == null)
                return Unauthorized();

            return Ok(new
            {
                message = "Login successful",
                roleId = roleId
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserLoginViewModel model)
        {
            int userId = await model.RegisterUser();
            return Ok(new { message = "User registered successfully", userId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<UserLoginViewModel> allUsers = await new UserLoginViewModel().GetAll();

                // Select only the Id field
                var result = allUsers.Select(user => new { user.Email, user.RoleId }).ToList();

                return Ok(result); // Return only the Ids as JSON

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }


        [HttpPost]
        public async Task<ActionResult> Post(UserLoginViewModel viewmodel)
        {
            try
            {
                await viewmodel.Add();
                return viewmodel.Id > 1
                    ? Ok(new { msg = "User " + viewmodel.Id + " added!" })
                    : Ok(new { msg = "User " + viewmodel.Id + " not added!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPut]
        public async Task<ActionResult> Put(UserLoginViewModel viewmodel)
        {
            try
            {
                int retVal = await viewmodel.Update();
                return retVal switch
                {
                    1 => Ok(new { msg = "User " + viewmodel.Id + " updated!" }),
                    -1 => Ok(new { msg = "User " + viewmodel.Id + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for " + viewmodel.Id + ", Employee not updated!" }),
                    _ => Ok(new { msg = "User " + viewmodel.Id + " not updated!" }),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                UserLoginViewModel viewmodel = new() { Email = email };
                await viewmodel.GetByEmail(); // Assumes you have a GetByEmail method in the ViewModel
                return Ok(viewmodel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); // something went wrong
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            try
            {
                UserLoginViewModel viewmodel = new() { Email = email };
                return await viewmodel.Delete() == 1
                    ? Ok(new { msg = "Employee " + email + " deleted!" })
                    : Ok(new { msg = "Employee " + email + " not deleted!" });
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
