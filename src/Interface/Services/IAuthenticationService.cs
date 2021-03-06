﻿using Minutz.Models.Entities;
using Minutz.Models.Message;

namespace Interface.Services
{
  public interface IAuthenticationService
  {
    (bool condition, string message, AuthRestModel tokenResponse) CreateUser
      (string name, string email, string password, string role, string invitationInstanceId, string meetingId);

     AuthRestModelResponse LoginFromLoginForm 
       (string username, string password, string instanceId);

    AuthRestModelResponse LoginFromFromToken
      (string access_token, string id_token, string expires_in, string instanceId = null);

    AuthRestModel GetUserInfo(string token);

    PersonResponse GetPersonByEmail
      (string email);

    AuthRestModelResponse ResetUserInfo
      (string token);
  }
}
