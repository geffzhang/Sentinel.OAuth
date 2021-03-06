﻿namespace Sentinel.Tests.Unit
{
    using Microsoft.AspNet.Identity;
    using NUnit.Framework;
    using Sentinel.OAuth.Core.Extensions;
    using Sentinel.OAuth.Core.Models.Tokens;
    using Sentinel.OAuth.Models.Identity;
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Sentinel.OAuth.Extensions;

    [TestFixture]
    public class JsonWebTokenTests
    {
        [TestCase("HS256", "NUnit", "https://sentinel.oauth/", 1445850630, 1445854230, "azzlack", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1bmlxdWVfbmFtZSI6ImF6emxhY2siLCJhdXRobWV0aG9kIjoidXNlcl9jcmVkZW50aWFscyIsImdpdmVuX25hbWUiOiJPdmUiLCJmYW1pbHlfbmFtZSI6IkFuZGVyc2VuIiwidXJuOm9hdXRoOmNsaWVudCI6Ik5Vbml0IiwidXJuOm9hdXRoOmdyYW50dHlwZSI6InBhc3N3b3JkIiwidXJuOm9hdXRoOnNjb3BlIjoib3BlbmlkIiwic3ViIjoiYXp6bGFjayIsImF0X2hhc2giOiJlSEZ1YVVjek9VaHdiMjExV1ZRclVtdGtRM2hqTjJoMFlUTnJiMEpyYzFKWVVHdGhLMWRMT1hJemVFNVNPVVZrVDFOeFJDOUdOSG95U2xWcWEyMUxNUT09IiwiaXNzIjoiaHR0cHM6Ly9zZW50aW5lbC5vYXV0aC8iLCJhdWQiOiJOVW5pdCIsImV4cCI6MTQ0NTg1NDIzMCwibmJmIjoxNDQ1ODUwNjMwfQ.m0m0iyCqssawb44VE8ANUGJMIBppUx1AnSCbSCfNdeM")]
        [TestCase("http://www.w3.org/2001/04/xmldsig-more#hmac-sha512", "Divvy.Web", "https://www.divvy.no/", 1446191826, 1446235026, "johgis", "eyJ0eXAiOiJKV1QiLCJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiJ9.eyJ1bmlxdWVfbmFtZSI6ImpvaGdpcyIsInJvbGUiOiJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L3JvbGUvdXNlciIsImh0dHA6Ly91bHRyYXJlZy5rbm93aXQubm8vaWRlbnRpdHkvY2xhaW0vbGFuZ3VhZ2UiOiJuYi1ubyIsImh0dHA6Ly91bHRyYXJlZy5rbm93aXQubm8vaWRlbnRpdHkvY2xhaW0vdGVhbSI6IjM0IiwiaHR0cDovL3VsdHJhcmVnLmtub3dpdC5uby9pZGVudGl0eS9jbGFpbS9jb21wYW55IjoiS1lCRVIiLCJlbWFpbCI6IkpvaGFubmVzLkdpc2tlQGtub3dpdC5ubyIsImdpdmVuX25hbWUiOiJKb2hhbm5lcyIsImZhbWlseV9uYW1lIjoiR2lza2UiLCJ3aW5hY2NvdW50bmFtZSI6Imtub3dpdFxcam9oZ2lzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrNDcgOTU0OTc3NzkiLCJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L2NsYWltL2F1dGhlbnRpY2F0aW9uc291cmNlIjoiTERBUDovLzEwLjEuMTAuMTAvREM9a25vd2l0LERDPWxvY2FsIiwidXJuOm9hdXRoOmNsaWVudCI6IkRpdnZ5LldlYiIsInVybjpvYXV0aDpyZWRpcmVjdHVyaSI6Imh0dHBzOi8vd2ViLmRpdnZ5Lm5vLyIsInVybjpvYXV0aDpzY29wZSI6WyJvcGVuaWQiLCJvcGVuaWQiXSwidXJuOm9hdXRoOmdyYW50dHlwZSI6InJlZnJlc2hfdG9rZW4iLCJuYW1laWQiOiJqb2hnaXMiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL2FjY2Vzc2NvbnRyb2xzZXJ2aWNlLzIwMTAvMDcvY2xhaW1zL2lkZW50aXR5cHJvdmlkZXIiOiJVbHRyYVJlZyIsInN1YiI6ImpvaGdpcyIsImF0X2hhc2giOiJUVmczUm5NMlpreDZlV3BLZFRGdFkydGxha2h0Vm05d1owaHVOV05wVjFSdGEzTlRWekpLTkU1V2RGTT0iLCJpc3MiOiJodHRwczovL3d3dy5kaXZ2eS5uby8iLCJhdWQiOiJEaXZ2eS5XZWIiLCJleHAiOjE0NDYyMzUwMjYsIm5iZiI6MTQ0NjE5MTgyNn0.NhSrl_6VDEgxTpo9LX2ZOzfceNjI3XZpCVI0xoCid928qxzSvszNf5J4hmZg_ZKz-h907VbMUf-eEhH9iN_wNw")]
        [TestCase("RS256", "https://contoso.com", "https://sts.windows.net/e481747f-5da7-4538-cbbe-67e57f7d214e/", 1391210850, 1391214450, "21749daae2a91137c259191622fa1", "eyJhbGciOiJSUzI1NiIsIng1dCI6IjdkRC1nZWNOZ1gxWmY3R0xrT3ZwT0IyZGNWQSIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL2NvbnRvc28uY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZTQ4MTc0N2YtNWRhNy00NTM4LWNiYmUtNjdlNTdmN2QyMTRlLyIsIm5iZiI6MTM5MTIxMDg1MCwiZXhwIjoxMzkxMjE0NDUwLCJzdWIiOiIyMTc0OWRhYWUyYTkxMTM3YzI1OTE5MTYyMmZhMSJ9.C4Ny4LeVjEEEybcA1SVaFYFS6nH-Ezae_RrTXUYInjXGt-vBOkAa2ryb-kpOlzU_R4Ydce9tKDNp1qZTomXgHjl-cKybAz0Ut90-dlWgXGvJYFkWRXJ4J0JyS893EDwTEHYaAZH_lCBvoYPhXexD2yt1b-73xSP6oxVlc_sMvz3DY__1Y_OyvbYrThHnHglxvjh88x_lX7RN-Bq82ztumxy97rTWaa_1WJgYuy7h7okD24FtsD9PPLYAply0ygl31ReI0FZOdX12Hl4THJm4uI_4_bPXL6YR2oZhYWp-4POWIPHzG9c_GL8asBjoDY9F5q1ykQiotUBESoMML7_N1g")]
        public void Construct_WhenGivenValidJwt_ReturnsJwt(string expectedAlgorithm, string expectedAudience, string expectedIssuer, long expectedValidFrom, long expectedExpires, string expectedSubject, string token)
        {
            var jwt = new JsonWebToken(token);

            Assert.AreEqual(expectedAlgorithm, jwt.Header.Algorithm);
            Assert.AreEqual("JWT", jwt.Header.Type);
            Assert.AreEqual(expectedAudience, jwt.Payload.Audience);
            Assert.AreEqual(new Uri(expectedIssuer), jwt.Payload.Issuer);
            Assert.AreEqual(expectedValidFrom, jwt.Payload.ValidFrom.ToUnixTime());
            Assert.AreEqual(expectedExpires, jwt.Payload.Expires.ToUnixTime());
            Assert.AreEqual(expectedSubject, jwt.Payload.Subject);
            Assert.IsNotNullOrEmpty(jwt.Signature);
        }

        [TestCase("http://www.w3.org/2001/04/xmldsig-more#hmac-sha512", "Divvy.Web", "https://www.divvy.no/", 1446191826, 1446235026, "johgis", new[] { "http://ultrareg.knowit.no/identity/role/user" }, "eyJ0eXAiOiJKV1QiLCJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiJ9.eyJ1bmlxdWVfbmFtZSI6ImpvaGdpcyIsInJvbGUiOiJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L3JvbGUvdXNlciIsImh0dHA6Ly91bHRyYXJlZy5rbm93aXQubm8vaWRlbnRpdHkvY2xhaW0vbGFuZ3VhZ2UiOiJuYi1ubyIsImh0dHA6Ly91bHRyYXJlZy5rbm93aXQubm8vaWRlbnRpdHkvY2xhaW0vdGVhbSI6IjM0IiwiaHR0cDovL3VsdHJhcmVnLmtub3dpdC5uby9pZGVudGl0eS9jbGFpbS9jb21wYW55IjoiS1lCRVIiLCJlbWFpbCI6IkpvaGFubmVzLkdpc2tlQGtub3dpdC5ubyIsImdpdmVuX25hbWUiOiJKb2hhbm5lcyIsImZhbWlseV9uYW1lIjoiR2lza2UiLCJ3aW5hY2NvdW50bmFtZSI6Imtub3dpdFxcam9oZ2lzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrNDcgOTU0OTc3NzkiLCJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L2NsYWltL2F1dGhlbnRpY2F0aW9uc291cmNlIjoiTERBUDovLzEwLjEuMTAuMTAvREM9a25vd2l0LERDPWxvY2FsIiwidXJuOm9hdXRoOmNsaWVudCI6IkRpdnZ5LldlYiIsInVybjpvYXV0aDpyZWRpcmVjdHVyaSI6Imh0dHBzOi8vd2ViLmRpdnZ5Lm5vLyIsInVybjpvYXV0aDpzY29wZSI6WyJvcGVuaWQiLCJvcGVuaWQiXSwidXJuOm9hdXRoOmdyYW50dHlwZSI6InJlZnJlc2hfdG9rZW4iLCJuYW1laWQiOiJqb2hnaXMiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL2FjY2Vzc2NvbnRyb2xzZXJ2aWNlLzIwMTAvMDcvY2xhaW1zL2lkZW50aXR5cHJvdmlkZXIiOiJVbHRyYVJlZyIsInN1YiI6ImpvaGdpcyIsImF0X2hhc2giOiJUVmczUm5NMlpreDZlV3BLZFRGdFkydGxha2h0Vm05d1owaHVOV05wVjFSdGEzTlRWekpLTkU1V2RGTT0iLCJpc3MiOiJodHRwczovL3d3dy5kaXZ2eS5uby8iLCJhdWQiOiJEaXZ2eS5XZWIiLCJleHAiOjE0NDYyMzUwMjYsIm5iZiI6MTQ0NjE5MTgyNn0.NhSrl_6VDEgxTpo9LX2ZOzfceNjI3XZpCVI0xoCid928qxzSvszNf5J4hmZg_ZKz-h907VbMUf-eEhH9iN_wNw")]
        [TestCase("http://www.w3.org/2001/04/xmldsig-more#hmac-sha512", "Divvy.Web", "https://www.divvy.no/", 1446191826, 1446235026, "ovea", new[] { "http://ultrareg.knowit.no/identity/role/user", "http://ultrareg.knowit.no/identity/role/projectmanager", "http://ultrareg.knowit.no/identity/role/billingmanager" }, "eyJ0eXAiOiJKV1QiLCJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiJ9.eyJ3aW5hY2NvdW50bmFtZSI6Imtub3dpdFxcb3ZlYW5kIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrNDcgOTkwMTY2MTAiLCJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L2NsYWltL2F1dGhlbnRpY2F0aW9uc291cmNlIjoiTERBUDovLzEwLjEuMTAuMTAvREM9a25vd2l0LERDPWxvY2FsIiwidW5pcXVlX25hbWUiOiJvdmVhIiwicm9sZSI6WyJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L3JvbGUvdXNlciIsImh0dHA6Ly91bHRyYXJlZy5rbm93aXQubm8vaWRlbnRpdHkvcm9sZS9wcm9qZWN0bWFuYWdlciIsImh0dHA6Ly91bHRyYXJlZy5rbm93aXQubm8vaWRlbnRpdHkvcm9sZS9iaWxsaW5nbWFuYWdlciJdLCJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L2NsYWltL2xhbmd1YWdlIjoibmItbm8iLCJodHRwOi8vdWx0cmFyZWcua25vd2l0Lm5vL2lkZW50aXR5L2NsYWltL3RlYW0iOiIxIiwiaHR0cDovL3VsdHJhcmVnLmtub3dpdC5uby9pZGVudGl0eS9jbGFpbS9jb21wYW55IjoiUkMiLCJlbWFpbCI6Im92ZS5hbmRlcnNlbkBrbm93aXQubm8iLCJnaXZlbl9uYW1lIjoiT3ZlIiwiZmFtaWx5X25hbWUiOiJBbmRlcnNlbiIsInVybjpvYXV0aDpjbGllbnQiOiJEaXZ2eS5XZWIiLCJ1cm46b2F1dGg6Z3JhbnR0eXBlIjoicGFzc3dvcmQiLCJuYW1laWQiOiJvdmVhIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS9hY2Nlc3Njb250cm9sc2VydmljZS8yMDEwLzA3L2NsYWltcy9pZGVudGl0eXByb3ZpZGVyIjoiVWx0cmFSZWciLCJ1cm46b2F1dGg6c2NvcGUiOiJvcGVuaWQiLCJzdWIiOiJvdmVhIiwiYXRfaGFzaCI6ImJuZEhZWFpZVm1KMk5EVkViak40VFROSE9HZ3ZaV0o1WkZkYWFqaDZhV2RYVFVKTVJYazNLelpVT1VrPSIsImlzcyI6Imh0dHBzOi8vd3d3LmRpdnZ5Lm5vLyIsImF1ZCI6IkRpdnZ5LldlYiIsImV4cCI6MTQ0NjIzMzU1MywibmJmIjoxNDQ2MTkwMzUzfQ.nDpakDgHBwpg2yoXQgK58Ln6Dr5K_v5fE-r2jLI6joAV52LTTCUZ3i_NFRvnQjXK3Byx4Cmo3hRCArIawxCi7g")]
        public void ConvertToIdentity_WhenGivenValidJwt_ReturnsValidIdentity(string expectedAlgorithm, string expectedAudience, string expectedIssuer, long expectedValidFrom, long expectedExpires, string expectedSubject, string[] expectedRoles, string token)
        {
            var jwt = new JsonWebToken(token);
            var cookieIdentity = new SentinelIdentity(DefaultAuthenticationTypes.ApplicationCookie, jwt);

            CollectionAssert.AreEquivalent(expectedRoles, cookieIdentity.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToArray());
        }

        [TestCase("QAXDyzzUPZG_JNXEvPQKCh@0F5zKioy@", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1bmlxdWVfbmFtZSI6InVzZXIiLCJuYW1laWQiOiJ1c2VyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS9hY2Nlc3Njb250cm9sc2VydmljZS8yMDEwLzA3L2NsYWltcy9pZGVudGl0eXByb3ZpZGVyIjoiU2VudGluZWwiLCJ1cm46b2F1dGg6Y2xpZW50IjoiTlVuaXQiLCJ1cm46b2F1dGg6c2NvcGUiOiJvcGVuaWQiLCJjX2hhc2giOiJSWDRtRlZNMGRjdWQ5NTlBM3hIMjhnPT0iLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjU1MzQ4LyIsImF1ZCI6Ik5Vbml0IiwiZXhwIjoxNDU2NDEwMjgxLCJuYmYiOjE0NTY0MDY2ODEsInVybjpvYXV0aDpncmFudHR5cGUiOiJhdXRob3JpemF0aW9uX2NvZGUiLCJzdWIiOiJ1c2VyIiwiYXRfaGFzaCI6ImMybG1UeXRhTkhCWVFWRktaV3N5ZVhVM2VEaFljUT09In0.J-t0QmVtidiM3BmZklKVMUxwXdDkvgE0qDsG32A4u6Q")]
        public void ValidateAuthorizationCode_WhenGivenValidCodeAndHash_ReturnsTrue(string code, string idToken)
        {
            var jwt = new JsonWebToken(idToken);

            Assert.IsTrue(jwt.ValidateAuthorizationCode(code));
        }

        [TestCase("!ooDLKT_GZaj0W1iF_AgQ7kNCaF6PDnUWAiaG0etThykr!ACnhPAzywZSX$RKxc_", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1bmlxdWVfbmFtZSI6InVzZXIiLCJuYW1laWQiOiJ1c2VyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS9hY2Nlc3Njb250cm9sc2VydmljZS8yMDEwLzA3L2NsYWltcy9pZGVudGl0eXByb3ZpZGVyIjoiU2VudGluZWwiLCJ1cm46b2F1dGg6Y2xpZW50IjoiTlVuaXQiLCJ1cm46b2F1dGg6c2NvcGUiOiJvcGVuaWQiLCJjX2hhc2giOiJHbEhyUVE5UVJiVzRqVDBLSkZwdWpBPT0iLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjU1MzQ4LyIsImF1ZCI6Ik5Vbml0IiwiZXhwIjoxNDU2NDEzNDk1LCJuYmYiOjE0NTY0MDk4OTUsInVybjpvYXV0aDpncmFudHR5cGUiOiJhdXRob3JpemF0aW9uX2NvZGUiLCJzdWIiOiJ1c2VyIiwiYXRfaGFzaCI6InEwYkh0RmRVdUE5MHkyZDlVSktGSVE9PSJ9.k5QiSnP-etA8OxhJAFhJyO2y1u6K-IHpUsNWYnyXKXk")]
        public void ValidateAccessToken_WhenGivenValidTokenAndHash_ReturnsTrue(string accessToken, string idToken)
        {
            var jwt = new JsonWebToken(idToken);

            Assert.IsTrue(jwt.ValidateAccessToken(accessToken));
        }
    }
}