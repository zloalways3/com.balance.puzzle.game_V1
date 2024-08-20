using System.Collections;
using UnityEngine;

public class MemoryMatchController : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject cardHolder;
    [SerializeField] private Sprite defaultCardBack;
    [SerializeField] private Sprite[] cardFaces;
    [SerializeField] private GameObject gamePanel;

    [SerializeField] private MatchGameController matchController;
    public static int gridSize;

    private MemoryCard[] memoryCards;
    private int selectedCardFace;
    private int selectedCardIndex;
    private int remainingCards;
    private bool isGameInitialized;

    private Coroutine cardRevealCoroutine;

    void Start()
    {
        isGameInitialized = false;
    }

    public void InitializeGame()
    {
        if (isGameInitialized)
        {
            return;
        }
        isGameInitialized = true;
        gamePanel.SetActive(true);
        ConfigureGameBoard();
        selectedCardIndex = selectedCardFace = -1;
        remainingCards = memoryCards.Length;
        AssignCardFaces();
        cardRevealCoroutine = StartCoroutine(ConcealCards());
    }

    private void AssignCardFaces()
    {
        int i, j;
        int[] faceIndices = new int[memoryCards.Length / 2];

        for (i = 0; i < memoryCards.Length / 2; i++)
        {
            int randomIndex = Random.Range(0, cardFaces.Length - 1);
            for (j = i; j > 0; j--)
            {
                if (faceIndices[j - 1] == randomIndex)
                    randomIndex = (randomIndex + 1) % cardFaces.Length;
            }
            faceIndices[i] = randomIndex;
        }

        for (i = 0; i < memoryCards.Length; i++)
        {
            memoryCards[i].SetActive();
            memoryCards[i].CardFaceID = -1;
            memoryCards[i].RestoreDefaultRotation();
        }

        for (i = 0; i < memoryCards.Length / 2; i++)
        {
            for (j = 0; j < 2; j++)
            {
                int randomIndex = Random.Range(0, memoryCards.Length - 1);
                while (memoryCards[randomIndex].CardFaceID != -1)
                    randomIndex = (randomIndex + 1) % memoryCards.Length;

                memoryCards[randomIndex].CardFaceID = faceIndices[i];
            }
        }
    }

    public void ConfigureGameBoard()
    {
        int oddCountAdjustment = gridSize % 2;

        memoryCards = new MemoryCard[gridSize * gridSize - oddCountAdjustment];
        foreach (Transform child in cardHolder.transform)
        {
            Destroy(child.gameObject);
        }
        RectTransform panelDimensions = gamePanel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float width = panelDimensions.sizeDelta.x;
        float height = panelDimensions.sizeDelta.y;
        float scale = 1.0f / gridSize;
        float xStep = width / gridSize;
        float yStep = height / gridSize;
        float currentX = -xStep * (float)(gridSize / 2);
        float currentY = -yStep * (float)(gridSize / 2);

        if (oddCountAdjustment == 0)
        {
            currentX += xStep / 2;
            currentY += yStep / 2;
        }
        float startX = currentX;
        for (int i = 0; i < gridSize; i++)
        {
            currentX = startX;
            for (int j = 0; j < gridSize; j++)
            {
                GameObject card;
                if (oddCountAdjustment == 1 && i == (gridSize - 1) && j == (gridSize - 1))
                {
                    int index = gridSize / 2 * gridSize + gridSize / 2;
                    card = memoryCards[index].gameObject;
                }
                else
                {
                    card = Instantiate(cardPrefab);
                    card.transform.SetParent(cardHolder.transform);

                    int index = i * gridSize + j;
                    memoryCards[index] = card.GetComponent<MemoryCard>();
                    memoryCards[index].CardUniqueID = index;
                    card.transform.localScale = new Vector3(scale, scale) * 3;
                }
                card.transform.localPosition = new Vector3(currentX, currentY, 0);
                currentX += xStep / 1.3f;
            }
            currentY += yStep * 1.2f;
        }
    }

    IEnumerator ConcealCards()
    {
        yield return new WaitForSeconds(3f);
        foreach (MemoryCard card in memoryCards)
        {
            card.StartFlip();
        }
        yield return new WaitForSeconds(0.5f);
    }

    public void HandleCardInteraction(int faceId, int cardIndex)
    {
        if (selectedCardFace == -1)
        {
            selectedCardFace = faceId;
            selectedCardIndex = cardIndex;
        }
        else
        {
            if (selectedCardFace == faceId)
            {
                matchController.AwardPoints();
                memoryCards[selectedCardIndex].SetInactive();
                memoryCards[cardIndex].SetInactive();
                remainingCards -= 2;
                VerifyGameCompletion();
            }
            else
            {
                memoryCards[selectedCardIndex].StartFlip();
                memoryCards[cardIndex].StartFlip();
            }
            selectedCardIndex = selectedCardFace = -1;
        }
    }

    private void VerifyGameCompletion()
    {
        if (remainingCards == 0)
        {
            ConcludeGame();
            matchController.ShowVictoryScreen();
        }
    }

    public void ConcludeGame()
    {
        StopCoroutine(cardRevealCoroutine);
        isGameInitialized = false;
        gamePanel.SetActive(false);
    }

    public void DefineGridSize(int size)
    {
        gridSize = size;
    }

    public Sprite RetrieveCardFace(int faceId)
    {
        return cardFaces[faceId];
    }

    public Sprite RetrieveCardBack()
    {
        return defaultCardBack;
    }

    public bool CanInteract()
    {
        return isGameInitialized;
    }
}