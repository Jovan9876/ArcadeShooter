using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
    [SerializeField] private List<GameObject> cardPrefabs;
    private Queue<Card> cards = new Queue<Card>();
    private const int NEW_SHOE_THRESHOLD = 15;

    public void GenerateDeck() {
        cards.Clear();

        List<Transform> deckChildren = new List<Transform>();
        foreach (Transform child in transform) {
            deckChildren.Add(child);
        }
        foreach (Transform child in deckChildren) {
            Destroy(child.gameObject);
        }

        foreach (GameObject cardPrefab in cardPrefabs) {
            GameObject cardObject = Instantiate(cardPrefab, transform);
            Card card = cardObject.GetComponent<Card>();
            cards.Enqueue(card);
        }
    }

    public void Shuffle() {
        List<Card> cardList = new List<Card>(cards);
        for (int i = 0; i < cardList.Count; ++i) {
            Card temp = cardList[i];
            int randomIndex = Random.Range(i, cardList.Count);
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }
        cards = new Queue<Card>(cardList);

        // Reposition the cards so that the first card in the queue is at (0,0,0)
        // and each subsequent card is stacked above it.
        for (int i = 0; i < cardList.Count; i++) {
            // The first card in cardList (cardList[0]) ends up at (0,0,0)
            float offsetY = 0.009f * i;
            cardList[i].transform.localPosition = new Vector3(0, offsetY, 0);
        }

    }

    public Card DrawCard() {
        if (cards.Count > 0) {
            Card drawnCard = cards.Dequeue();
            UpdateDeckPositions();
            return drawnCard;
        }
        return null;
    }

    private void UpdateDeckPositions() {
        // Convert the remaining cards in the queue to an array.
        Card[] remainingCards = cards.ToArray();
        // Iterate over the array so that the first card is at (0, 0, 0) and the others are stacked on top.
        for (int i = 0; i < remainingCards.Length; i++) {
            float offsetY = 0.009f * i;
            remainingCards[i].transform.localPosition = new Vector3(0, offsetY, 0);
        }
    }

    public Card BurnCard() {
        if (cards.Count > 0) {
            Card burnedCard = cards.Dequeue();
            UpdateDeckPositions();
            return burnedCard;
        }
        return null;
    }

    public bool NeedsNewShoe() {
        return cards.Count < NEW_SHOE_THRESHOLD;
    }

}
