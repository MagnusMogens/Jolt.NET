using Jolt.NET.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Jolt.NET.Core.Tests
{
    [TestFixture]
    public class SettingsTests
    {
        [Test] public void 
        SetCurrentUser_WhenCalled_SetsCurrentUser()
        {
            var user = new Data.User
            {
                Username = "Testuser",
                Token = "Testtoken"
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            Settings.Instance.SetCurrentUser("Testuser");

            Settings.Instance.CurrentUser.ShouldBeEquivalentTo(user);
        }

        [Test] public void 
        SetCurrentUser_EmptyUserName_ShoutldThrow()
        {
            var ex = Assert.Catch<UserNotAuthenticatedException>(
                () => Settings.Instance.SetCurrentUser(""));

            ex.Message.ShouldBeEquivalentTo("The username cannot be empty.");            
        }

        [Test] public void
        SetCurrentUser_InvalidUsername_ShouldThrow()
        {
            var ex = Assert.Catch<UserNotAuthenticatedException>(
                () => Settings.Instance.SetCurrentUser("TestuserFailNotAuthenticated"));

            ex.Message.ShouldBeEquivalentTo("The user cannot be found.");
        }

        [Test] public void 
        SetCurrentUser_UserWithoutToken_ShouldThrow()
        {
            var user = new Data.User
            {
                Username = "TestuserFailEmptyToken",
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            var ex = Assert.Catch<UserNotAuthenticatedException>(
                () => Settings.Instance.SetCurrentUser(user.Username));

            ex.Message.ShouldBeEquivalentTo("The user must be authenticated to use the API.");
        }

        [Test] public void 
        GetAuthenticatedUser_ValidUserName_ReturnUser()
        {
            var user = new Data.User
            {
                Username = "GetTestuser",
                Token = "Testtoken"
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            var result = Settings.Instance.GetAuthenticatedUser(user.Username);

            result.Should().Be(user);
        }

        [Test] public void 
        GetAuthenticatedUser_EmptyInput_ReturnCurrentUser()
        {
            var user = new Data.User
            {
                Username = "GetCurrentTestuser",
                Token = "Testtoken"
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            Settings.Instance.CurrentUser = user;
            var result = Settings.Instance.GetAuthenticatedUser();

            result.Should().Be(user);
        }

        public void
        IsAuthenticated_ValidAuthenticatedUser_ReturnTrue()
        {
            var user = new Data.User
            {
                Username = "ValidIsAuthenticatedUser",
                IsAuthenticated = true
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            var result = Settings.Instance.IsAuthenticated(user);

            result.Should().Be(true);
        }
        
        public void
        IsAuthenticated_ValidNotAuthenticatedUser_ReturnFalse()
        {
            var user = new Data.User
            {
                Username = "ValidIsNotAuthenticatedUser",
                IsAuthenticated = false
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            var result = Settings.Instance.IsAuthenticated(user);

            result.Should().Be(false);
        }

        [Test] public void 
        IsAuthenticated_EmptyInput_ReturnCurrentUserTrue()
        {
            var user = new Data.User
            {
                Username = "CurrentAuthenticatedUser",
                IsAuthenticated = true
            };
            Settings.Instance.AuthenticatedUsers.Add(user);
            Settings.Instance.CurrentUser = user;
            var result = Settings.Instance.IsAuthenticated();

            result.Should().BeTrue();
        }
    }
}