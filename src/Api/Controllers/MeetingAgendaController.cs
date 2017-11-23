﻿using System.Collections.Generic;
using System.Linq;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Api.Controllers
{
  //[Route ("api/[controller]")]
  public class MeetingAgendaController : Controller
  {
    private readonly IMeetingService _meetingService;
    public MeetingAgendaController (IMeetingService meetingService)
    {
      _meetingService = meetingService;
    }

    /// <summary>
    /// Get all agenda items for a meeting
    /// </summary>
    /// <returns>Collection of MeetingAgenda objects</returns>
    [HttpGet("api/meeting/{meetingId}/agenda")]
    [Authorize]
    public List<MeetingAgenda> Get ([FromRoute] string meetingId)
    {
      var token = Request.Headers.FirstOrDefault (i => i.Key == "Authorization").Value;
      return new List<MeetingAgenda>();
    }

    /// <summary>
    ///  Returns one instance of a meeting agenda
    /// </summary>
    /// <returns>MeetingAgenda object</returns>
    [HttpGet ("api/meeting/{meetingId}/agenda/{id}")]
    [Authorize]
    public List<MeetingAgenda> Get ([FromRoute] string meetingId, string id)
    {
      var token = Request.Headers.FirstOrDefault (i => i.Key == "Authorization").Value;
      return null;
    }

    /// <summary>
    /// Create a meeting Agenda
    /// </summary>
    /// <returns></returns>
    [HttpPut("api/meeting/{ReferenceId}/agenda")]
    [Authorize]
    public MeetingAgenda Put (MeetingAgenda agenda)
    {
      var token = Request.Headers.FirstOrDefault (i => i.Key == "Authorization").Value;
      return new MeetingAgenda();
    }

    /// <summary>
    /// Update the Meeting Agenda
    /// </summary>
    /// <returns></returns>
    [HttpPost("api/meeting/{ReferenceId}/agenda/{id}")]
    [Authorize]
    public MeetingAgenda Post (MeetingAgenda agenda)
    {
      var token = Request.Headers.FirstOrDefault (i => i.Key == "Authorization").Value;
      return new MeetingAgenda();
    }

    /// <summary>
    /// Delete the single instance of the agenda item.
    /// </summary>
    /// <returns>true if successful and false if failure.</returns>
    [HttpDelete("api/meeting/{meetingId}/agenda/{id}")]
    [Authorize]
    public bool Delete ([FromRoute] string meetingId, string id)
    {
      var token = Request.Headers.FirstOrDefault (i => i.Key == "Authorization").Value;
      return true;
    }
  }
}