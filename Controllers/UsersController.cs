using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Task_Mangment_Api.AuthenticationServices;
using Task_Mangment_Api.DTO;
using Task_Mangment_Api.Helpers;
using Task_Mangment_Api.Models;

namespace Task_Mangment_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration configuration;
        private readonly JWT _jwt;
        public UsersController(AppDbContext context, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IOptions<JWT> jwt)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            this.configuration = configuration;
            _jwt = jwt.Value;

        }
        //register endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegistirationModel registrationModel)
        {
            try
            {
                if(await _userManager.FindByEmailAsync(registrationModel.Email) is not null || await _userManager.FindByNameAsync(registrationModel.Name) is not null) 
                {
                    return BadRequest("Invalid registration data.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid registration data.");
                }

                using var dataStream = new MemoryStream();
                await registrationModel.ProfilePicture.CopyToAsync(dataStream);

                var user = new User
                {
                    UserName = registrationModel.Name,
                    Name = registrationModel.Name,
                    PhoneNumber = registrationModel.Phone,
                    Email = registrationModel.Email,
                    ProfilePicture = dataStream.ToArray()
                };

                await _userManager.AddToRoleAsync(user, "User");


                var result = await _userManager.CreateAsync(user,registrationModel.Password);
                if (!result.Succeeded) 
                {
                    var errors = string.Empty;
                    foreach(var error in result.Errors)
                    {
                        errors += $"{error.Description},";
                    }
                }
                var jwtToken = new JWTauthentication(_userManager, _jwt).CreateJwtToken(user);
                _context.SaveChanges();

                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginModel)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginModel.UserName);

                if (user == null || !(await _userManager.CheckPasswordAsync(user, loginModel.Password)))
                {
                    return Unauthorized("Invalid password.");
                }
                
                var jwtToken = new JWTauthentication(_userManager, _jwt).CreateJwtToken(user);
                _context.SaveChanges();



                var tokenResponse = new
                {
                    Token = jwtToken
                };

                return Ok(tokenResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while logging in.");
            }
        }

        /*[HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(string Role)
        {
            var newRole = new IdentityRole(Role);
            if (await _roleManager.RoleExistsAsync(Role))
            {
                return BadRequest("Role is already exist!");
            }
            var result = await _roleManager.CreateAsync(newRole);

            return Ok();
        }*/
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUserFromUser()
        {
          
            var user = _userManager.Users;

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    
        [HttpGet("/Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUser()
        {
            var user = await _userManager.Users.Include(u=>u.UserTasks).ThenInclude(ut=>ut.Task).ToListAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] UserDTO DTO)
        {
            //var user = _mapper.Map<User>(DTO);
            using var dataStream = new MemoryStream();
            await DTO.ProfilePicture.CopyToAsync(dataStream);

            var user = new User
            {
                UserName = DTO.Name,
                Name = DTO.Name,
                PhoneNumber = DTO.Phone,
                Email = DTO.Email,
                ProfilePicture = dataStream.ToArray()
            };
            var result = await _userManager.CreateAsync(user,DTO.Password);
            if(!result.Succeeded) 
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Admin");
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}
