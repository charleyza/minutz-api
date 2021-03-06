﻿using Interface.Repositories;
using Interface.Services;
using Minutz.Models.Entities;
using Core.Helper;

namespace Core
{
  public class UserValidationService : IUserValidationService
  {
    private readonly IUserRepository _userRepository;
    private readonly IApplicationSetting _applicationSetting;
    public UserValidationService(IUserRepository userRepository,
                                 IApplicationSetting applicationSetting)
    {
      _userRepository = userRepository;
      _applicationSetting = applicationSetting;
    }

    /// <summary>
    /// checks if the user is new, uses the referenceKey to check if there are any instances where the user is in another company
    /// </summary>
    /// <returns><c>true</c>, if new user was ised, <c>false</c> otherwise.</returns>
    /// <param name="authUserId">Auth user identifier.</param>
    /// <param name="referenceKey">Reference key.</param>
    public bool IsNewUser(string authUserId, string referenceKey)
    {
      (string key, string reference) reference = (string.Empty, string.Empty);
      if (!string.IsNullOrEmpty(referenceKey))
        reference = referenceKey.TupleSplit();

      return _userRepository.CheckIfNewUser(reference,
                                            authUserId,
                                            _applicationSetting.Schema,
        _applicationSetting.CreateConnectionString(_applicationSetting.Server,
                                                   _applicationSetting.Catalogue,
                                                   _applicationSetting.Username,
                                                   _applicationSetting.Password));
    }

    public string CreateAttendee(AuthRestModel authUser, string referenceKey)
    {
      (string key, string reference) reference = (string.Empty, string.Empty);
      if (!string.IsNullOrEmpty(referenceKey))
      {
        reference = referenceKey.TupleSplit();
        switch (reference.key)
        {
          case Core.Helper.AuthenticationHelper.Guest:
            authUser.Role = Core.Helper.AuthenticationHelper.Guest;
            authUser.Related = reference.reference;
            break;
          case Core.Helper.AuthenticationHelper.User:
            authUser.Role = Core.Helper.AuthenticationHelper.User;
            if (reference.reference == Core.Helper.AuthenticationHelper.User)
            {
              authUser.Related = string.Empty;
            }
            else
            {
              if (!string.IsNullOrEmpty(reference.reference))
              {
                authUser.Related = reference.reference;
              }
            }
            break;
          case Core.Helper.AuthenticationHelper.Admin:
            authUser.Role = Core.Helper.AuthenticationHelper.Admin;
            authUser.Related = reference.reference;
            break;
        }
      }
      authUser.Role = "Guest";
      return _userRepository.CreateNewUser(reference, authUser, _applicationSetting.Schema,
        _applicationSetting.CreateConnectionString(_applicationSetting.Server,
                                                   _applicationSetting.Catalogue,
                                                   _applicationSetting.Username,
                                                   _applicationSetting.Password));
    }

    public AuthRestModel GetUser(string authUserId)
    {
      return _userRepository.GetUser(authUserId, _applicationSetting.Schema,
        _applicationSetting.CreateConnectionString(_applicationSetting.Server,
                                                   _applicationSetting.Catalogue,
                                                   _applicationSetting.Username,
                                                   _applicationSetting.Password));
    }
  }
}
