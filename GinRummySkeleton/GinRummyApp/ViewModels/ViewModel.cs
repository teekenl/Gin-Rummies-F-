using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QUT
{
    class ViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Cards.Card> HumanCards { get; private set; }
        public ObservableCollection<Cards.Card> ComputerCards { get; private set; }
        public ObservableCollection<Cards.Card> Discards { get; private set; }
        public ObservableCollection<Cards.Card> RemainingDeck { get; private set; }

        public ObservableCollection<Cards.Card> PossibleDeck { get; private set; }
        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public ICommand ContinueCommand { get; set; }
        public ICommand KnockCommand { get; set; }
        public ICommand GinCommand { get; set; }
        public ICommand DiscardCardFromHandCommand { get; set; }
        public ICommand TakeCardFromDiscardPileCommand { get; set; }
        public ICommand TakeCardFromDeckCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool startGame = false;

        public bool humanDraw = false;

        public bool humanDiscard = false;

        public bool playerTurn = true;

        private int winingScore = 100;

        private int round = 0;

        private int firstWinnerScore = 0;

        private int secondWinnerScore = 0;

        public ViewModel()
        {
            TakeCardFromDiscardPileCommand = new DelegateCommand<Cards.Card>(TakeCardFromDiscardPile);
            DiscardCardFromHandCommand = new DelegateCommand<Cards.Card>(DiscardCardFromHand);
            TakeCardFromDeckCommand = new DelegateCommand<Cards.Card>(TakeCardFromDeck);

            ContinueCommand = new DelegateCommand(ContinueClick);
            KnockCommand = new DelegateCommand(KnockClick);
            GinCommand = new DelegateCommand(GinClick);
            NotificationRequest = new InteractionRequest<INotification>();

            HumanCards = new ObservableCollection<Cards.Card>();
            ComputerCards = new ObservableCollection<Cards.Card>();
            Discards = new ObservableCollection<Cards.Card>();
            RemainingDeck = new ObservableCollection<Cards.Card>();
            PossibleDeck = new ObservableCollection<Cards.Card>();

            HumanCards.CollectionChanged += HumanCards_CollectionChanged;
            ComputerCards.CollectionChanged += ComputerCards_CollectionChanged;
            Deal();
        }

        private async void Deal()
        {
            var deck = Cards.Shuffle(Cards.FullDeck);

            foreach (var card in deck)
            {
                PossibleDeck.Add(card); // Possible list of card
                RemainingDeck.Add(card);
                await Task.Delay(1);
            }

            for (int i = 0; i < 10; i++)
            {
                Cards.Card computerCard = DrawTopCardFromDeck();
                ComputerCards.Add(computerCard);
                // Remove the list of card on computer hand, as it is seen
                removePossibleCard(computerCard);
                await Task.Delay(30);
                HumanCards.Add(DrawTopCardFromDeck());
                await Task.Delay(30);
            }

            Discards.Add(DrawTopCardFromDeck());
            // Remove the discard from possible deck
            removePossibleCard(Discards[Discards.Count - 1]);
            ScoreToWin = "Score To Win : " + winingScore.ToString();
            scoreLabel(firstWinnerScore, secondWinnerScore);
        }

        private Cards.Card DrawTopCardFromDeck()
        { 
            var top = RemainingDeck[RemainingDeck.Count - 1];
            RemainingDeck.Remove(top);
            return top;
        }

        private void TakeCardFromDeck(Cards.Card card)
        {
            if (!humanDraw)
            {
                RemainingDeck.Remove(card);
                HumanCards.Add(card);
                humanDraw = true;
            }           
            else
            {
                RaiseNotification("You already take a card", "Invalid Action");
            }
        }

        private void TakeCardFromDiscardPile(Cards.Card p)
        {
            if (!humanDraw)
            {
                Discards.Remove(p);
                HumanCards.Add(p);
                humanDraw = true;
            }
            else
            {
                RaiseNotification("You already take a card", "Invalid Action");
            }
        }

        private void DiscardCardFromHand(Cards.Card p)
        {
            if (!humanDiscard && humanDraw)
            { 
                HumanCards.Remove(p);
                Discards.Add(p);
                removePossibleCard(p);
                humanDiscard = true;
                startGame = true;
                playerTurn = false;
            }
            else
            {
                if(!startGame)
                {
                    RaiseNotification("You must draw a card first", "Invalid Action");
                }
                else
                {
                    if (!playerTurn)
                    {
                        RaiseNotification("You already discard a card", "Invalid Action");
                    } 
                    else
                    {
                        RaiseNotification("You must draw a card first", "Invalid Action");

                    }
                } 
                
            }
        }

        private void ComputerDiscardCardFromHand()
        {
            if (humanDiscard)
            { 
                ComputerPlayer.Move move = ComputerPlayer.ComputerMove(ComputerCards).Item1;
                Cards.Card discard = ComputerPlayer.ComputerMove(ComputerCards).Item2.Value;
                Discards.Add(discard);
                ComputerCards.Remove(discard);
                humanDiscard = false;
                playerTurn = true;
          
                if (move == ComputerPlayer.Move.Gin)
                {
                    endGame();
                }
                else if (move == ComputerPlayer.Move.Knock)
                {
                    endGame();
                }
               
            }
        }

        private void ComputerTakeCardFromDiscardPile()
        {
            Cards.Card topDiscard = Discards[Discards.Count-1];
            if (humanDraw)
            {
                bool computerPickUp = ComputerPlayer.ComputerPickupDiscard(ComputerCards, topDiscard, PossibleDeck);
                if (computerPickUp)
                {
                    ComputerCards.Add(topDiscard);
                    Discards.Remove(topDiscard);
                    humanDraw = false;
                    ComputerDiscardCardFromHand();
                }
                else
                {
                    var topHiddenCard = RemainingDeck[RemainingDeck.Count - 1];
                    ComputerCards.Add(topHiddenCard);
                    PossibleDeck.Remove(topHiddenCard);
                    RemainingDeck.Remove(topHiddenCard);
                    humanDraw = false;
                    ComputerDiscardCardFromHand();
                }
            }
        }

        // Function to reset the game for next rounds
        // The game will continue if none of player has exceeded
        // maximum score to win
        private void resetGame()
        {
            Discards.Clear();
            HumanCards.Clear();
            ComputerCards.Clear();
            RemainingDeck.Clear();
            PossibleDeck.Clear();
            startGame = false;
            playerTurn = true;
            humanDraw = false;
            humanDiscard = false;

            if(firstWinnerScore>=winingScore || secondWinnerScore >= winingScore)
            {
                scoreReset();
            }
            Deal();
        }
        
        async private void HumanCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HumanDeadwood = "Calculating ...";
            // this might take a while, so let's do it in the background
            int deadwood = await Task.Run(() => GinRummy.Deadwood(HumanCards));
            HumanDeadwood = "Deadwood: " + deadwood;
        }

        async private void ComputerCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int deadwood = await Task.Run(() => GinRummy.Deadwood(ComputerCards));
        }

        private string humanDeadwood;
        public string HumanDeadwood 
        { 
            get
            {
                return humanDeadwood;
            }
            private set
            {
                humanDeadwood = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HumanDeadwood"));
            }
        }

        private string scoreToWin;
        public string ScoreToWin
        {
            get
            {
                return scoreToWin;
            }
            private set
            {
                scoreToWin = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ScoreToWin"));
            }
        }


        private string playerScore;
        public string PlayerScore
        {
            get
            {
                return playerScore;
            }
            private set
            {
                playerScore = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PlayerScore"));
            }
        }

        private string computerScore;
        public string ComputerScore
        {
            get
            {
                return computerScore;
            }
            private set
            {
                computerScore = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ComputerScore"));
            }
        }

        private void RaiseNotification(string msg, string title)
        {
            NotificationRequest.Raise(new Notification { Content = msg, Title = title });
        }

        //Function to end your turn, give a turn to computer if you
        // have more than 10 point
        private void ContinueClick()
        {
            
            if (humanDiscard)
            {
                ComputerTakeCardFromDiscardPile();       
            }
            else
            {
                RaiseNotification("You must throw a card!", "Invalid Action");   
            }
        }

        // Function to end the game when player can knock 
        // when the button is clicked 
        private void KnockClick()
        {
            int deadwoodScore = GinRummy.Deadwood(HumanCards);

            if(deadwoodScore<=10)
            {
                endGame();
            }
            else
            {
                RaiseNotification("You have not reached less than 10 points", "Invalid action");
            }
        }

        // Function to end the game when player,
        // when button is clicked
        private void GinClick()
        {
            int deadwoodScore = GinRummy.Deadwood(HumanCards);

            if (deadwoodScore == 10)
            {
                endGame();
            }
            else
            {
                RaiseNotification("You have not reached 0 points", "Invalid action");
            }

        }
        
        // Function to finish the different rounds of game,
        // The game will be ended if one of player has exceeded
        // maximum scrore to win.
        private void endGame()
        {
            int differenceScore = GinRummy.Score(HumanCards,ComputerCards);
            round += 1;
            if(differenceScore>0)
            {
                firstWinnerScore += differenceScore;
                RaiseNotification("You win at Round" + round, "Round" + round);
            } 
            else
            {
                secondWinnerScore += (differenceScore * -1);
                RaiseNotification("Computer wins at Round" + round, "Round" + round);
            }

            scoreLabel(firstWinnerScore, secondWinnerScore);

            if (firstWinnerScore >= winingScore)
            {
                RaiseNotification("Congratulations, You have win this game", "You wins");
                resetGame();
            }
            else if(secondWinnerScore >= winingScore)
            {
                RaiseNotification("Sorry, Computer Player wins this game", "Computer Wins");
                resetGame();
            }
            else
            {
                resetGame();
            }
            
        }

        // Change the the score of player and computer
        private void scoreLabel(int firstScore, int secondScore)
        {
            PlayerScore = "Player Score: " + firstScore.ToString();
            ComputerScore = "Computer Score: " + secondScore.ToString();
        }

        // Reset the score when one of player wins the game
        private void scoreReset()
        {
            firstWinnerScore = 0;
            secondWinnerScore = 0;
        }
        
        //Function to remove possible card after it is seen by computer
        private void removePossibleCard(Cards.Card p)
        {
            if(PossibleDeck.Contains(p))
            {
                PossibleDeck.Remove(p);
            } 
        }

    }
}