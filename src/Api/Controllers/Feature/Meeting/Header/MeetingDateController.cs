﻿using System;
using Api.Extensions;
using Interface.Services.Feature.Meeting.Header;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class MeetingDateController : Controller
    {
        private readonly IMinutzDateService _minutzDateService;

        public MeetingDateController(IMinutzDateService minutzDateService)
        {
            _minutzDateService = minutzDateService;
        }

        [Authorize]
        [HttpPost("api/feature/header/date", Name = "Update Meeting date")]
        public IActionResult UpdateMeetingDateResult(string id, DateTime date)
        {
            var userInfo = User.ToRest();
            var result = _minutzDateService.Update(id, date, userInfo);
            if (result.Condition)
            {
                return Ok();
            }

            return StatusCode(result.Code, result.Message);
        }
    }
}