﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Minutz.Models.Entities;
//using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Controllers
{
  public class MeetingAttendeeController : Controller
  {
    private readonly IMeetingService _meetingService;
    public MeetingAttendeeController(IMeetingService meetingService)
    {
      _meetingService = meetingService;
    }

    /// <summary>
    /// Get all a attendees for a meeting
    /// </summary>
    /// <returns>Collection of MeetingAttendee</returns>
    [HttpGet("api/meeting/{referenceId}/attendees")]
    [Authorize]
    public List<MeetingAttendee> Get(string referenceId)
    {
      var token = Request.Headers.FirstOrDefault(i => i.Key == "Authorization").Value;
      return new List<MeetingAttendee>();
    }

    /// <summary>
    /// Get a meeting attendee for a meeting.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("api/meeting/{referenceId}/attendee/{id}")]
    [Authorize]
    public MeetingAttendee Get(string referenceId, string id)
    {
      var token = Request.Headers.FirstOrDefault(i => i.Key == "Authorization").Value;
      return new MeetingAttendee();
    }

    /// <summary>
    /// Invite Attendee.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /Invite
    ///     {
    ///        "id": 1,
    ///        "name": "Item1",
    ///        "isComplete": true
    ///     }
    ///
    /// </remarks>
    /// <param name="item"></param>
    /// <returns>A newly-created TodoItem</returns>
    /// <response code="201">Returns the newly-created item</response>
    /// <response code="400">If the item is null</response>
    [HttpPut("api/meeting/{referenceId}/invite", Name = "Invite")]
    //[ProducesResponseType(typeof(MeetingAttendee),200)]
    //[SwaggerResponse((int)System.Net.HttpStatusCode.OK, Type = typeof(MeetingAttendee))]
    [Authorize]
    public IActionResult Invite([FromBody] MeetingAttendee invitee)
    {
      if (string.IsNullOrEmpty(invitee.Email))
      {
        return BadRequest("Please provide a valid email address");
      }
      if (string.IsNullOrEmpty(invitee.Name))
      {
        return BadRequest("Please provide a valid name.");
      }
      System.Guid meetingId;
      if (invitee.ReferenceId == null || System.Guid.TryParse(invitee.ReferenceId.ToString(), out meetingId))
      {
        return BadRequest("Please provide a valid meetingId");
      }
      var token = Request.Headers.FirstOrDefault(i => i.Key == "Authorization").Value;
      return new ObjectResult(new MeetingAttendee());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attendee"></param>
    /// <returns></returns>
    [HttpPost("api/meeting/{ReferenceId}/attendee/{id}")]
    [Authorize]
    public MeetingAttendee Post([FromBody] MeetingAttendee attendee)
    {
      var token = Request.Headers.FirstOrDefault(i => i.Key == "Authorization").Value;
      return attendee;
    }

    [HttpPut("api/meeting/{ReferenceId}/attendee")]
    [Authorize]
    public MeetingAttendee Put([FromBody] MeetingAttendee attendee)
    {
      var token = Request.Headers.FirstOrDefault(i => i.Key == "Authorization").Value;
      return attendee;
    }

    [HttpDelete("api/meeting/{referenceId}/attendee/{id}")]
    [Authorize]
    public bool Delete(string referenceId, string id)
    {
      var token = Request.Headers.FirstOrDefault(i => i.Key == "Authorization").Value;
      return true;
    }
  }
}