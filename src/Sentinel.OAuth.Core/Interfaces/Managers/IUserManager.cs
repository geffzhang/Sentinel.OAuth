﻿namespace Sentinel.OAuth.Core.Interfaces.Managers
{
    using System.Threading.Tasks;

    using Sentinel.OAuth.Core.Interfaces.Identity;

    public interface IUserManager
    {
        /// <summary>
        /// Authenticates the user using username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user principal.</returns>
        Task<ISentinelPrincipal> AuthenticateUserWithPasswordAsync(string username, string password);

        /// <summary>
        /// Authenticates the user using username only.
        /// This method is used to get new user claims after a refresh token has been used. You can therefore assume that the user is already logged in.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user principal.</returns>
        Task<ISentinelPrincipal> AuthenticateUserAsync(string username);
    }
}