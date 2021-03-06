﻿namespace Sentinel.OAuth.Extensions
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using Sentinel.OAuth.Core.Interfaces.Identity;
    using Sentinel.OAuth.Core.Models.OAuth.Http;

    public static class SentinelIdentityExtensions
    {
        /// <summary>
        ///     An ISentinelIdentity extension method that converts an identity to the claims
        ///     identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>A ClaimsIdentity.</returns>
        public static ClaimsIdentity AsClaimsIdentity(this ISentinelIdentity identity)
        {
            return new ClaimsIdentity(identity.Claims.ToClaims(), identity.AuthenticationType);
        }

        /// <summary>
        /// An ISentinelIdentity extension method that converts an identity to an identity response.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>An IdentityResponse.</returns>
        public static IdentityResponse AsIdentityResponse(this ISentinelIdentity identity)
        {
            var claims = new List<KeyValuePair<string, string>>();

            foreach (var claim in identity.Claims)
            {
                if (!string.IsNullOrEmpty(claim.Alias))
                {
                    claims.Add(new KeyValuePair<string, string>(claim.Alias, claim.Value));
                }
                else
                {
                    claims.Add(new KeyValuePair<string, string>(claim.Type, claim.Value));
                }
            }

            return new IdentityResponse(claims);
        }
    }
}