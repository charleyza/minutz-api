﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using tzatziki.minutz.interfaces;
using tzatziki.minutz.models.Auth;
using tzatziki.minutz.models.Entities;

namespace tzatziki.minutz.sqlrepository
{
  public class MeetingRepository : IMeetingRepository
  {
    private const string _createMeetingSchemaStoredProcedure = "createMeetingSchema";
    private const string _meetingTableName = "Meeting";
    private const string _meetingAgendaTableName = "MeetingAgenda";
    private const string _meetingOwnerIdProperty = "MeetingOwnerId";
    private const string _meetingAttendee = "MeetingAttendee";
    private const string _meetingAction = "MeetingAction";
    private const string _meetingNote = "MeetingNote";
    private const string _meetingAttachment = "MeetingAttachment";

    private readonly ITableService _tableService;

    public MeetingRepository(ITableService tableService)
    {
      _tableService = tableService;
    }

    public IEnumerable<Meeting> Get(string connectionString, string schema, UserProfile user)
    {
      var result = new List<Meeting>();
      if (_tableService.Initiate(connectionString, schema, _meetingTableName, _createMeetingSchemaStoredProcedure))
      {
        foreach (Meeting meeting in GetUserMeetings(connectionString, schema, user))
        {
          result.Add(ToMeeting(connectionString, schema, meeting));
        }
      }
      return result;
    }

    public Meeting Get(string connectionString, string schema, Meeting meeting, bool read = false)
    {
      if (_tableService.Initiate(connectionString, schema, _meetingTableName, _createMeetingSchemaStoredProcedure))
      {
        var instance = ToMeeting(connectionString, schema, $" Id = '{meeting.Id}'");
        if (instance == null)
        {
          instance = ToMeeting(connectionString, schema, meeting, false);
        }
        else
        {
          if (read)
          {
            //get the agenda items
            var agendCollection = ToMeetingAgenda(connectionString, schema, $" Id = '{meeting.Id}'");

            return instance;
          }
          else
          {
            instance = ToMeeting(connectionString, schema, meeting, true);
          }
        }
        return instance;
      }
      throw new Exception($"Error retrieving the meeting instance for: {schema}, {meeting.Id}");
    }

    internal IEnumerable<Meeting> GetUserMeetings(string connectionString, string schema, UserProfile user)
    {
      return ToList(schema, connectionString, $" {_meetingOwnerIdProperty} = '{user.UserId.ToString()}'");
    }

