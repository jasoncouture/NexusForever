using System;
using System.Net.Http;
using System.Threading.Tasks;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Client;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Account Management")]
    public class AccountCommandHandler : CommandCategory
    {
        public AccountCommandHandler()
            : base(false, "acc", "account")
        {
        }

        [SubCommandHandler("create", "email password - Create a new account")]
        public async Task HandleAccountCreate(CommandContext context, string subCommand, string[] parameters)
        {
            try
            {
                var id = await AuthClient.Client.CreateAccountAsync(parameters[0], parameters[1], AccountType.User);
            }
            catch (AccountAlreadyExistsException)
            {
                await context.SendMessageAsync("That account already exists");
                return;
            }
            catch (Exception ex)
            {
                await context.SendMessageAsync("An unknown error occured, please try again: " + ex.Message);
                Logger.Error(ex, "Error creating account");
                return;
                // TODO
            }

            await context.SendMessageAsync($"Account {parameters[0]} created successfully")
                .ConfigureAwait(false);
        }

        [SubCommandHandler("delete", "email - Delete an account")]
        public async Task HandleAccountDeleteAsync(CommandContext context, string subCommand, string[] parameters)
        {
            if (await AuthClient.Client.DeleteAccountAsync(parameters[0]))
                await context.SendMessageAsync($"Account {parameters[0]} successfully removed!")
                    .ConfigureAwait(false);
            else
                await context.SendMessageAsync($"Cannot find account with Email: {parameters[0]}")
                    .ConfigureAwait(false);
        }
    }
}
