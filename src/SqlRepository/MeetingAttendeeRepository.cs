﻿using System.Collections.Generic;
using System.Data.SqlClient;
using Minutz.Models.Entities;
using System.Linq;
using System.Data;
using Dapper;
using System;
using Interface.Repositories;

namespace SqlRepository
{
  public class MeetingAttendeeRepository : IMeetingAttendeeRepository
  {
    /// <summary>
    /// Get the specified id, schema and connectionString.
    /// </summary>
    /// <returns>The get.</returns>
    /// <param name="id">Identifier.</param>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public MeetingAttendee Get(Guid id, string schema, string connectionString)
    {
      if (id == Guid.NewGuid() || string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(connectionString))
        throw new ArgumentException("Please provide a valid meeting attendee identifier, schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"select * from [{schema}].[MeetingAttendee] WHERE Id = '{id.ToString()}'";
        var data = dbConnection.Query<MeetingAttendee>(sql).FirstOrDefault();
        return data;
      }
    }

    /// <summary>
    /// Gets the meeting attendees.
    /// </summary>
    /// <returns>The meeting attendees.</returns>
    /// <param name="referenceId">Reference identifier.</param>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public List<MeetingAttendee> GetMeetingAttendees(Guid referenceId, string schema, string connectionString)
    {
      if (referenceId == Guid.NewGuid() || string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(connectionString))
        throw new ArgumentException("Please provide a valid meeting attendee identifier, schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"select * from [{schema}].[MeetingAttendee] WHERE ReferanceId = '{referenceId.ToString()}'";
        var data = dbConnection.Query<MeetingAttendee>(sql);
        return data.ToList();
      }
    }

    /// <summary>
    /// Gets the avalible attendees.
    /// </summary>
    /// <returns>The avalible attendees.</returns>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public List<MeetingAttendee> GetAvalibleAttendees(string schema, string connectionString)
    {
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"select * from [{schema}].[AvailibleAttendee]";
        var data = dbConnection.Query<MeetingAttendee>(sql);
        return data.ToList();
      }
    }

    /// <summary>
    /// List the specified schema and connectionString.
    /// </summary>
    /// <returns>The list.</returns>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public IEnumerable<MeetingAttendee> List(string schema, string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(schema))
        throw new ArgumentException("Please provide a valid schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"select * from [{schema}].[MeetingAttendee]";
        var data = dbConnection.Query<MeetingAttendee>(sql).ToList();
        return data;
      }
    }

    /// <summary>
    /// Add the specified action, schema and connectionString.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="attendee">Action.</param>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public bool Add(MeetingAttendee attendee, string schema, string connectionString)
    {
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        if (string.IsNullOrEmpty(attendee.Id))
        {
          attendee.Id = Guid.NewGuid().ToString();
        }
        dbConnection.Open();
        string insertSql = $@"insert into [{schema}].[MeetingAttendee](
                                                                 [Id]
                                                                ,[ReferanceId]
                                                                ,[PersonIdentity]
                                                                ,[Role]
                                                                ) 
                                                         values(
                                                                 @Id
                                                                ,@ReferenceId
                                                                ,@PersonIdentity
                                                                ,@Role
                                                                )";
        var instance = dbConnection.Execute(insertSql, new
        {
          attendee.Id,
          attendee.ReferenceId,
          attendee.PersonIdentity,
          attendee.Role
        });
        return instance == 1;
      }
    }

    public bool AddInvitee(MeetingAttendee attendee, string schema, string connectionString)
    {
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        if (string.IsNullOrEmpty(attendee.Id))
        {
          attendee.Id = Guid.NewGuid().ToString();
        }
        dbConnection.Open();
        string insertSql = $@"insert into [{schema}].[AvailibleAttendee](
                                                                 [Id]
                                                                ,[ReferanceId]
                                                                ,[PersonIdentity]
                                                                ,[Status]
                                                                ,[Role]
                                                                ) 
                                                         values(
                                                                 @Id
                                                                ,@ReferenceId
                                                                ,@PersonIdentity
                                                                ,@Status
                                                                ,@Role
                                                                )";
        var instance = dbConnection.Execute(insertSql, new
        {
          attendee.Id,
          attendee.ReferenceId,
          attendee.PersonIdentity,
          attendee.Status,
          attendee.Role
        });
        return instance == 1;
      }
    }

    /// <summary>
    /// Update the specified action, schema and connectionString.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="action">Action.</param>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public bool Update(MeetingAttendee action, string schema, string connectionString)
    {
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        string updateQuery = $@"UPDATE [{schema}].[MeetingAttendee] 
                             SET ReferanceId = @ReferanceId, 
                                 PersonIdentity = @PersonIdentity, 
                                 Role = @Role
                             WHERE Id = @Id";
        var instance = dbConnection.Execute(updateQuery, new
        {
          action.ReferenceId,
          action.PersonIdentity,
          action.Role
        });
        return instance == 1;
      }
    }

    /// <summary>
    /// Deletes the meeting attendees.
    /// </summary>
    /// <returns><c>true</c>, if meeting attendees was deleted, <c>false</c> otherwise.</returns>
    /// <param name="referanceId">Referance identifier.</param>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public bool DeleteMeetingAttendees(Guid referanceId, string schema, string connectionString)
    {
      if (referanceId == Guid.NewGuid() || string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(connectionString))
        throw new ArgumentException("Please provide a valid meeting attendee identifier, schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"delete from [{schema}].[MeetingAttendee] WHERE ReferanceId = '{referanceId.ToString()}'";
        var instance = dbConnection.Execute(sql);
        return instance == 1;
      }
    }

    /// <summary>
    /// Delete the specified id, schema and connectionString.
    /// </summary>
    /// <returns>The delete.</returns>
    /// <param name="id">Identifier.</param>
    /// <param name="schema">Schema.</param>
    /// <param name="connectionString">Connection string.</param>
    public bool Delete(Guid id, string schema, string connectionString)
    {
      if (id == Guid.NewGuid() || string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(connectionString))
        throw new ArgumentException("Please provide a valid meeting attendee identifier, schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"delete from [{schema}].[MeetingAttendee] WHERE Id = '{id.ToString()}'";
        var instance = dbConnection.Execute(sql);
        return instance == 1;
      }
    }
  }
}
