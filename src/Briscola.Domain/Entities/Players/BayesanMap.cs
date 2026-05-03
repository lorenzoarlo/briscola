using Briscola.Domain.Enums;

namespace Briscola.Domain.Entities.Players;


/// <summary>
/// A class that represents the Bayesian map of the game. 
/// It is used to keep track of the probabilities of the cards being in the opponents' hands
/// </summary>
public class BayesanCardMap
{
    // The probabilities of the cards being in the opponents' hands. The key is the card and the value is the probability (between 0 and 100)
    private readonly Dictionary<Card, float> _probabilities = [];

    // Cards that are known to *not* be in the opponents' hands. 
    private readonly HashSet<Card> _alreadySeenCards = [];

    // Cards that are not known to be in the opponents' hands. 
    // This is used to quickly get the list of unknown cards when we need to update the probabilities.
    private readonly HashSet<Card> _availableCards = [.. Deck.Create()];

    // The maximum number of cards that the opponents can have in their hands. 
    private int OpponentsHandSize => Math.Min(ImmutableHand.MAX_HAND_SIZE, _availableCards.Count);
    public BayesanCardMap()
    {
        const float InitialProbability = 1f / Deck.TotalCards;
        _probabilities = Deck.Create().ToDictionary(card => card, _ => InitialProbability);
    }


    private void AddSeenCard(Card card)
    {
        // If we already know that the card is not in the opponents' hands, we can skip it
        if (_alreadySeenCards.Contains(card))
        {
            return;
        }
        _availableCards.Remove(card);
        _alreadySeenCards.Add(card);
        // for sure not in the opponents' hands
        _probabilities[card] = 0f;


    }

    /// <summary>
    /// Registers one or more cards as being seen.
    /// </summary>
    public void RegisterSeenCards(params Card[] cards) => RegisterSeenCards((IEnumerable<Card>)cards);

    /// <summary>
    /// Registers a collection of cards as being seen.
    /// </summary>
    public void RegisterSeenCards(IEnumerable<Card> cards)
    {
        if (cards == null) return;

        foreach (var card in cards)
        {
            AddSeenCard(card);
        }

        NormalizeProbability();
    }


    // Registers a card as being in the opponents' hands with probability 1. 
    // This is used for example when the opponent's draw the briscola card.
    public void RegisterSureCard(Card card)
    {
        // Remove the card from the known cards, since we now know that it is in the opponents' hands
        _alreadySeenCards.Remove(card);
        _availableCards.Add(card);
        _probabilities[card] = 1f;

        // After updating the probability of a card, we need to normalize the probabilities
        NormalizeProbability();

    }

    /// <summary>
    /// Applies a heuristic penalty to the probabilities of the cards that satisfy the given condition. 
    /// This is used to reduce the probabilities of the cards that are less likely to be in the opponents' hands based on some heuristic 
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="penaltyFactor">Factor that will decrease the probability of each cards selected</param>
    public void ApplyHeuristicPenalty(Func<Card, bool> condition, float penaltyFactor)
    {
        foreach (var card in _availableCards.Where(condition))
        {
            _probabilities[card] *= penaltyFactor;
        }
        NormalizeProbability();
    }

    /// <summary>
    /// This method should be called after every update to the probabilities to ensure that they are normalized (i.e. they sum up to 1).
    /// </summary>
    private void NormalizeProbability()
    {
        // Get the unknown cards and their probabilities

        float currentSum = _availableCards.Sum(c => _probabilities[c]);
        if (currentSum == 0) return;

        // The hand is full, we can normalize the probabilities by dividing them by the sum of the probabilities. This ensures that they sum up to 1.

        float normalizationFactor = OpponentsHandSize / currentSum;

        foreach (var card in _availableCards)
        {
            _probabilities[card] *= normalizationFactor;
        }
    }

    /// <summary>
    /// Gets the probability of a card being in the opponents' hands
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float GetCardProbability(Card card) => _probabilities.TryGetValue(card, out var value) ? value : throw new ArgumentException($"Card {card} not found.");


    /// <summary>
    /// Returns a condition that can be used to select cards of a specific suit. 
    /// </summary>
    /// <param name="suit"></param>
    /// <returns></returns>
    public static Func<Card, bool> CardOfSuitCondition(Suit suit) => card => card.Suit == suit;

    /// <summary>
    /// Returns a condition that can be used to select cards that are not of a specific suit.
    /// </summary>
    /// <param name="suit"></param>
    /// <returns></returns>
    public static Func<Card, bool> CardNotOfSuitCondition(Suit suit) => card => card.Suit != suit;



}