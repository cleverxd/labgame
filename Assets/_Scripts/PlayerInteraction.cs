using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Tuning")]
    public float interactionRange = 2f;
    public TMP_Text yellowCards;
    public TMP_Text redCards;
    [HideInInspector] public int cardsCollectedYellow = 0;
    [HideInInspector] public int cardsCollectedRed = 0;

    [Header("Interaction audio")]
    public AudioSource audioSource;
    public AudioClip activateKeycardAllow;
    public AudioClip activateKeycardDeny;
    public AudioClip pickupKeycardSound;

    private void Start()
    {
        mainCamera = Camera.main;
        UpdateCardsUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            HandleKeycardInteraction(hit);
        }
    }

    private void HandleKeycardInteraction(RaycastHit hit)
    {
        if (hit.collider.CompareTag("KeyPlacerYellow"))
        {
            KeyPlacer keyPlacer = hit.collider.GetComponent<KeyPlacer>();
            if (cardsCollectedYellow > 0 && !keyPlacer.isUnlocked)
            {
                cardsCollectedYellow--;
                audioSource.PlayOneShot(activateKeycardAllow);
                UpdateCardsUI();
                keyPlacer.UnlockPlacer();
            }
            else if (!keyPlacer.isUnlocked)
            {
                audioSource.PlayOneShot(activateKeycardDeny);
            }
        }
        else if (hit.collider.CompareTag("KeyPlacerRed"))
        {
            KeyPlacer keyPlacer = hit.collider.GetComponent<KeyPlacer>();
            if (cardsCollectedRed > 0 && !keyPlacer.isUnlocked)
            {
                cardsCollectedRed--;
                audioSource.PlayOneShot(activateKeycardAllow);
                UpdateCardsUI();
                keyPlacer.UnlockPlacer();
            }
            else if (!keyPlacer.isUnlocked)
            {
                audioSource.PlayOneShot(activateKeycardDeny);
            }
        }
        else if (hit.collider.CompareTag("KeyPlacerLasers"))
        {
            KeyPlacerLasers keyPlacerLasers = hit.collider.GetComponent<KeyPlacerLasers>();
            if (cardsCollectedYellow > 0 && !keyPlacerLasers.isUnlocked)
            {
                cardsCollectedYellow--;
                audioSource.PlayOneShot(activateKeycardAllow);
                UpdateCardsUI();
                keyPlacerLasers.UnlockPlacerLasers();
            }
            else if (!keyPlacerLasers.isUnlocked)
            {
                audioSource.PlayOneShot(activateKeycardDeny);
            }
        }
        else if (hit.collider.CompareTag("KeycardYellow"))
        {
            hit.collider.gameObject.SetActive(false);
            audioSource.PlayOneShot(pickupKeycardSound);
            cardsCollectedYellow++;
            UpdateCardsUI();
        }
        else if (hit.collider.CompareTag("KeycardRed"))
        {
            hit.collider.gameObject.SetActive(false);
            audioSource.PlayOneShot(pickupKeycardSound);
            cardsCollectedRed++;
            UpdateCardsUI();
        }
    }

    private void UpdateCardsUI()
    {
        yellowCards.text = "x" + cardsCollectedYellow;
        redCards.text = "x" + cardsCollectedRed;
    }
}
