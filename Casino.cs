using System;
using System.Collections.Generic;


class Player
{
    public string Name;
    public double Money;

    public int GamesPlayed;
    public int GamesWon;
    public int GamesLost;
    public int GamesTied;

    public double TotalWagered;
    public double TotalWon;
    public double TotalLost;

    public int CurrentStreak;
    public int BestStreak;

    public bool LuckyCharm;
    public bool VIPCard;

    public Player(string name)
    {
        Name = name;
        Money = 500;

        GamesPlayed = 0;
        GamesWon = 0;
        GamesLost = 0;
        GamesTied = 0;

        TotalWagered = 0;
        TotalWon = 0;
        TotalLost = 0;

        CurrentStreak = 0;
        BestStreak = 0;

        LuckyCharm = false;
        VIPCard = false;
    }
}


class Program
{
    static Random random = new Random();
    static Player player;


    static void Main()
    {
        Console.WriteLine("================================");
        Console.WriteLine("       WELCOME TO THE CASINO");
        Console.WriteLine("================================");

        Console.Write("Enter your name: ");
        string name = Console.ReadLine();

        if (name == "")
        {
            name = "Player";
        }

        player = new Player(name);

        Console.WriteLine();
        Console.WriteLine("Welcome, " + player.Name + "!");
        Console.WriteLine("You start with $500.");
        Console.WriteLine("Good luck!");
        Console.WriteLine();

        MainMenu();

        Console.WriteLine();
        Console.WriteLine("Thanks for playing, " + player.Name + "!");
        Console.WriteLine("You left the casino with $" + player.Money.ToString("F2"));
    }



