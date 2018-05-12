using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MudeyeBot
{
    class Blackjack
    {
        public List<int> hand = new List<int>();
        public List<int> handSums = new List<int>();
        public List<int> hand2 = new List<int>(); //used for splits
        public List<int> hand2Sums = new List<int>(); //used for splits
        public List<int> dealer = new List<int>();
        public List<int> dealerSums = new List<int>();
        public int[,] cards = new int[,] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
        public double WinMult = 1.34; //1.00 for default pay

        public long OwnerID;
        public string OwnerName;
        public int Bet;
        public int Chips;

        //initialize the game
        public Blackjack(long playerID, int bet, int playerChips, string playerName)
        {
            //game information
            OwnerID = playerID;
            Bet = bet;
            Chips = playerChips;
            OwnerName = playerName;

            //Give two cards to the player and dealer
            AddCard(1);
            AddCard(1);
            AddCard(0);
            AddCard(0);
        }
        //add a card to a hand
        public void AddCard(int handInt)
        {
            List<int> toWhere = null;
            if(handInt==0){
                toWhere = dealer;
            }
            else if(handInt == 1)
            {
                toWhere = hand;
            }
            
            int trigger = 1;
            while (trigger > 0)
            {
                int suit;
                int card;
                
                suit = GlobalVars.rnd.Next(0, 4);
                card = GlobalVars.rnd.Next(0, 13);
                if (cards[suit, card] == 0)
                {
                    toWhere.Add(suit);
                    toWhere.Add(card);
                    cards[suit, card] = 1;
                    trigger -= 1;
                }
            }
        }
        //get the card suit and number
        public string CardDisplay(int suit, int value)
        {
            string card = "";
            if (suit == 0)
            {
                card += "♥";
            }
            else if (suit == 1)
            {
                card += "♦";
            }
            else if (suit == 2)
            {
                card += "♣";
            }
            else if (suit == 3)
            {
                card += "♠";
            }
            else
            {
                card += " ";
            }
            if (value > 0 && value < 10)
            {
                int val = value + 1;
                card += val;
            }
            else if (value == 0)
            {
                card += "A";
            }
            else if (value == 10)
            {
                card += "J";
            }
            else if (value == 11)
            {
                card += "Q";
            }
            else if (value == 12)
            {
                card += "K";
            }
            else
            {
                card += " ";
            }
            return card;
        }
        //show a hand
        public string ShowHand(int toShow)
        {
            string handName = "";
            List<int> handShow = null;
            List<int> sums = null;
            //display first dealer card hidden
            if(toShow==0){
                List<int> dealer2 = new List<int>(dealer);
                dealer2[0] = 100;
                dealer2[1] = 100;
                sums = GetSums(dealer2);
                dealerSums = sums;
                handShow = dealer2;
                handName = "Dealer";
            }
            //display all dealer cards
            else if (toShow==1){
                sums = GetSums(dealer);
                handShow = dealer;
                handName = "Dealer";
            }
            //display user cards
            else if (toShow==2){
                sums = GetSums(hand);
                handSums = sums;
                handShow = hand;
                handName = OwnerName;
            }
            string sumsString = "";
            if (sums.Count!=0)
            {
                sumsString += " " + sums[0];
                for (int x = 1; x < sums.Count; x++)
                {
                    sumsString += " or " + sums[x];
                }
            }
            else
            {
                sumsString += " BUST";
            }
            string handString = '\n' + string.Format("{0}'s Hand:{1}", handName, sumsString);
            handString += '\n';
            for (int i = 0; i < handShow.Count / 2; i++)
            {
                handString += "┌─────┐";
            }
            handString += '\n';
            for (int i = 0; i < handShow.Count / 2; i++)
            {
                if (handShow[2 * i + 1] == 9)
                {
                    handString += "│" + CardDisplay(handShow[2 * i], handShow[2 * i + 1]) + "  │";
                }
                else
                {
                    handString += "│" + CardDisplay(handShow[2 * i], handShow[2 * i + 1]) + "   │";
                }
            }
            handString += '\n';
            for (int i = 0; i < handShow.Count / 2; i++)
            {
                handString += "│     │";
            }
            handString += '\n';
            for (int i = 0; i < handShow.Count / 2; i++)
            {
                if (handShow[2 * i + 1] == 9)
                {
                    handString += "│  " + CardDisplay(handShow[2 * i], handShow[2 * i + 1]) + "│";
                }
                else
                {
                    handString += "│   " + CardDisplay(handShow[2 * i], handShow[2 * i + 1]) + "│";
                }
            }
            handString += '\n';
            for (int i = 0; i < handShow.Count / 2; i++)
            {
                handString += "└─────┘";
            }
            return handString;
        }
        public string End(string circumstance)
        {
            string output = "";
            if (circumstance == "lose")
            {
                output += '\n' + "**Dealer wins.**";
                int newChips = ModifyChips(-1 * Bet);
                output += '\n' + string.Format("**You lost {0:n0} chips and now have {1:n0}.**", Bet, newChips);
            }
            else if (circumstance == "win")
            {
                output += '\n' + "**" + OwnerName + " wins!**";
                int newChips = ModifyChips((int)(Bet*WinMult));
                output += '\n' + string.Format("**You win {0:n0} chips ({1:n2}x) and now have {2:n0}.**", (int)(Bet * WinMult), WinMult, newChips);//this line needs to show correct won amount for multiplier
            }
            else if (circumstance == "push")
            {
                output += '\n' + "**Push.**";
                int newChips = ModifyChips(0);
                output += '\n' + string.Format("**You still have {0:n0} chips.**", newChips);
            }
            return output;
        }
        public List<int> GetSums(List<int> toShow)
        {
            List<int> sums = new List<int>();
            sums.Add(0);
            for (int i = 0; i < toShow.Count / 2; i++)
            {
                if (toShow[2 * i + 1] == 0)
                {
                    List<int> sums2 = new List<int>();
                    for (int x = 0; x < sums.Count; x++)
                    {
                        sums2.Add(sums[x] + 11);
                        sums2.Add(sums[x] + 1);
                    }
                    sums = sums2;
                }
                else if (toShow[2 * i + 1] == 100)
                {

                }
                else if (toShow[2 * i + 1] > 8)
                {
                    for (int x = 0; x < sums.Count; x++)
                    {
                        sums[x] = sums[x] + 10;
                    }
                }
                else
                {
                    for (int x = 0; x < sums.Count; x++)
                    {
                        sums[x] = sums[x] + toShow[2 * i + 1] + 1;
                    }
                }
            }
            sums.RemoveAll(item => item > 21);
            sums.Sort();
            sums.Reverse();
            sums=sums.Distinct().ToList();
            return sums;
        }
        public string DealerTurn()
        {
            List<int> sums = null;
            int trigger = 0;
            while (trigger == 0)
            {
                sums = GetSums(dealer);
                if (sums.Count != 0 )
                {
                    for (int x = 0; x < sums.Count; x++)
                    {
                        if (sums[x] > 16 && sums[x] < 22 && sums[x] > trigger)
                        {
                            trigger = sums[x];
                        }
                    }
                    if (trigger == 0)
                    {
                        AddCard(0);
                    }
                }
                else
                {
                    trigger = 1;
                }
            }
            int userScore = 0;
            sums = GetSums(hand);
            for (int i = 0; i < sums.Count; i++)
            {
                if (sums[i] > userScore && sums[i] < 22)
                {
                    userScore = sums[i];
                }
            }

            string output = "```";
            output += ShowHand(1);
            output += ShowHand(2);
            if (trigger > userScore)
            {
                output += End("lose");
                
            }
            else if (userScore > trigger)
            {
                output += End("win");
            }
            else
            {
                output += End("push");
            }
            output += '\n' + "```";
            return output;
        }
        public int ModifyChips(int changeAmount)
        {
            int newChips = Chips + changeAmount;
            if (newChips == 0)
            {
                newChips = 1;
            }
            if (changeAmount != 0)
            {
                MySqlConnection connection = null;
                try
                {
                    connection = new MySqlConnection(GlobalVars.ConnectionSTR);
                    connection.Open();

                    MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;
                    command.CommandText = "UPDATE users SET chips=" + newChips + " WHERE ID=" + OwnerID;
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            return newChips;
        }
        public void ClearCards()
        {
            cards = new int[,] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
            hand = new List<int>();
            dealer = new List<int>();
        }
    }
}