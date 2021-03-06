﻿using System;
using Interface.Repositories;
using Interface.Services;
using Minutz.Models.Entities;

namespace Core
{
  public class ApplicationManagerService : IApplicationManagerService
  {
    private readonly IApplicationSetupRepository _applicationSetupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationSetting _applicationSetting;
    public ApplicationManagerService(IApplicationSetupRepository applicationSetupRepository,
                                     IUserRepository userRepository,
                                     IApplicationSetting applicationSetting)
    {
      _applicationSetupRepository = applicationSetupRepository;
      _userRepository = userRepository;
      _applicationSetting = applicationSetting;
    }

    public (bool condition, string message) ResetAcccount(AuthRestModel user)
    {
      var connectionString = _applicationSetting.CreateConnectionString(
                                                           _applicationSetting.Server,
                                                           _applicationSetting.Catalogue,
                                                           _applicationSetting.Username,
                                                           _applicationSetting.Password);
      return this._userRepository.Reset(connectionString, user.InstanceId, user.Name);

    }

    public (bool condition, string message) StartFullVersion(AuthRestModel user)
    {
      user.Role = "User";
      var masterConnectionString = _applicationSetting.CreateConnectionString(
                                                           _applicationSetting.Server,
                                                           "master",
                                                           _applicationSetting.Username,
                                                           _applicationSetting.Password);
      var userConnectionString = _applicationSetting.CreateConnectionString(
                                                           _applicationSetting.Server,
                                                           _applicationSetting.Catalogue,
                                                           _applicationSetting.Username,
                                                           _applicationSetting.Password);
      try
      {
        var schemaCreate = _userRepository.CreateNewSchema(
                                                    user,
                                                    userConnectionString,
                                                    masterConnectionString);

        _applicationSetupRepository.CreateSchemaTables(_applicationSetting.Schema, schemaCreate,
                                      _applicationSetting.CreateConnectionString(
                                                                    _applicationSetting.Server,
                                                                    _applicationSetting.Catalogue,
                                                                    _applicationSetting.Username,
                                                                    _applicationSetting.Password));
      }
      catch (Exception ex)
      {
        return (false, ex.Message);
      }

      return (true, "successfull");
    }
  }
}
