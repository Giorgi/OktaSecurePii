using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace ExpenseTracker.Identity
{
    public class OktaUserManager<T> : UserManager<T> where T : IdentityUser<string>, new()
    {
        private OktaClient oktaClient;

        public OktaUserManager(IConfiguration configuration, IUserStore<T> store, IOptions<IdentityOptions> optionsAccessor,
                               IPasswordHasher<T> passwordHasher, IEnumerable<IUserValidator<T>> userValidators,
                               IEnumerable<IPasswordValidator<T>> passwordValidators, ILookupNormalizer keyNormalizer,
                               IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<T>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = configuration["Okta:Domain"],
                Token = configuration["Okta:ApiToken"]
            });
        }

        public override async Task<IdentityResult> CreateAsync(T user, string password)
        {
            var validate = await ValidatePasswordAsync(user, password);
            if (!validate.Succeeded)
            {
                return validate;
            }

            var result = await CreateAsync(user);

            if (!result.Succeeded)
            {
                return result;
            }

            await oktaClient.Users.PartialUpdateUserAsync(new User
            {
                Credentials = new UserCredentials
                {
                    Password = new PasswordCredential
                    {
                        Value = password
                    }
                }
            }, user.Id);

            return result;
        }
    }
}
