Console.Clear();

Player player1 = new();

player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "boss"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinnable"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinner"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinnable"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinner"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinnable"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinner"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinnable"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinner"));
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "thinnable"));

int iterations = 10000000;
int fez_first_successes = 0;
int ogerpon_first_successes = 0;

for (int i = 0; i < iterations; i++)
{
    if (i % 50000 == 0) Console.WriteLine(i);

    //ogerpon first

    player1.ResetCardLocations();

    player1.Draw(1);

    while (
        player1.Hand.Any(c => c.category == "thinner")
    ) {
        player1.Hand.First(c => c.category == "thinner").location = Card.Location.Discard;
        if (player1.Deck.FirstOrDefault(c => c.category == "thinnable") is Card thinnableCard) thinnableCard.location = Card.Location.Discard;
        if (player1.Deck.FirstOrDefault(c => c.category == "thinnable") is Card thinnableCard2) thinnableCard2.location = Card.Location.Discard;
    }

    player1.Draw(3);

    if (player1.Hand.Any(c => c.category == "boss")) ogerpon_first_successes += 1;

    //fez first

    player1.ResetCardLocations();

    player1.Draw(1);

    while (
        player1.Hand.Any(c => c.category == "thinner")
    ) {
        player1.Hand.First(c => c.category == "thinner").location = Card.Location.Discard;
        if (player1.Deck.FirstOrDefault(c => c.category == "thinnable") is Card thinnableCard) thinnableCard.location = Card.Location.Discard;
        if (player1.Deck.FirstOrDefault(c => c.category == "thinnable") is Card thinnableCard2) thinnableCard2.location = Card.Location.Discard;
    }

    player1.Draw(3);

    if (player1.Hand.Any(c => c.category == "boss")) fez_first_successes += 1;
}

Console.WriteLine("Ogerpon first:");
Console.WriteLine((float)ogerpon_first_successes / iterations);
Console.WriteLine("Fez first:");
Console.WriteLine((float)fez_first_successes / iterations);

class Card(Card.Location location, Card.Stage kind, string category = "") {
    public enum Location
    {
        Deck,
        Hand,
        Prize,
        Discard
    }

    public enum Stage
    {
        Basic,
        Other
    }

    public Location location = location;
    public readonly Card.Stage kind = kind;
    public readonly string category = category;
}

class Player {
    readonly Random RNG = new();
    Card[] Cards = [];

    public Card[] Deck {
        get {
            return Cards.Where(card => card.location == Card.Location.Deck).ToArray();
        }
    }

    public Card[] Hand {
        get {
            return Cards.Where(card => card.location == Card.Location.Hand).ToArray();
        }
    }

    public Card[] Prize {
        get {
            return Cards.Where(card => card.location == Card.Location.Prize).ToArray();
        }
    }

    public void GiveCard(Card card)
    {
        Array.Resize(ref Cards, Cards.Length + 1); // Increase size by 1
        Cards[^1] = card; // Add the new card to the last position
    }

    public void Move(Card.Location startLocation, Card.Location newLocation, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Card[] targetCards = Cards.Where(card => card.location == startLocation).ToArray();

            int index = RNG.Next(0, targetCards.Length);
            targetCards[index].location = newLocation;
        }
    }

    public void Draw(int amount)
    {
        Move(Card.Location.Deck, Card.Location.Hand, amount);
    }

    public void ResetCardLocations()
    {
        foreach (Card card in Cards)
        {
            card.location = Card.Location.Deck;
        }
    }

    public void SetUp()
    {
        do {
            ResetCardLocations();
            Draw(7);
        }
        while (!Hand.Any(card => card.kind == Card.Stage.Basic));

        Move(Card.Location.Deck, Card.Location.Prize, 6);
    }

    public void Log(Card.Location location)
    {
        foreach (Card card in Cards)
        {
            if (card != null && card.location == location)
            {
                Console.WriteLine($"{card.kind}, {card.category}");
            }
        }
    }
}
