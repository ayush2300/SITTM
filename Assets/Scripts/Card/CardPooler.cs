using System.Collections.Generic;
using UnityEngine;

public class CardPooler : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject cardPrefab;
    public int initialPoolSize = 5;

    private readonly Queue<GameObject> cardPool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            card.SetActive(false);
            cardPool.Enqueue(card);
        }
    }

    public GameObject GetCard(Transform parent)
    {
        GameObject card;

        if (cardPool.Count > 0)
        {
            card = cardPool.Dequeue();
        }
        else
        {
            // Expand pool if needed
            card = Instantiate(cardPrefab);
        }

        card.transform.SetParent(parent, false);
        card.SetActive(true);
        return card;
    }

    public void ReturnCard(GameObject card)
    {
        card.SetActive(false);
        card.transform.SetParent(transform, false);
        cardPool.Enqueue(card);
    }
}
