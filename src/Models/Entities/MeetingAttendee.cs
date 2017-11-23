﻿using System;

namespace Models.Entities
{
  public class MeetingAttendee
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ReferenceId { get; set; }
    public string Email {get;set;}
    public  string PersonIdentity { get; set; }
    public string Status {get;set;}
    public  string Role { get; set; }
  }
}