    static void MainMenu()
    {
        bool playing = true;

        while (playing)
        {
            Console.WriteLine();
            Console.WriteLine("================================");
            Console.WriteLine("          CASINO MENU");
            Console.WriteLine("================================");
            Console.WriteLine("Balance: $" + player.Money.ToString("F2"));
            Console.WriteLine();
            Console.WriteLine("1. Blackjack");
            Console.WriteLine("2. Slots");
            Console.WriteLine("3. Roulette");
            Console.WriteLine("4. View Stats");
            Console.WriteLine("5. Casino Shop");
            Console.WriteLine("6. Exit");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Blackjack();
                    break;

                case "2":
                    Slots();
                    break;

                case "3":
                    Roulette();
                    break;

                case "4":
                    ShowStats();
                    break;

                case "5":
                    Shop();
                    break;

                case "6":
                    playing = false;
                    break;

                default:
                    Console.WriteLine("That isn't an option.");
                    break;
            }

            // If the player has no money left, the casino is over.
            if (player.Money <= 0)
            {
                Console.WriteLine();
                Console.WriteLine("You ran out of money!");
                Console.WriteLine("Maybe gambling wasn't your thing...");
                playing = false;
            }
        }
    }



    static double GetBet()
    {
        while (true)
        {
            Console.Write("Enter your bet: $");
            string input = Console.ReadLine();

            double bet;

            if (double.TryParse(input, out bet))
            {
                if (bet > 0 && bet <= player.Money)
                {
                    return bet;
                }
            }

            Console.WriteLine("That isn't a valid bet.");
            Console.WriteLine("You have $" + player.Money.ToString("F2"));
        }
    }



    static void Blackjack()
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("           BLACKJACK");
        Console.WriteLine("================================");

        double bet = GetBet();

        player.Money -= bet;
        player.TotalWagered += bet;

        List<int> playerCards = new List<int>();
        List<int> dealerCards = new List<int>();

        // Give the player and dealer their first two cards
        playerCards.Add(GetCard());
        playerCards.Add(GetCard());

        dealerCards.Add(GetCard());
        dealerCards.Add(GetCard());

        Console.WriteLine();
        Console.WriteLine("Your cards:");
        ShowCards(playerCards);

        Console.WriteLine();
        Console.WriteLine("Dealer's cards:");
        Console.WriteLine(dealerCards[0] + " and a hidden card");

        int playerScore = GetHandValue(playerCards);

        // Check for a blackjack right away
        if (playerScore == 21)
        {
            Console.WriteLine();
            Console.WriteLine("BLACKJACK!");

            double winnings = bet * 2.5;

 
            if (player.LuckyCharm)
            {
                winnings *= 1.10;
            }

            player.Money += winnings;
            player.TotalWon += winnings - bet;

            if (player.VIPCard)
            {
                player.Money += 25;
                Console.WriteLine("Your VIP Card gave you an extra $25!");
            }

            player.GamesPlayed++;
            player.GamesWon++;
            player.CurrentStreak++;

            if (player.CurrentStreak > player.BestStreak)
            {
                player.BestStreak = player.CurrentStreak;
            }

            Console.WriteLine("You won $" + (winnings - bet).ToString("F2") + "!");
            RandomEvent();
            return;
        }

        bool playerTurn = true;
        bool doubled = false;

        while (playerTurn)
        {
            playerScore = GetHandValue(playerCards);

            Console.WriteLine();
            Console.WriteLine("Your score: " + playerScore);
            Console.WriteLine("1. Hit");
            Console.WriteLine("2. Stand");

            if (playerCards.Count == 2 && player.Money >= bet)
            {
                Console.WriteLine("3. Double Down");
            }

            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                int newCard = GetCard();
                playerCards.Add(newCard);

                Console.WriteLine("You drew: " + newCard);
                ShowCards(playerCards);

                playerScore = GetHandValue(playerCards);

                if (playerScore > 21)
                {
                    Console.WriteLine();
                    Console.WriteLine("You busted with " + playerScore + "!");
                    playerTurn = false;
                }
            }
            else if (choice == "2")
            {
                playerTurn = false;
            }
            else if (choice == "3" && playerCards.Count == 2 && player.Money >= bet)
            {
                player.Money -= bet;
                player.TotalWagered += bet;

                bet *= 2;
                doubled = true;

                int newCard = GetCard();
                playerCards.Add(newCard);

                Console.WriteLine("You doubled your bet!");
                Console.WriteLine("You drew: " + newCard);

                ShowCards(playerCards);

                playerScore = GetHandValue(playerCards);

                if (playerScore > 21)
                {
                    Console.WriteLine("You busted!");
                }

                playerTurn = false;
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        playerScore = GetHandValue(playerCards);

        if (playerScore > 21)
        {
            player.GamesPlayed++;
            player.GamesLost++;
            player.TotalLost += bet;
            player.CurrentStreak = 0;

            Console.WriteLine("You lost $" + bet.ToString("F2"));
            RandomEvent();
            return;
        }


        Console.WriteLine();
        Console.WriteLine("Dealer's cards:");
        ShowCards(dealerCards);

        int dealerScore = GetHandValue(dealerCards);

        while (dealerScore < 17)
        {
            int newCard = GetCard();
            dealerCards.Add(newCard);

            dealerScore = GetHandValue(dealerCards);

            Console.WriteLine("Dealer drew: " + newCard);
        }

        Console.WriteLine();
        Console.WriteLine("Your score: " + playerScore);
        Console.WriteLine("Dealer score: " + dealerScore);

        if (dealerScore > 21)
        {
            Console.WriteLine("Dealer busted! You win!");

            GiveWin(bet);
        }
        else if (playerScore > dealerScore)
        {
            Console.WriteLine("You win!");

            GiveWin(bet);
        }
        else if (playerScore < dealerScore)
        {
            Console.WriteLine("Dealer wins.");

            GiveLoss(bet);
        }
        else
        {
            Console.WriteLine("It's a tie!");

            player.Money += bet;

            player.GamesPlayed++;
            player.GamesTied++;
            player.CurrentStreak = 0;
        }

        RandomEvent();
    }



    static int GetCard()
    {
        int card = random.Next(1, 14);

        if (card >= 10)
        {
            return 10;
        }

        if (card == 1)
        {
            return 11;
        }

        return card;
    }



    static int GetHandValue(List<int> cards)
    {
        int total = 0;
        int aces = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            total += cards[i];

            if (cards[i] == 11)
            {
                aces++;
            }
        }

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return total;
    }



    static void ShowCards(List<int> cards)
    {
        Console.Write("Cards: ");

        for (int i = 0; i < cards.Count; i++)
        {
            Console.Write(cards[i] + " ");
        }

        Console.WriteLine();
        Console.WriteLine("Total: " + GetHandValue(cards));
    }



    static void Slots()
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("             SLOTS");
        Console.WriteLine("================================");

        double bet = GetBet();

        player.Money -= bet;
        player.TotalWagered += bet;

        string[] symbols =
        {
            "Cherry",
            "Lemon",
            "Bell",
            "Star",
            "Seven"
        };

        string slot1 = symbols[random.Next(symbols.Length)];
        string slot2 = symbols[random.Next(symbols.Length)];
        string slot3 = symbols[random.Next(symbols.Length)];

        Console.WriteLine();
        Console.WriteLine("Spinning...");
        Console.WriteLine();

        Console.WriteLine("| " + slot1 + " | " + slot2 + " | " + slot3 + " |");

        double winnings = 0;

        // Three matching symbols is the biggest payout
        if (slot1 == slot2 && slot2 == slot3)
        {
            winnings = bet * 5;

            if (slot1 == "Seven")
            {
                winnings = bet * 10;
                Console.WriteLine("JACKPOT!!!");
            }
            else
            {
                Console.WriteLine("THREE MATCHING SYMBOLS!");
            }
        }
        
        else if (slot1 == slot2 || slot1 == slot3 || slot2 == slot3)
        {
            winnings = bet * 2;

            Console.WriteLine("Two matching symbols!");
        }
        else
        {
            Console.WriteLine("No match.");
        }

        if (winnings > 0)
        {
            
            if (player.LuckyCharm)
            {
                winnings *= 1.10;
                Console.WriteLine("Your Lucky Charm increased your winnings!");
            }

            player.Money += winnings;
            player.TotalWon += winnings - bet;

            player.GamesPlayed++;
            player.GamesWon++;
            player.CurrentStreak++;

            if (player.CurrentStreak > player.BestStreak)
            {
                player.BestStreak = player.CurrentStreak;
            }

            if (player.VIPCard)
            {
                player.Money += 25;
                Console.WriteLine("Your VIP Card gave you an extra $25!");
            }

            Console.WriteLine("You won $" + (winnings - bet).ToString("F2") + "!");
        }
        else
        {
            player.GamesPlayed++;
            player.GamesLost++;
            player.TotalLost += bet;
            player.CurrentStreak = 0;

            Console.WriteLine("You lost $" + bet.ToString("F2"));
        }

        RandomEvent();
    }


    
    static void Roulette()
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("            ROULETTE");
        Console.WriteLine("================================");

        double bet = GetBet();

        player.Money -= bet;
        player.TotalWagered += bet;

        Console.WriteLine();
        Console.WriteLine("Choose your bet:");
        Console.WriteLine("1. Red");
        Console.WriteLine("2. Black");
        Console.WriteLine("3. Odd");
        Console.WriteLine("4. Even");
        Console.WriteLine("5. Pick a Number (0-36)");

        Console.Write("Choose: ");
        string choice = Console.ReadLine();

        int pickedNumber = -1;

       
        if (choice == "5")
        {
            Console.Write("Pick a number from 0-36: ");
            string numberInput = Console.ReadLine();

            if (!int.TryParse(numberInput, out pickedNumber) ||
                pickedNumber < 0 || pickedNumber > 36)
            {
                Console.WriteLine("Invalid number.");
                player.Money += bet;
                return;
            }
        }

        int result = random.Next(0, 37);
        string color = GetRouletteColor(result);

        Console.WriteLine();
        Console.WriteLine("The wheel is spinning...");
        Console.WriteLine();
        Console.WriteLine("The number is: " + result);
        Console.WriteLine("Color: " + color);

        bool won = false;
        double winnings = 0;

        if (choice == "1" && color == "Red")
        {
            won = true;
            winnings = bet * 2;
        }
        else if (choice == "2" && color == "Black")
        {
            won = true;
            winnings = bet * 2;
        }
        else if (choice == "3" && result != 0 && result % 2 == 1)
        {
            won = true;
            winnings = bet * 2;
        }
        else if (choice == "4" && result != 0 && result % 2 == 0)
        {
            won = true;
            winnings = bet * 2;
        }
        else if (choice == "5" && result == pickedNumber)
        {
            won = true;
            winnings = bet * 36;
        }

        if (won)
        {
            if (player.LuckyCharm)
            {
                winnings *= 1.10;
                Console.WriteLine("Your Lucky Charm increased your winnings!");
            }

            player.Money += winnings;
            player.TotalWon += winnings - bet;

            player.GamesPlayed++;
            player.GamesWon++;
            player.CurrentStreak++;

            if (player.CurrentStreak > player.BestStreak)
            {
                player.BestStreak = player.CurrentStreak;
            }

            if (player.VIPCard)
            {
                player.Money += 25;
                Console.WriteLine("Your VIP Card gave you an extra $25!");
            }

            Console.WriteLine("You won $" + (winnings - bet).ToString("F2") + "!");
        }
        else
        {
            player.GamesPlayed++;
            player.GamesLost++;
            player.TotalLost += bet;
            player.CurrentStreak = 0;

            Console.WriteLine("You lost $" + bet.ToString("F2"));
        }

        RandomEvent();
    }


   
    static string GetRouletteColor(int number)
    {
        if (number == 0)
        {
            return "Green";
        }

        int[] redNumbers =
        {
            1, 3, 5, 7, 9,
            12, 14, 16, 18,
            19, 21, 23, 25, 27,
            30, 32, 34, 36
        };

        for (int i = 0; i < redNumbers.Length; i++)
        {
            if (number == redNumbers[i])
            {
                return "Red";
            }
        }

        return "Black";
    }


    
    static void GiveWin(double bet)
    {
        double winnings = bet * 2;

        if (player.LuckyCharm)
        {
            winnings *= 1.10;
            Console.WriteLine("Your Lucky Charm increased your winnings!");
        }

        player.Money += winnings;
        player.TotalWon += winnings - bet;

        player.GamesPlayed++;
        player.GamesWon++;

        player.CurrentStreak++;

        if (player.CurrentStreak > player.BestStreak)
        {
            player.BestStreak = player.CurrentStreak;
        }

        if (player.VIPCard)
        {
            player.Money += 25;
            Console.WriteLine("Your VIP Card gave you an extra $25!");
        }

        Console.WriteLine("You won $" + (winnings - bet).ToString("F2") + "!");
    }


    
    static void GiveLoss(double bet)
    {
        player.GamesPlayed++;
        player.GamesLost++;
        player.TotalLost += bet;

        player.CurrentStreak = 0;

        Console.WriteLine("You lost $" + bet.ToString("F2"));
    }


    
    static void ShowStats()
    {
        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("           YOUR STATS");
        Console.WriteLine("================================");

        Console.WriteLine("Player: " + player.Name);
        Console.WriteLine("Balance: $" + player.Money.ToString("F2"));
        Console.WriteLine();

        Console.WriteLine("Games Played: " + player.GamesPlayed);
        Console.WriteLine("Games Won: " + player.GamesWon);
        Console.WriteLine("Games Lost: " + player.GamesLost);
        Console.WriteLine("Games Tied: " + player.GamesTied);

        Console.WriteLine();

        Console.WriteLine("Total Wagered: $" + player.TotalWagered.ToString("F2"));
        Console.WriteLine("Total Won: $" + player.TotalWon.ToString("F2"));
        Console.WriteLine("Total Lost: $" + player.TotalLost.ToString("F2"));

        Console.WriteLine();

        if (player.GamesPlayed > 0)
        {
            double winRate =
                ((double)player.GamesWon / player.GamesPlayed) * 100;

            Console.WriteLine("Win Rate: " + winRate.ToString("F1") + "%");
        }
        else
        {
            Console.WriteLine("Win Rate: 0%");
        }

        Console.WriteLine("Current Win Streak: " + player.CurrentStreak);
        Console.WriteLine("Best Win Streak: " + player.BestStreak);

        Console.WriteLine();

        Console.WriteLine("Shop Items:");

        if (player.LuckyCharm)
        {
            Console.WriteLine("- Lucky Charm");
        }
        else
        {
            Console.WriteLine("- No Lucky Charm");
        }

        if (player.VIPCard)
        {
            Console.WriteLine("- VIP Card");
        }
        else
        {
            Console.WriteLine("- No VIP Card");
        }
    }


    
    static void Shop()
    {
        bool shopping = true;

        while (shopping)
        {
            Console.WriteLine();
            Console.WriteLine("================================");
            Console.WriteLine("          CASINO SHOP");
            Console.WriteLine("================================");
            Console.WriteLine("Your money: $" + player.Money.ToString("F2"));
            Console.WriteLine();

            Console.WriteLine("1. Lucky Charm - $200");
            Console.WriteLine("   Gives you 10% more winnings.");
            Console.WriteLine();

            Console.WriteLine("2. VIP Card - $300");
            Console.WriteLine("   Gives you $25 every time you win.");
            Console.WriteLine();

            Console.WriteLine("3. Leave Shop");
            Console.WriteLine();

            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    BuyLuckyCharm();
                    break;

                case "2":
                    BuyVIPCard();
                    break;

                case "3":
                    shopping = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }


    
    static void BuyLuckyCharm()
    {
        if (player.LuckyCharm)
        {
            Console.WriteLine("You already have the Lucky Charm.");
            return;
        }

        if (player.Money < 200)
        {
            Console.WriteLine("You don't have enough money.");
            return;
        }

        player.Money -= 200;
        player.LuckyCharm = true;

        Console.WriteLine("You bought the Lucky Charm!");
        Console.WriteLine("You now get 10% more winnings.");
    }


    
    static void BuyVIPCard()
    {
        if (player.VIPCard)
        {
            Console.WriteLine("You already have the VIP Card.");
            return;
        }

        if (player.Money < 300)
        {
            Console.WriteLine("You don't have enough money.");
            return;
        }

        player.Money -= 300;
        player.VIPCard = true;

        Console.WriteLine("You bought the VIP Card!");
        Console.WriteLine("You now get $25 every time you win.");
    }


   
    static void RandomEvent()
    {
        int chance = random.Next(1, 101);

        if (chance <= 15)
        {
            int bonus = random.Next(20, 76);

            player.Money += bonus;

            Console.WriteLine();
            Console.WriteLine("!!! RANDOM CASINO EVENT !!!");
            Console.WriteLine("The casino gave you a $" + bonus + " bonus!");
            Console.WriteLine("New balance: $" + player.Money.ToString("F2"));
        }
    }
}
