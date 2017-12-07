﻿using System;

namespace Minutz.Models.Entities
{
  public class MeetingAttendee
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string ReferenceId { get; set; }
    public string Email { get; set; }
    public string PersonIdentity { get; set; }
    public string Status { get; set; }
    public string Role { get; set; }
  }
}