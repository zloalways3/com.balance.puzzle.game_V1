using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    private int cardFaceID;
    private int cardUniqueID;
    private bool isFaceUp;
    private bool isRotating;
    [SerializeField] private Image cardImage;
    [SerializeField] MemoryMatchController memoryMatchController;

    public int CardFaceID
    {
        set
        {
            cardFaceID = value;
            isFaceUp = true;
            UpdateCardFace();
        }
        get { return cardFaceID; }
    }

    public int CardUniqueID
    {
        set { cardUniqueID = value; }
        get { return cardUniqueID; }
    }

    private IEnumerator CardFadeOut()
    {
        float fadeRate = 1.0f / 2.5f;
        float fadeProgress = 0.0f;
        while (fadeProgress < 1.0f)
        {
            fadeProgress += Time.deltaTime * fadeRate;
            cardImage.color = Color.Lerp(cardImage.color, Color.clear, fadeProgress);

            yield return null;
        }
    }

    public void SetActive()
    {
        if (cardImage)
        {
            cardImage.color = Color.white;
        }
    }

    public void SetInactive()
    {
        StartCoroutine(CardFadeOut());
    }

    public void RestoreDefaultRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        isFaceUp = true;
    }

    public void OnCardClick()
    {
        if (isFaceUp || isRotating)
        {
            return;
        }
        if (!memoryMatchController.CanInteract())
        {
            return;
        }
        StartFlip();
        StartCoroutine(CardSelectionRoutine());
    }

    public void StartFlip()
    {
        isRotating = true;
        StartCoroutine(RotateCard90(transform, 0.25f, true));
    }

    private IEnumerator CardSelectionRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        memoryMatchController.HandleCardInteraction(cardFaceID, cardUniqueID);
    }

    private IEnumerator RotateCard90(Transform thisTransform, float duration, bool shouldUpdateFace)
    {
        Quaternion initialRotation = thisTransform.rotation;
        Quaternion finalRotation = thisTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float progressRate = 1.0f / duration;
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * progressRate;
            thisTransform.rotation = Quaternion.Slerp(initialRotation, finalRotation, progress);

            yield return null;
        }
        if (shouldUpdateFace)
        {
            isFaceUp = !isFaceUp;
            UpdateCardFace();
            StartCoroutine(RotateCard90(transform, duration, false));
        }
        else
            isRotating = false;
    }

    private void UpdateCardFace()
    {
        if (cardFaceID == -1 || cardImage == null)
        {
            return;
        }
        if (isFaceUp)
        {
            cardImage.sprite = memoryMatchController.RetrieveCardFace(cardFaceID);
        }
        else
        {
            cardImage.sprite = memoryMatchController.RetrieveCardBack();
        }
    }
}