using Interface.Repositories;
using SqlRepository;
using System;
using Xunit;

namespace TestIntegrations
{
  public class DatabaseCreateTests
  {
    private readonly IApplicationSetupRepository _applicationSetupRepository;
    private readonly string _connectionString;
    private readonly string _falseConnectionString;
    public DatabaseCreateTests()
    {
      _applicationSetupRepository = new ApplicationSetupRepository();
      _connectionString = "Server=tcp:127.0.0.1,1433;User ID=sa;pwd=password1234$;database=minutz;";
      _falseConnectionString = "Server=tcp:127.0.0.1,1433;User ID=sa;pwd=password234$;database=minutz;";
    }

    [Fact]
    public void InstallDatabase_CheckConnectivity_GivenNoConnectionString_ShouldThrowException()
    {
      Exception ex = Assert.Throws<ArgumentNullException>(() => _applicationSetupRepository.Exists(null));
    }

    [Fact]
    public void InstallDatabase_CheckConnectivity_GivenEmptyConnectionString_ShouldThrowException()
    {
      Exception ex = Assert.Throws<ArgumentNullException>(() => _applicationSetupRepository.Exists(string.Empty));
    }

    [Fact]
    public void InstallDatabase_CheckConnectivity_GivenConnectionStringWithFalseDetails_ShouldThrowException()
    {
      var result = _applicationSetupRepository.Exists(_falseConnectionString);
      Assert.False(result);
    }

    [Fact]
    public void InstallDatabase_CheckConnectivity_GivenConnectionStringWithValidDetails_ShouldThrowException()
    {
      var result = _applicationSetupRepository.Exists(_connectionString);
      Assert.True(result);
    }
  }
}
