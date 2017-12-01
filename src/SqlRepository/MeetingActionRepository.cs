﻿using System.Collections.Generic;
using Interface.Repositories;
using System.Data.SqlClient;
using Models.Entities;
using System.Data;
using System.Linq;
using System;
using Dapper;

namespace SqlRepository
{
  public class MeetingActionRepository : IMeetingActionRepository
  {
    public MinutzAction Get(Guid id, string schema, string connectionString)
    {
      if (id == Guid.NewGuid() || string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(connectionString))
        throw new ArgumentException("Please provide a valid meeting identifier, schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"select * from [{schema}].[MinutzAction] WHERE Id = '{id.ToString()}'";
        var data = dbConnection.Query<MinutzAction>(sql).FirstOrDefault();
        return data;
      }
    }
    public IEnumerable<MinutzAction> List(string schema, string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(schema))
        throw new ArgumentException("Please provide a valid schema or connection string.");
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        var sql = $"select * from [{schema}].[MinutzAction]";
        var data = dbConnection.Query<MinutzAction>(sql).ToList();
        return data;
      }
    }
    public bool Add(MinutzAction action,string schema, string connectionString)
    {
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        string insertSql = $@"insert into [{schema}].[MinutzAction](
                                                                 [Id]
                                                                ,[ReferanceId]
                                                                ,[ActionText]
                                                                ,[PersonId]
                                                                ,[DueDate]
                                                                ,[IsComplete]
                                                                ) 
                                                         values(
                                                                 @Id
                                                                ,@ReferanceId
                                                                ,@ActionText
                                                                ,@PersonId
                                                                ,@DueDate
                                                                ,@IsComplete)";
        var instance = dbConnection.Execute(insertSql, new
                                                        {
                                                          action.Id,
                                                          action.ReferanceId,
                                                          action.ActionText,
                                                          action.PersonId,
                                                          action.DueDate,
                                                          action.IsComplete
                                                        });
        return instance == 1;
      }
    }
    public bool Update(MinutzAction action, string schema, string connectionString)
    {
      using (IDbConnection dbConnection = new SqlConnection(connectionString))
      {
        dbConnection.Open();
        string updateQuery = $@"UPDATE [{schema}].[MinutzAction]
                             SET ActionText = @ActionText, 
                                 PersonId = @PersonId, 
                                 DueDate = @DueDate, 
                                 IsComplete = @IsComplete
                             WHERE Id = @Id";
        var instance = dbConnection.Execute(updateQuery, new
        {
          Action = action.ActionText,
          PersonId = action.PersonId.ToString(),
          action.DueDate,
          action.IsComplete
        });
        return instance == 1;
      }
    }

    public bool Delete(MinutzAction action, string schema, string connectionString)
    {
      return true;
    }
  }
}
