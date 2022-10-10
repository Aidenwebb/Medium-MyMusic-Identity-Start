using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyMusic.Api.Resources;
using MyMusic.Api.Validations;
using MyMusic.Core.Models;
using MyMusic.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyMusic.Core.Models.Auth;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyMusic.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        
        public AuthController(
            IMapper mapper, 
            UserManager<User> userManager,
            RoleManager<Role> roleManager
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpResource userSignUpResource){
            var user = _mapper.Map<UserSignUpResource, User>(userSignUpResource);

            var userCreateResult = await _userManager.CreateAsync(user, userSignUpResource.Password);

            if(!userCreateResult.Succeeded){
                return BadRequest(userCreateResult.Errors);
            }

            return Created(string.Empty, string.Empty);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserLoginResource userLoginResource){
            //var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Username == userLoginResource.Email);

            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userLoginResource.Email);

            if(user == null){
                return BadRequest("Invalid login attempt");
            }

            var userSignInResult = await _userManager.CheckPasswordAsync(user, userLoginResource.Password);

            if(!userSignInResult){
                return BadRequest("Invalid login attempt");
            }

            return Ok("User logged in successfully");
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] SaveRoleResource saveRoleResource)
        {
            var role = _mapper.Map<SaveRoleResource, Role>(saveRoleResource);

            if(string.IsNullOrWhiteSpace(role.Name)){
                return BadRequest("Role name is required");
            }

            var roleCreateResult = await _roleManager.CreateAsync(role);

            if(!roleCreateResult.Succeeded){
                return BadRequest(roleCreateResult.Errors);
            }

            return Created(string.Empty, string.Empty);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleResources = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleResource>>(roles);

            return Ok(roleResources);
        }

        [HttpGet("User/{userEmail}/roles")]
        public async Task<IActionResult> GetUserRoles(string userEmail)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == userEmail);

            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(userRoles);
        }

        [HttpPost("User/{userEmail}/roles")]
        public async Task<IActionResult> AddUserToRole(string userEmail, [FromBody] string roleName)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == userEmail);
            var role = await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName);

            if(role == null){
                return BadRequest("Role does not exist");
            }

            if(user == null){
                return BadRequest("User does not exist");
            }

            var userAddToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);

            if(!userAddToRoleResult.Succeeded){
                return BadRequest(userAddToRoleResult.Errors);
            }

            return Ok("User added to role successfully");
        }
    }
}