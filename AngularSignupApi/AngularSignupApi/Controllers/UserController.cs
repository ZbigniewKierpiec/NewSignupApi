﻿using AngularSignupApi.Context;
using AngularSignupApi.Helpers;
using AngularSignupApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;

namespace AngularSignupApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        [HttpPost("authenticate")]

        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Username == userObj.Username);
            if(user == null)    
                return NotFound(new {Message = "User Not Found!"});
            if(!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new {message="Password is Incorrect"});
            }
            return Ok( new {Message = "Login Success!"});
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            // Check username

            if(await CheckUserNameExistAsync(userObj.Username))
                return BadRequest(new {message = "Username Already Exist!"});




            // Check email

                if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { message = "Email Already Exist!" });









            // Check password Strength

            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            userObj.Password= PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
           await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new {Message = "User Registered!"});
        }

        private async Task<bool> CheckUserNameExistAsync(string username)

             => await _authContext.Users.AnyAsync(x => x.Username == username);
        private async Task<bool> CheckEmailExistAsync(string email)

              => await _authContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);

            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,&,*,^,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special chars" + Environment.NewLine);
            return sb.ToString();

        }


    }
}
