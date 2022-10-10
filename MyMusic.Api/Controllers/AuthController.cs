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

namespace MyMusic.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        
        public AuthController(IMapper mapper, UserManager<User> userManager){
            _mapper = mapper;
            _userManager = userManager;
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



    }
}