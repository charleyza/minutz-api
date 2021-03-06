﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Api.Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Minutz.Models.Entities;
using Minutz.Models.Message;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Controllers
{
  public class MeetingAttendeeController : Controller
  {
    private readonly IMeetingService _meetingService;
    private readonly IInstanceService _invationService;
    private readonly IAuthenticationService _authenticationService;

    public MeetingAttendeeController(
      IMeetingService meetingService,
      IInstanceService invatationService,
      IAuthenticationService authenticationService)
    {
      _meetingService = meetingService;
      _invationService = invatationService;
      _authenticationService = authenticationService;
    }

    /// <summary>
    /// Get all a attendees for a meeting
    /// </summary>
    /// <returns>Collection of MeetingAttendee</returns>
     [HttpGet("api/meeting/{referenceId}/attendees")]
     [Authorize]
     public List<MeetingAttendee> Get(string referenceId)
     {
       var userInfo = Request.ExtractAuth(User, _authenticationService);
       return new List<MeetingAttendee>();
     }

    /// <summary>
    /// Get a meeting attendee for a meeting.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
     [HttpGet("api/meeting/{referenceId}/attendee/{id}")]
     [Authorize]
     public MeetingAttendee GetMeetingAttendee(string referenceId, string id)
     {
       var userInfo = Request.ExtractAuth(User, _authenticationService);
       var result = _meetingService.GetAttendee(userInfo.InfoResponse, Guid.Parse(referenceId), Guid.Parse(id));
       return result;
     }

     [HttpPost("api/updateMeetingAttendees")]
     [Authorize]
     public List<MeetingAttendee> UpdateMeetingAttendees(List<MeetingAttendee> attendees)
     {
       var userInfo = Request.ExtractAuth(User, _authenticationService);
       var result = _meetingService.UpdateMeetingAttendees(attendees, userInfo.InfoResponse);
       return result;
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
//    [HttpPut("api/meetingAttendee/invite", Name = "Invite")]
//    [ProducesResponseType(typeof(MeetingAttendee),200)]
//    [SwaggerResponse((int)System.Net.HttpStatusCode.OK, Type = typeof(MeetingAttendee))]
//     [Authorize]
//     public IActionResult Invite([FromBody] MeetingAttendee invitee)
//     {
//       if (string.IsNullOrEmpty(invitee.Email))
//       {
//         return BadRequest("Please provide a valid email address");
//       }
//       if (string.IsNullOrEmpty(invitee.Name))
//       {
//         return BadRequest("Please provide a valid name.");
//       }
//       System.Guid meetingId;
//       if (invitee.ReferenceId == null || !System.Guid.TryParse(invitee.ReferenceId.ToString(), out meetingId))
//       {
//         return BadRequest("Please provide a valid meetingId");
//       }
//       var userInfo = Request.ExtractAuth(User, _authenticationService);
//       var meeting = _meetingService.GetMeeting(userInfo.InfoResponse, invitee.ReferenceId.ToString());
//       //var instance = _meetingService.GetInstance(token);
//       invitee.PersonIdentity = invitee.Email;
//       
//       invitee.Role = "Invited";
//       invitee.Status = "Invited";
//       var result = _invationService.SendMeetingInvatation(invitee, meeting,userInfo.InfoResponse.InstanceId);
//       if (result)
//       {
//         var savedUser = _meetingService.InviteUser(userInfo.InfoResponse, invitee,meeting.Id,invitee.Email);
//         if (savedUser)
//         {
//           return new ObjectResult(invitee);
//         }
//         return BadRequest("There was a issue saving the invited user.");
//       }
//       return BadRequest("There was a issue inviting the user.");
//     }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attendee"></param>
    /// <returns></returns>
     [HttpPost("api/meeting/{ReferenceId}/attendee/{id}")]
     [Authorize]
     public MeetingAttendee Post([FromBody] MeetingAttendee attendee)
     {
       var userInfo = Request.ExtractAuth(User, _authenticationService);
       return attendee;
     }

     [HttpPut("api/meeting/{ReferenceId}/attendee")]
     [Authorize]
     public MeetingAttendee Put([FromBody] MeetingAttendee attendee)
     {
       var userInfo = Request.ExtractAuth(User, _authenticationService);
       return attendee;
     }

     [HttpDelete("api/meeting/{referenceId}/attendee/{id}")]
     [Authorize]
     public bool Delete(string referenceId, string id)
     {
       var userInfo = Request.ExtractAuth(User, _authenticationService);
       return true;
     }
  }
}