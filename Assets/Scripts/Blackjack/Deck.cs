using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
    [SerializeField] private List<GameObject> cardPrefabs;
    private Queue<Card> cards = new Queue<Card>();
    private const int NEW_SHOE_THRESHOLD = 15;

    public void GenerateDeck() {
        cards.Clear();
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
    }

    public Card DrawCard() {
        return cards.Count > 0 ? cards.Dequeue() : null;
    }

    public Card BurnCard() {
        return cards.Dequeue();
    }

    public bool NeedsNewShoe() {
        return cards.Count < NEW_SHOE_THRESHOLD;
    }

}
