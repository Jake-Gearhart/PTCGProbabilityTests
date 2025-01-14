Console.Clear();

Player player1 = new();

player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic, "rotom")); //rotom
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic, "rotom")); //rotom
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic, "rotom")); //rotom
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic, "rotom")); //rotom
// player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "rotom-out")); //nest ball
// player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "rotom-out")); //nest ball
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "rotom-out")); //nest ball
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "rotom-out")); //ultra ball
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "trolley")); //trolley
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest")); //seal stone
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest")); //seal stone
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest")); //seal stone
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest")); //seal stone
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest-out")); //town store
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest-out")); //town store
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest-out")); //town store
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Other, "forest-out")); //town store

player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //dreepy
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //dreepy
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //dreepy
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //dreepy
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //fez
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //manaphy
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //radzard
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //munki
player1.GiveCard(new Card (Card.Location.Deck, Card.Stage.Basic)); //munki

player1.FillDeckWithGenerics();

int iterations = 1000000;
int successes = 0;

for (int i = 0; i < iterations; i++)
{
    if (i % 50000 == 0) Console.WriteLine(i);

    player1.SetUp();
    player1.Draw(1);

    if (player1.Prize.Any(card => card.category == "trolley")) continue;

    if (player1.Hand.Any(card => card.category == "trolley")) {
        successes += 1;
        continue;
    }

    bool hasRotom = false;

    if (player1.Hand.Any(card => card.category == "rotom")) hasRotom = true;
    else if (
        player1.Hand.Any(card => card.category == "rotom-out") &&
        player1.Deck.Any(card => card.category == "rotom") 
    ) hasRotom = true;

    if (hasRotom == false) continue;

    if (player1.Hand.Any(card => card.category == "forest")) successes += 1;
    else if (
        player1.Hand.Any(card => card.category == "forest-out") &&
        player1.Deck.Any(card => card.category == "forest") 
    ) successes += 1;
}

Console.WriteLine((float)successes / iterations);

class Card(Card.Location location, Card.Stage kind, string category = "") {
    public enum Location
    {
        Deck,
        Hand,
        Prize
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
    readonly Card[] Cards = new Card[60];

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
        int index = Array.FindIndex(Cards, element => element == null);
        Cards[index] = card;
    }

    public void FillDeckWithGenerics()
    {
        while (Cards.Any(element => element == null))
        {
            GiveCard(new Card(Card.Location.Deck, Card.Stage.Other));
        }
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