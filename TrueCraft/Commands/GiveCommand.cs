using System;
using System.Linq;
using System.Text.RegularExpressions;
using TrueCraft.Core.Windows;
using TrueCraft.API;
using TrueCraft.API.Networking;

namespace TrueCraft.Commands
{
    public class GiveCommand : Command
    {
        public override string Name
        {
            get { return "give"; }
        }

        public override string Description
        {
            get { return "Give the specified player an amount of items."; }
        }

        public override string[] Aliases
        {
            get { return new string[1]{ "i" }; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            var regexMatch = GetValuesFromArguments(arguments);
            if (regexMatch == null)
            {
                Help(client, alias, arguments);
                return;
            }

            string  username    = regexMatch.Groups[1].ToString(),
                    itemid      = regexMatch.Groups[2].ToString(),
                    amount      = regexMatch.Groups[4].ToString(); // match 3 is the amount with the leading space

            if (String.IsNullOrEmpty(amount)) amount = "1"; // default to 1 when amount is omitted
            
            var receivingPlayer =
                client.Server.Clients.FirstOrDefault(c => String.Equals(c.Username, username, StringComparison.CurrentCultureIgnoreCase));

            short id;
            int count;

            if (short.TryParse(itemid, out id) && Int32.TryParse(amount, out count))
            {
                if (receivingPlayer == null)
                {
                    client.SendMessage("No client with the username \"" + username + "\" was found.");
                    return;
                }

                if (client.Server.ItemRepository.GetItemProvider(id) == null)
                {
                    client.SendMessage("Invalid item id \"" + id + "\".");
                    return;
                }

                var inventory = receivingPlayer.Inventory as InventoryWindow;
                if (inventory != null)
                {
                    sbyte toAdd;
                    while (count > 0)
                    {
                        if (count >= 64)
                            toAdd = 64;
                        else
                            toAdd = (sbyte)count;

                        count -= toAdd;

                        inventory.PickUpStack(new ItemStack(id, toAdd));
                    }
                }
            }
        }

        private Match GetValuesFromArguments(string[] arguments)
        {
            try
            {
                var myRegex = new Regex(@"^give ([a-zA-Z0-9_\.]+) ([0-9]+)( ([0-9]+))?$", RegexOptions.IgnoreCase);
                return myRegex.Matches(String.Join(" ", arguments)).Cast<Match>().First(myMatch => myMatch.Success);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + " <User> <Item ID> [Amount]");
        }
    }
}
