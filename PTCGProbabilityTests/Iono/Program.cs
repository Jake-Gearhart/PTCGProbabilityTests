namespace Iono
{
    class Program
    {
        static void Main()
        {
            Console.Clear();

            Player player = new();
            player.FillDeck(basics: 12, bosses: 3);

            int iterations = 10000000;

            int extra_hand_cards = 0;
            int cards_in_deck = 32;
            
            int failed = 0;
            int boss_in_prizes = 0;
            int boss_in_hand = 0;
            int boss_in_deck_before_iono = 0;
            int boss_in_deck = 0;
            int boss_off_iono = 0;

            for (int i = 0; i < iterations; i++)
            {
                if (i % 50000 == 0) Console.WriteLine(i);

                player.SetUp();

                // Draw extra cards
                player.MoveRandom(CardLocation.Deck, CardLocation.Hand, extra_hand_cards);

                // Ensure hand doesn't have any Boss
                if (player.hand.Any(card => card.kind == CardKind.Boss)) {failed += 1; continue;}

                // Discard 16 random cards to make the deck size 33
                player.MoveRandom(CardLocation.Deck, CardLocation.Discard, 47 - cards_in_deck);

                // Ensure there is at least 1 Boss in the deck
                if (!player.deck.Any(card => card.kind == CardKind.Boss)) {failed += 1; continue;}

                // Luminous Sign for Boss, use Boss
                player.deck.FirstOrDefault(card => card.kind == CardKind.Boss).location = CardLocation.Discard;

                // Take 2 prize cards
                player.MoveRandom(CardLocation.Prize, CardLocation.Hand, 2);

                // Draw for turn
                player.MoveRandom(CardLocation.Deck, CardLocation.Hand, 1);

                // Ensure hand doesn't have any Boss
                if (player.hand.Any(card => card.kind == CardKind.Boss)) {failed += 1; continue;}

                // Ensure there is at least 1 Boss in the deck
                if (!player.deck.Any(card => card.kind == CardKind.Boss)) {failed += 1; continue;}

                // Luminous Sign for Boss, use Boss
                player.deck.FirstOrDefault(card => card.kind == CardKind.Boss).location = CardLocation.Discard;

                // Take 2 prize cards
                player.MoveRandom(CardLocation.Prize, CardLocation.Hand, 2);

                // Check if Boss is still prized
                if (player.prizes.Any(card => card.kind == CardKind.Boss)) {
                    boss_in_prizes += 1;
                    continue;
                }
                
                // Check if Boss is in hand now, or is the next top-deck
                player.MoveRandom(CardLocation.Deck, CardLocation.Looking, 1);
                
                if (player.hand.Any(card => card.kind == CardKind.Boss) || player.looking.Any(card => card.kind == CardKind.Boss)) {
                    boss_in_hand += 1;
                    continue;
                }

                boss_in_deck_before_iono += 1;

                // Look at the next 2 cards after the top-deck
                player.MoveRandom(CardLocation.Deck, CardLocation.Looking, 2);

                if (player.looking.Any(card => card.kind == CardKind.Boss)) {
                    boss_off_iono += 1;
                    continue;
                }

                boss_in_deck += 1;
            }

            Console.Clear();

            int successful_iterations = iterations - failed;

            Console.WriteLine($"Attempted simulations: {iterations}");
            Console.WriteLine($"Successful simulations: {iterations - failed}");
            Console.WriteLine($"Starting cards in hand: {7 + extra_hand_cards}");
            Console.WriteLine($"Starting cards in deck: {cards_in_deck}");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine($"Boss in prizes: {(float)100*boss_in_prizes/successful_iterations:F5}%");
            Console.WriteLine($"Boss in deck before Iono: {(float)100*boss_in_deck_before_iono/successful_iterations:F5}%");
            Console.WriteLine($"Boss in deck after Iono: {(float)100*boss_in_deck/successful_iterations:F5}%");
            Console.WriteLine($"Has boss unless Iono: {(float)100*boss_in_hand/successful_iterations:F5}%");
            Console.WriteLine($"Has Boss after Iono: {(float)100*boss_off_iono/successful_iterations:F5}%");
            Console.WriteLine("----------------------------------------------------------------");

            /*
                P_has_boss_with_iono
                    a) ---------
                        boss is the top-deck
                            boss is not in the hand
                            boss is not in the prizes
                    b) ---------
                        boss is in the 2 cards following the top-deck
                            boss is not in the hand
                            boss is not in the prizes

                P_has_boss_unless_iono
                    a) ---------
                        boss is the top-deck
                            boss is not in the hand
                            boss is not in the prizes
                    b) --------
                        boss is in the hand
                            boss is not in the prizes
                            boss is not in the deck

            */

            /*

                Piles A, B, C

                A = Inaccessible
                B = With
                C = Unless
            */
        }
    }

    class Player {
        private readonly Random rng = new();
        public readonly Card[] cards = new Card[60];

        public Card[] deck => cards.Where(card => card.location == CardLocation.Deck).ToArray();
        public Card[] hand => cards.Where(card => card.location == CardLocation.Hand).ToArray();
        public Card[] prizes => cards.Where(card => card.location == CardLocation.Prize).ToArray();
        public Card[] discard => cards.Where(card => card.location == CardLocation.Discard).ToArray();
        public Card[] looking => cards.Where(card => card.location == CardLocation.Looking).ToArray();

        public void FillDeck(int basics, int bosses)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (i < basics) cards[i] = new Card(CardKind.Basic);
                else if (i < basics + bosses) cards[i] = new Card(CardKind.Boss);
                else cards[i] = new Card(CardKind.Generic);
            }
        }

        public void MoveRandom(CardLocation startLocation, CardLocation newLocation, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Card[] targetCards = cards.Where(card => card.location == startLocation).ToArray();

                int index = rng.Next(0, targetCards.Length);
                targetCards[index].location = newLocation;
            }
        }

        public void MoveAll(CardLocation startLocation, CardLocation newLocation)
        {
            IEnumerable<Card> targetCards = cards.Where(card => card.location == startLocation);
            foreach (Card card in targetCards)
            {
                card.location = newLocation;
            }
        }

        private void ResetCardLocations()
        {
            foreach (Card card in cards)
            {
                card.location = CardLocation.Deck;
            }
        }

        public void SetUp()
        {
            do {
                ResetCardLocations();
                MoveRandom(CardLocation.Deck, CardLocation.Hand, 7);
            }
            while (!cards.Any(card => card.location == CardLocation.Hand && card.kind == CardKind.Basic));

            MoveRandom(CardLocation.Deck, CardLocation.Prize, 6);
        }

        public void Log(CardLocation location)
        {
            foreach (Card card in cards)
            {
                if (card.location == location) Console.WriteLine(card.kind);
            }
        }
    }

    enum CardKind
    {
        Generic,
        Basic,
        Boss
    }

    enum CardLocation
    {
        Deck,
        Hand,
        Prize,
        Discard,
        Looking
    }

    class Card(CardKind kind) {
        public readonly CardKind kind = kind;
        public CardLocation location = CardLocation.Deck;
    }
}