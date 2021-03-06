﻿using System.Text;
using Interface.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Notifications
{
  public class StartupService : IStartupService
  {
    private readonly INotify _notify;
    private readonly string _invitationSubject = "You are invited to a Minutz Meeting.";
    public StartupService(INotify notify)
    {
      this._notify = notify;
    }

    public bool SendInvitationMessage(Minutz.Models.Entities.MeetingAttendee attendee, Minutz.Models.ViewModels.MeetingViewModel meeting)
    {
      var to = new EmailAddress(attendee.Email, attendee.Name);
      var result = new SendGridClient(_notify.NotifyKey)
                                      .SendEmailAsync(CreateInvitationMessage(to,
                                                                               _invitationSubject,
                                                                               meeting.Id.ToString(),
                                                                               meeting.Name))
                                      .Result;
      var resultBody = result.Body.ReadAsStringAsync().Result;
      if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Accepted)
      {
        return true;
      }
      return false;
    }

    internal SendGridMessage CreateInvitationMessage(EmailAddress to,
                                                     string subject,
                                                     string meetingId,
                                                     string meetingName)
    {
      var message = MailHelper.CreateSingleEmail(CreateFromUser(),
                                                  to,
                                                  subject,
                                                  createInvitationTextMessage(),
                                                  createInvitationHtmlMessage(to.Name, meetingId, meetingName));
      message.SetTemplateId(_notify.NotifyDefaultTemplateKey);
      return message;
    }

    internal EmailAddress CreateFromUser()
    {
      return new EmailAddress(_notify.NotifyInvitationAddress,
                               _notify.NotifyUser);
    }

    internal string createInvitationHtmlMessage(string attendeeName,
                                                string meetingId,
                                                string meetingName)
    {
      var message = new StringBuilder();
      message.AppendLine($"<div><h2>Welcome {attendeeName},</h2></div>");
      message.AppendLine($"<div><p>You are invited to: {meetingName} .</p></div>");
      message.AppendLine($"<div><p></p></div>");
      message.AppendLine($"<div><p>Click <a href='{_notify.DestinationBaseAddress}?id={meetingId}'>Join</a> to accept the meeting request, and start collaborating. </p></div>");
      message.AppendLine($"<div></div>");
      return message.ToString();
    }
    internal string createInvitationTextMessage()
    {
      return "and easy to do anywhere, even with C#";
    }
  }
}