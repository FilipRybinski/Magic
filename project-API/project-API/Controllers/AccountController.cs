﻿using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_API.Entities;
using project_API.Models;
using project_API.Services;

namespace project_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class accountController : ControllerBase
    {
        private  readonly IAccountService _accountService;
        public accountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] userRegisterDto dto)
        {
            _accountService.RegisterUser(dto);
            return Ok();
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] loginDto dto)
        {
            string token = _accountService.GenerateJwt(dto);
            
            return Ok(token);
        }
        [HttpDelete("delete/{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _accountService.DeleteUser(id);
            return NoContent();
        }
    }
}
