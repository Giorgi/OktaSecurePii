using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace ExpenseTracker.Identity
{
    public class OktaUserStore<T> : UserStore<T> where T : IdentityUser<string>, new()
    {
        private OktaClient oktaClient;

        public OktaUserStore(IConfiguration configuration, DbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = configuration["Okta:Domain"],
                Token = configuration["Okta:ApiToken"]
            });
        }

        public override async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await oktaClient.Users.CreateUserAsync(new CreateUserWithoutCredentialsOptions
            {
                Profile = new UserProfile()
                {
                    Login = user.NormalizedUserName,
                    Email = user.NormalizedEmail,
                    PrimaryPhone = user.PhoneNumber
                }
            }, cancellationToken: cancellationToken);

            user.Id = result.Id;

            return IdentityResult.Success;
        }

        public override async Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await oktaClient.Users.GetUserAsync(userId, cancellationToken);

            return MapUser(user);
        }

        private static T MapUser(IUser user)
        {
            if (user == null)
            {
                return null;
            }

            return new T
            {
                Id = user.Id,
                PhoneNumber = user.Profile.PrimaryPhone,
                UserName = user.Profile.Login,
                NormalizedUserName = user.Profile.Login,
                Email = user.Profile.Email,
                NormalizedEmail = user.Profile.Email,
            };
        }

        public override async Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await oktaClient.Users.ListUsers(filter: $"profile.login eq \"{normalizedUserName}\"").FirstOrDefaultAsync(cancellationToken);

            return MapUser(user);
        }

        protected override Task<T> FindUserAsync(string userId, CancellationToken cancellationToken)
        {
            return FindByIdAsync(userId, cancellationToken);
        }

        public override async Task<T> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = new CancellationToken())
        {
            return await FindByIdAsync(providerKey, cancellationToken);
        }

        public override async Task<T> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await oktaClient.Users.ListUsers(filter: $"profile.login eq \"{normalizedEmail}\"").FirstOrDefaultAsync(cancellationToken);

            return MapUser(user);
        }
    }
}