    internal List<Meeting> ToList(string schema, string connectionString)
    {
      var result = new List<Meeting>();
      if (_tableService.Initiate(connectionString, schema, _meetingTableName, _createMeetingSchemaStoredProcedure))
      {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
          using (SqlCommand command = new SqlCommand(SelectMeetingStatement(schema), con))
          {
            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                result.Add(ToMeeting(reader));
              }
            }
          }
        }
      }
      return result;
    }

    internal List<Meeting> ToList(string schema, string connectionString, string filter)
    {
      var result = new List<Meeting>();
      if (_tableService.Initiate(connectionString, schema, _meetingTableName, _createMeetingSchemaStoredProcedure))
      {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
          con.Open();
          using (SqlCommand command = new SqlCommand(SelectMeetingStatement(schema, filter), con))
          {
            using (SqlDataReader reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                result.Add(ToMeeting(reader));
              }
            }
          }
          con.Close();
        }
      }
      return result;
    }

    internal Meeting ToMeeting(string connectionString, string schema, string filter)
    {
      Meeting result;
      using (SqlConnection con = new SqlConnection(connectionString))
      {
        con.Open();

        using (SqlCommand command = new SqlCommand(SelectMeetingStatement(schema, filter), con))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              return result = ToMeeting(reader);
            }
          }
        }
        con.Close();
      }
      return null;
    }

    internal IEnumerable<MeetingAgendaItem> ToMeetingAgenda(string connectionString, string schema, string filter)
    {
      var result = new List<MeetingAgendaItem>();
      using (SqlConnection con = new SqlConnection(connectionString))
      {
        con.Open();

        using (SqlCommand command = new SqlCommand(SelectMeetingAgendaStatement(schema, filter), con))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              result.Add(ToMeetingAgenda(reader));
            }
          }
        }
        con.Close();
      }
      return result;
    }

    internal Meeting ToMeeting(string connectionString, string schema, Meeting meeting, bool update = false)
    {
      if (_tableService.Initiate(connectionString, schema, _meetingTableName, _createMeetingSchemaStoredProcedure))
      {
        if (update)
        {
          using (SqlConnection con = new SqlConnection(connectionString))
          {
            con.Open();
            using (SqlCommand command = new SqlCommand(UpdateMeetingStatement(schema, meeting), con))
            {
              try
              {
                command.ExecuteNonQuery();
              }
              catch (Exception ex)
              {
                throw new Exception($"Issue updating the meeting record. {ex.Message}", ex.InnerException);
              }
            }
            con.Close();
          }
        }
        else
        {
          using (SqlConnection con = new SqlConnection(connectionString))
          {
            con.Open();
            using (SqlCommand command = new SqlCommand(InsertMeetingStatement(schema, meeting), con))
            {
              try
              {
                command.ExecuteNonQuery();

              }
              catch (Exception ex)
              {
                throw new Exception($"Issue insert the meeting record. {ex.Message}", ex.InnerException);
              }
            }
            con.Close();
          }
        }
      }
      return ToMeeting(connectionString, schema, $" Id = '{meeting.Id}'");
    }

    internal string SelectMeetingStatement(string schema)
    {
      return $"SELECT * FROM [{schema}].[Meeting] ";
    }

    ///SELECT STATEMENTS

    internal string SelectMeetingStatement(string schema, String filter)
    {
      return $"SELECT * FROM [{schema}].[{_meetingTableName}] WHERE {filter}";
    }

    internal string SelectMeetingAgendaStatement(string schema, String filter)
    {
      return $"SELECT * FROM [{schema}].[{_meetingAgendaTableName}] WHERE {filter}";
    }

    internal string SelectMeetingAttendeeStatement(string schema, String filter)
    {
      return $"SELECT * FROM [{schema}].[{_meetingAttendee}] WHERE {filter}";
    }

    internal string SelectMeetingNoteStatement(string schema, String filter)
    {
      return $"SELECT * FROM [{schema}].[{_meetingNote}] WHERE {filter}";
    }

    ///UPDATE STATEMENTS

    internal string UpdateMeetingStatement(string schema, Meeting meeting)
    {
      var isFormal = meeting.IsFormal == true ? 1 : 0;
      var isReacurance = meeting.IsReacurance == true ? 1 : 0;
      var isPrivate = meeting.IsPrivate == true ? 1 : 0;
      var isLocked = meeting.IsLocked == true ? 1 : 0;
      return $@"UPDATE [{schema}].[Meeting]
        SET [Name] = '{meeting.Name.EmptyIfNull()}',
        [Location] ='{meeting.Location.EmptyIfNull()}',
        [Date] = '{meeting.Date}',
        [Time] = '{meeting.Time.EmptyIfNull()}',
        [Duration] = '{meeting.Duration.EmptyIfNull()}',
        [IsReacurance] = {isReacurance},
        [IsPrivate] = {isPrivate},
        [ReacuranceType] = '{meeting.ReacuranceType}',
        [IsLocked] = {isLocked},
        [IsFormal] = {isFormal},
        [TimeZone] = '{meeting.TimeZone.EmptyIfNull()}',
        [Tag] = '{string.Join(",", meeting.Tag)}',
        [Purpose] = '{meeting.Purpose.EmptyIfNull()}',
        [Outcome] = '{meeting.Outcome.EmptyIfNull()}'
        WHERE [Id] = '{meeting.Id}'";
    }

    internal string UpdateMeetingAgendaItemStatement(string schema, MeetingAgendaItem meetingAgendaItem)
    {
      var isComplete = meetingAgendaItem.IsComplete == true ? 1 : 0;
      return $@"UPDATE [{schema}].[_meetingAgendaTableName] SET
        [ReferanceId] = '{meetingAgendaItem.ReferanceId}',
        [AgendaHeading] = '{meetingAgendaItem.AgendaHeading.EmptyIfNull()}',
        [AgendaText] = '{meetingAgendaItem.AgendaText.EmptyIfNull()}',
        [MeetingAttendeeId] = '{meetingAgendaItem.MeetingAttendeeId}',
        [Duration] = '{meetingAgendaItem.Duration.EmptyIfNull()}',
        [CreatedDate] = '{meetingAgendaItem.CreatedDate}',
        [IsComplete] = {isComplete}
        WHERE [Id] = '{meetingAgendaItem.Id}'";
    }


    ///INSERT STATEMENTS

    internal string InsertMeetingStatement(string schema, Meeting meeting)
    {
      var isFormal = meeting.IsFormal == true ? 1 : 0;
      var isReacurance = meeting.IsReacurance == true ? 1 : 0;
      var isPrivate = meeting.IsPrivate == true ? 1 : 0;
      var isLocked = meeting.IsLocked == true ? 1 : 0;
      var meetingDate = meeting.Date == DateTime.MinValue ? DateTime.UtcNow.ToString() : meeting.Date.ToUniversalTime().ToString();

      return $@"INSERT INTO [{schema}].[{_meetingTableName}] VALUES (
        '{meeting.Id}',
        '{meeting.Name.EmptyIfNull()}',
        '{meeting.Location.EmptyIfNull()}',
        '{meetingDate}',
        '{DateTime.UtcNow.ToString()}',
        '{meeting.Time.EmptyIfNull()}',
        '{meeting.Duration.EmptyIfNull()}',
        {isReacurance},
        {isPrivate},
        '{meeting.ReacuranceType}',
        {isLocked},
        {isFormal},
        '{meeting.TimeZone.EmptyIfNull()}',
        '{string.Join(",", meeting.Tag)}',
        '{meeting.Purpose.EmptyIfNull()}',
        '{meeting.MeetingOwnerId}',
        '{meeting.Outcome.EmptyIfNull()}'
        )";
    }

    internal string InsertMeetingAgendaItemStatement(string schema, MeetingAgendaItem meetingAgendaItem)
    {
      var isComplete = meetingAgendaItem.IsComplete == true ? 1 : 0;

      return $@"INSERT INTO [{schema}].[{_meetingAgendaTableName}] VALUES (
        '{meetingAgendaItem.Id}',
        '{meetingAgendaItem.ReferanceId}',
        '{meetingAgendaItem.AgendaHeading.EmptyIfNull()}',
        '{meetingAgendaItem.AgendaText.EmptyIfNull()}',
        '{meetingAgendaItem.MeetingAttendeeId}',
        '{meetingAgendaItem.Duration.EmptyIfNull()}',
        '{meetingAgendaItem.CreatedDate}',
        {isComplete}
        )";
    }

    internal string InsertMeetingAttendeeStatement(string schema, MeetingAttendee meetingAttendee)
    {
      return $@"INSERT INTO [{schema}].[{_meetingAttendee}] VALUES (
        '{meetingAttendee.Id}',
        '{meetingAttendee.ReferanceId}',
        '{meetingAttendee.PersonIdentity.EmptyIfNull()}',
        '{meetingAttendee.Role.EmptyIfNull()}'
        )";
    }

    internal string InsertMeetingNoteStatement(string schema, MeetingNoteItem meetingNote)
    {
      return $@"INSERT INTO [{schema}].[{_meetingNote}] VALUES (
        '{meetingNote.Id}',
        '{meetingNote.ReferanceId}',
        '{meetingNote.NoteText.EmptyIfNull()}',
        '{meetingNote.MeetingAttendeeId}',
        '{meetingNote.CreatedDate.ToString()}'
        )";
    }

    internal string InsertMeetingAttachmentStatement(string schema, MeetingAttachmentItem meetingAttachmentItem)
    {
      return $@"INSERT INTO [{schema}].[{_meetingAttachment}] VALUES (
        '{meetingAttachmentItem.Id}',
        '{meetingAttachmentItem.ReferanceId}',
        '{meetingAttachmentItem.FileName.EmptyIfNull()}',
        '{meetingAttachmentItem.MeetingAttendeeId}',
        '{meetingAttachmentItem.Date.ToString()}'
        )";
    }

    ///DATA READER

    internal Meeting ToMeeting(SqlDataReader dataReader)
    {
      return new Meeting
      {
        Id = Guid.Parse(dataReader["Id"].ToString()),
        Name = dataReader["Name"].ToString(),
        Location = dataReader["Location"].ToString(),
        Date = DateTime.Parse(dataReader["Date"].ToString()),
        UpdatedDate = DateTime.Parse(dataReader["UpdatedDate"].ToString()),
        Time = dataReader["Time"].ToString(),
        Duration = Int32.Parse(dataReader["Duration"].ToString()),
        IsReacurance = bool.Parse(dataReader["IsReacurance"].ToString()),
        IsPrivate = bool.Parse(dataReader["IsPrivate"].ToString()),
        IsLocked = bool.Parse(dataReader["IsLocked"].ToString()),
        IsFormal = bool.Parse(dataReader["IsFormal"].ToString()),
        ReacuranceType = dataReader["ReacuranceType"].ToString(),
        Tag = dataReader["Tag"].ToString().Split(','),
        Purpose = dataReader["Purpose"].ToString(),
        Outcome = dataReader["Outcome"].ToString(),
        MeetingOwnerId = dataReader["MeetingOwnerId"].ToString(),
        TimeZone = dataReader["TimeZone"].ToString()
      };
    }

    internal MeetingAgendaItem ToMeetingAgenda(SqlDataReader dataReader)
    {
      return new MeetingAgendaItem
      {
        AgendaHeading = dataReader["AgendaHeading"].ToString(),
        AgendaText = dataReader["AgendaText"].ToString(),
        CreatedDate = DateTime.Parse(dataReader["CreatedDate"].ToString()),
        Duration = dataReader["Duration"].ToString(),
        Id = Guid.Parse(dataReader["Id"].ToString()),
        IsComplete = bool.Parse(dataReader["IsComplete"].ToString()),
        MeetingAttendeeId = dataReader["MeetingAttendeeId"].ToString(),
        ReferanceId = Guid.Parse(dataReader["ReferanceId"].ToString())
      };
    }

    internal MeetingAttendee ToMeetingAttendee(SqlDataReader dataReader)
    {
      return new MeetingAttendee
      {
        Id = Guid.Parse(dataReader["Id"].ToString()),
        Role = dataReader["Role"].ToString(),
        PersonIdentity = dataReader["PersonIdentity"].ToString(),
        ReferanceId = Guid.Parse(dataReader["ReferanceId"].ToString())
      };
    }

    internal MeetingAttachmentItem ToMeetingAttachment(SqlDataReader dataReader)
    {
      return new MeetingAttachmentItem
      {
        Id = Guid.Parse(dataReader["Id"].ToString()),
        Date = DateTime.Parse(dataReader["Date"].ToString()),
        FileName = dataReader["FileName"].ToString(),
        MeetingAttendeeId = Guid.Parse(dataReader["ReferanceId"].ToString()),
        ReferanceId = Guid.Parse(dataReader["ReferanceId"].ToString()),
        //FileData = byte[]
      };
    }

    internal MeetingNoteItem ToMeetingNote(SqlDataReader dataReader)
    {
      return new MeetingNoteItem
      {
        Id = Guid.Parse(dataReader["Id"].ToString()),
        CreatedDate = DateTime.Parse(dataReader["CreatedDate"].ToString()),
        NoteText = dataReader["NoteText"].ToString(),
        MeetingAttendeeId = Guid.Parse(dataReader["ReferanceId"].ToString()),
        ReferanceId = Guid.Parse(dataReader["ReferanceId"].ToString()),
      };
    }


  }
}

public static class Extensions
{
  public static string EmptyIfNull(this object value)
  {
    return value?.ToString() ?? string.Empty;
  }
}