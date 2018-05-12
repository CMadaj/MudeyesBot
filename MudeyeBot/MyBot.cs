using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net;

namespace MudeyeBot
{

    class MyBot
    {
        private List<Blackjack> blackjackGames = new List<Blackjack>();

        
        //MySQL sMud = new MySQL();
        DiscordClient discord;
        CommandService commands;

        string[,] dice = new string[,] { { "┌───────┐", "", "" }, { "│       │", "│   ☻   │", "│       │" }, { "│ ☻     │", "│       │", "│     ☻ │" }, { "│ ☻     │", "│   ☻   │", "│     ☻ │" }, { "│ ☻   ☻ │", "│       │", "│ ☻   ☻ │" }, { "│ ☻   ☻ │", "│   ☻   │", "│ ☻   ☻ │" }, { "│ ☻ ☻ ☻ │", "│       │", "│ ☻ ☻ ☻ │" }, { "└───────┘", "", "" } };

        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands( x =>
                {
                    x.PrefixChar = '-';
                    x.AllowMentionPrefix = true;
                });
            
            commands = discord.GetService<CommandService>();

            #region easystreet cmd
            commands.CreateCommand("easystreet")
                .Do(async (e) =>
                {
                    string output = "```";
                    output += '\n' + "We're on easy street";
                    output += '\n' + "And it feels so sweet";
                    output += '\n' + "'Cause the world is 'bout a treat";
                    output += '\n' + "When you're on easy street";
                    output += '\n' + "And we're breaking out the good champagne";
                    output += '\n' + "We're sitting pretty on the gravy train";
                    output += '\n' + "And when we sing every sweet refrain repeats";
                    output += '\n' + "Right here on easy street";
                    output += '\n' + "```";
                    await e.Channel.SendMessage(output);
                });
            #endregion
            #region boss cmd
            commands.CreateCommand("boss")
                .Do(async (e) =>
                {
                    string output = "```";
                    output += '\n' + e.User.Name;
                    output += ", you are a boss.";
                    output += '\n' + "```";
                    await e.Channel.SendMessage(output);
                });
            #endregion
            #region flip cmd
            commands.CreateCommand("flip")
                .Do(async (e) =>
                {
                    int face = GlobalVars.rnd.Next(1, 3); // creates a number between 1 and 2
                    if(face==1)
                    {
                        await e.Channel.SendMessage("The coin lands on HEADS.");
                    }
                    else
                    {
                        await e.Channel.SendMessage("The coin lands on TAILS.");
                    }
                });
            #endregion
            #region random cmd
            commands.CreateCommand("random")
                .Parameter("low", ParameterType.Required)
                .Parameter("high", ParameterType.Required)
                .Do(async (e) =>
                {
                    int floor = Int32.Parse(e.GetArg("low"));
                    int roof = Int32.Parse(e.GetArg("high"));
                    int num = GlobalVars.rnd.Next(floor, roof + 1);
                    string output = "```" + "Your number is: " + num + "```";
                    await e.Channel.SendMessage(output);
                });
            #endregion
            #region purgeold cmd
            commands.CreateCommand("purgeold")
                .Parameter("num", ParameterType.Required)
                .Do(async (e) =>
                {
                    int num = Int32.Parse(e.GetArg("num"));
                    if (num > 0)
                    {
                        int left = num + 1;
                        int count = -1;
                        Message[] messagesToDelete;
                        while (left > 0)
                        {
                            if (left > 100)
                            {
                                messagesToDelete = await e.Channel.DownloadMessages(100);
                                left -= 100;
                            }
                            else
                            {
                                messagesToDelete = await e.Channel.DownloadMessages(left);
                            }
                            count += messagesToDelete.Length;
                            if (messagesToDelete.Length < 100)
                            {
                                left = 0;
                            }
                            await e.Channel.DeleteMessages(messagesToDelete);
                            await Task.Delay(400);
                        }
                        string output = "**```==================================" + '\n' + string.Format("{0:n0}/{1:n0} messages removed.", count, num) + '\n' + "==================================```**";
                        Message notice = await e.Channel.SendMessage(output);
                        await Task.Delay(3000);
                        await e.Channel.DeleteMessages(new ulong[] { notice.Id });
                    }
                    
                });
            #endregion
            #region purge cmd
            commands.CreateCommand("purge")
                .Parameter("num", ParameterType.Required)
                .Do(async (e) =>
                {
                    int num = Int32.Parse(e.GetArg("num"));
                    if (num > 0)
                    {
                        int left = num + 1;
                        int count = -1;
                        Message[] messagesToDelete;
                        while (left > 0)
                        {
                            messagesToDelete = await e.Channel.DownloadMessages(1);
                            left -= 1;
                            count += 1;

                            if (messagesToDelete.Length < 1)
                            {
                                left = 0;
                            }
                            await e.Channel.DeleteMessages(messagesToDelete);
                            //await Task.Delay(400);
                        }
                        string output = "**```==================================" + '\n' + string.Format("{0:n0}/{1:n0} messages removed!", count, num) + '\n' + "==================================```**";
                        Message notice = await e.Channel.SendMessage(output);
                        await Task.Delay(3000);
                        await e.Channel.DeleteMessages(new ulong[] { notice.Id });
                    }

                });
            #endregion
            #region userid cmd
            commands.CreateCommand("userid")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(e.User.Name + "'s ID is " + e.User.Id + ".");
                });
            #endregion
            #region help cmd
            commands.CreateCommand("help")
                .Do(async (e) =>
                {
                    string output = "```";
                    output += '\n' + "Commands:";
                    output += '\n' + "-balance [Show your chip count]";
                    output += '\n' + "-balancetop [Show the top 10 chip holders]";
                    output += '\n' + "-blackjack/bj x [Play blackjack by betting x chips]";
                    output += '\n' + "-easystreet [We're on...]";
                    output += '\n' + "-flip [Flip a coin]";
                    output += '\n' + "-help [Show this menu]";
                    output += '\n' + "-purge x [Remove x amount of messages]";
                    output += '\n' + "-random x y [Get a random number from x to y]";
                    output += '\n' + "-roll x [Roll x amount of dice]";
                    output += '\n' + "-userid [show your Discord ID]";
                    output += '\n' + "```";
                    await e.Channel.SendMessage(output);
                });
            #endregion
            /**#region getchips cmd
            commands.CreateCommand("getchips")
                .Do(async (e) =>
                {
                    string output = "";
                    await e.Channel.SendMessage(output);
                });
            #endregion**/
            #region spam cmd
            commands.CreateCommand("spam")
                .Parameter("num", ParameterType.Required)
                .AddCheck((cm, u, ch) => u.ServerPermissions.BanMembers)
                .Do(async (e) =>
                {
                    int num = Int32.Parse(e.GetArg("num"));
                    while (num > 0)
                    {
                        await Task.Delay(500);
                        await e.Channel.SendMessage(""+num);
                        num -= 1;
                    }
                });
            #endregion
            #region balance cmd
            commands.CreateCommand("balance")
                .Do(async (e) =>
                {
                    MySqlConnection connection = null;
                    MySqlDataReader rdr = null;
                    try
                    {
                        connection = new MySqlConnection(GlobalVars.ConnectionSTR);
                        connection.Open();

                        string stm = "SELECT * FROM Users WHERE ID=" + e.User.Id;
                        MySqlCommand command = new MySqlCommand(stm, connection);
                        rdr = command.ExecuteReader();
                        
                        if (rdr.Read())
                        {
                            await e.Channel.SendMessage(string.Format("{0:s} has {1:n0} chips.", e.User.Name, rdr.GetInt32(1)));
                        }
                        else
                        {
                            rdr.Close();
                            command = new MySqlCommand();
                            command.Connection = connection;
                            command.CommandText = "insert into users (ID,chips,username) VALUES (@ID,@CHIPS,@USER)";
                            command.Prepare();
                            command.Parameters.AddWithValue("@ID", e.User.Id);
                            command.Parameters.AddWithValue("@CHIPS", 100);
                            command.Parameters.AddWithValue("@USER", e.User.Name);
                            command.ExecuteNonQuery();
                            await e.Channel.SendMessage("You have been given 100 chips to start!");
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: {0}", ex.ToString());
                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            rdr.Close();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                        }
                    } 
                });
            #endregion
            #region balancetop cmd
            commands.CreateCommand("balancetop")
                .Do(async (e) =>
                {
                    MySqlConnection connection = null;
                    MySqlDataReader rdr = null;
                    try
                    {
                        connection = new MySqlConnection(GlobalVars.ConnectionSTR);
                        connection.Open();

                        string stm = "SELECT * FROM Users";
                        MySqlCommand command = new MySqlCommand(stm, connection);
                        rdr = command.ExecuteReader();
                        List<int> chipcount = new List<int>();
                        List<string> username = new List<string>();
                        while (rdr.Read())
                        {
                            chipcount.Add(rdr.GetInt32(1));
                            username.Add(rdr.GetString(2));
                        }
                        int[] iChipcount = chipcount.ToArray();
                        string[] iUsername = username.ToArray();
                        Array.Sort(iChipcount,iUsername);
                        Array.Reverse(iChipcount);
                        Array.Reverse(iUsername);
                        int count = 10;
                        if (iChipcount.Length < 10)
                        {
                            count = iChipcount.Length;
                        }
                        string output = "```" + '\n';
                        output += '\n' + "Chip Leaderboard";
                        output += '\n' + "──────────────────────────────────";
                        for (int i = 0; i < count; i++ )
                        {
                            output += '\n';
                            output += String.Format("{0,-2}| {1,-20} {2:n0}", i+1, iUsername[i], iChipcount[i]);
                        }
                        output += '\n' + "```";
                        await e.Channel.SendMessage(output);
                        
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: {0}", ex.ToString());
                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            rdr.Close();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                        }
                    }
                });
            #endregion
            #region blackjack cmd
            commands.CreateCommand("blackjack")
                .Parameter("bet", ParameterType.Required)
                .Alias(new string[] { "bj" }) 
                .Do(async (e) =>
                {
                    string betString = e.GetArg("bet");
                    int bet;
                    bool isNumeric = int.TryParse(betString, out bet);
                    bool trigger = true;
                    for (int i = 0; i < blackjackGames.Count; i++)
                    {
                        if (blackjackGames[i].OwnerID == (long)e.User.Id)
                        {
                            //await e.Channel.SendMessage("Game already in progress.");
                            string output = string.Format("A game is already in progress with a bet of {0:n0} chips!", blackjackGames[i].Bet);
                            output += '\n' + "```";
                            output += blackjackGames[i].ShowHand(0);
                            output += blackjackGames[i].ShowHand(2);
                            if(blackjackGames[i].hand.Count>4){
                                output += '\n' + "Hit or stand?";
                            }
                            else{
                                output += '\n' + "Hit, stand, or double down?";
                            }
                            output += '\n' + "```";
                            await e.Channel.SendMessage(output);
                            trigger = false;
                        }
                    }
                        if (isNumeric && trigger)
                        {
                            Blackjack game = null;
                            MySqlConnection connection = null;
                            MySqlDataReader rdr = null;
                            try
                            {
                                connection = new MySqlConnection(GlobalVars.ConnectionSTR);
                                connection.Open();

                                string stm = "SELECT * FROM Users WHERE ID=" + e.User.Id;
                                MySqlCommand command = new MySqlCommand(stm, connection);
                                rdr = command.ExecuteReader();

                                if (rdr.Read())
                                {
                                    long playerID = rdr.GetInt64(0);
                                    int playerChips = rdr.GetInt32(1);
                                    string playerName = rdr.GetString(2);

                                    if (bet > playerChips)
                                    {
                                        await e.Channel.SendMessage(string.Format("You only have {0:n0} chips to bet!", playerChips));
                                        throw new Exception();
                                    }
                                    if (bet < 1)
                                    {
                                        await e.Channel.SendMessage("You have to bet at least 1 chip!");
                                        throw new Exception();
                                    }
                                    game = new Blackjack(playerID, bet, playerChips, playerName);
                                    blackjackGames.Add(game);
                                }
                                else
                                {
                                    rdr.Close();
                                    command = new MySqlCommand();
                                    command.Connection = connection;
                                    command.CommandText = "insert into users (ID,chips,username) VALUES (@ID,@CHIPS,@USER)";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@ID", e.User.Id);
                                    command.Parameters.AddWithValue("@CHIPS", 100);
                                    command.Parameters.AddWithValue("@USER", e.User.Name);
                                    command.ExecuteNonQuery();
                                    await e.Channel.SendMessage("You have been given 100 chips to start!");
                                    throw new Exception();
                                }
                            }
                            catch (MySqlException ex)
                            {
                                Console.WriteLine("Error: {0}", ex.ToString());
                            }
                            catch
                            {
                                //checking for invalid bet/user had no SQL row
                            }
                            finally
                            {
                                if (rdr != null)
                                {
                                    rdr.Close();
                                }
                                if (connection != null)
                                {
                                    connection.Close();
                                }
                            }
                            string output = "```";
                            output += game.ShowHand(0);
                            output += game.ShowHand(2);
                            output += '\n' + "Hit, stand, or double down?";
                            output += '\n' + "```";
                            await e.Channel.SendMessage(output);
                        }
                });
            #endregion
            #region hit cmd
            commands.CreateCommand("hit")
                .Alias(new string[] { "h" }) 
                .Do(async (e) =>
                {
                    Blackjack game = null;
                    for (int i = 0; i < blackjackGames.Count; i++)
                    {
                        if (blackjackGames[i].OwnerID == (long)e.User.Id)
                        {
                            game = blackjackGames[i];
                        }
                    }
                    if (game != null){
                        string output;
                        game.AddCard(1);
                        game.handSums = game.GetSums(game.hand);
                        if (game.handSums.Count==0)
                        {
                            output = "```";
                            output += game.ShowHand(1);
                            output += game.ShowHand(2);
                            output += game.End("lose");
                            blackjackGames.Remove(game);
                            output += '\n' + "```";
                        }
                        else
                        {
                            if (game.handSums[0] != 21)
                            {
                                output = "```";
                                output += game.ShowHand(0);
                                output += game.ShowHand(2);
                                output += '\n' + "Hit or stand?";
                                output += '\n' + "```";
                            }
                            else
                            {
                                output = game.DealerTurn();
                                blackjackGames.Remove(game);
                            }
                        }
                        
                        await e.Channel.SendMessage(output);
                    }
                });
            #endregion
            #region stand cmd
            commands.CreateCommand("stand")
                .Alias(new string[] { "s" }) 
                .Do(async (e) =>
                {
                    Blackjack game = null;
                    for (int i = 0; i < blackjackGames.Count; i++)
                    {
                        if (blackjackGames[i].OwnerID == (long)e.User.Id)
                        {
                            game = blackjackGames[i];
                        }
                    }
                    if (game != null)
                    {
                        string output = game.DealerTurn();
                        blackjackGames.Remove(game);
                        await e.Channel.SendMessage(output);
                    }
                });
            #endregion
            #region doubledown cmd
            commands.CreateCommand("doubledown")
                .Alias(new string[] { "dd" }) 
                .Do(async (e) =>
                {
                    Blackjack game = null;
                    for (int i = 0; i < blackjackGames.Count; i++)
                    {
                        if (blackjackGames[i].OwnerID == (long)e.User.Id)
                        {
                            game = blackjackGames[i];
                        }
                    }
                    if (game != null && game.hand.Count==4)
                    {
                        string output;
                        if (game.Bet * 2 > game.Chips) 
                        {
                            output = "You don't have enough chips to double down.";
                        }
                        else
                        {
                            game.Bet *= 2;
                            game.AddCard(1);
                            game.handSums = game.GetSums(game.hand);
                            if (game.handSums.Count == 0)
                            {
                                output = "```";
                                output += game.ShowHand(1);
                                output += game.ShowHand(2);
                                output += game.End("lose");
                                output += '\n' + "```";
                            }
                            else
                            {
                                output = game.DealerTurn();
                            }
                            blackjackGames.Remove(game);
                        }
                        await e.Channel.SendMessage(output);
                    }
                });
            #endregion
            #region roll cmd
            commands.CreateCommand("roll")
                .Parameter("num", ParameterType.Required)
                .Do(async (e) =>
                {
                    int num = Int32.Parse(e.GetArg("num"));
                    if (num > 0 && num < 11)
                    {
                        List<int> rolls = new List<int>();
                        for (int i = num; i > 0; i--)
                        {
                            rolls.Add(GlobalVars.rnd.Next(1, 7));
                        }
                        string output = "```" + num + " dice rolled:";
                        int sum = 0;
                        int x = 0;
                        while (x < num)
                        {
                            if (x + 5 < num)
                            {
                                output += '\n';
                                for (int i = x; i < x + 5; i++)
                                {
                                    output += dice[0, 0];
                                    sum += rolls[i];
                                }
                                output += '\n';
                                for (int i = x; i < x + 5; i++)
                                {
                                    output += dice[rolls[i], 0];
                                }
                                output += '\n';
                                for (int i = x; i < x + 5; i++)
                                {
                                    output += dice[rolls[i], 1];
                                }
                                output += '\n';
                                for (int i = x; i < x + 5; i++)
                                {
                                    output += dice[rolls[i], 2];
                                }
                                output += '\n';
                                for (int i = x; i < x + 5; i++)
                                {
                                    output += dice[7, 0];
                                }
                                x += 5;
                            }
                            else
                            {
                                output += '\n';
                                for (int i = x; i < num; i++)
                                {
                                    output += dice[0, 0];
                                    sum += rolls[i];
                                }
                                output += '\n';
                                for (int i = x; i < num; i++)
                                {
                                    output += dice[rolls[i], 0];
                                }
                                output += '\n';
                                for (int i = x; i < num; i++)
                                {
                                    output += dice[rolls[i], 1];
                                }
                                output += '\n';
                                for (int i = x; i < num; i++)
                                {
                                    output += dice[rolls[i], 2];
                                }
                                output += '\n';
                                for (int i = x; i < num; i++)
                                {
                                    output += dice[7, 0];
                                }
                                x = num;
                            }
                        }
                        output += '\n' + "Total: " + sum + "```";
                        await e.Channel.SendMessage(output);
                    }
                    else
                    {
                        await e.Channel.SendMessage("**The number of dice must be between 1 and 10.**");
                    }
                });
            #endregion
            discord.ExecuteAndWait(async () =>
                {
                    await discord.Connect("REDACTED", TokenType.Bot);
                });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

