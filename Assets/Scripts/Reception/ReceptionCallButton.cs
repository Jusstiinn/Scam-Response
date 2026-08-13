using UnityEngine;

public class ReceptionCallButton : MonoBehaviour, IInteractable
{
    [SerializeField] private ReceptionManager receptionManager;
    [SerializeField] private PressButtonAnimation physicalButton;

    [Header("Button VFX")]
    [SerializeField] private GameObject buttonVfxPrefab;
    [SerializeField] private Transform vfxSpawnPoint;
    [SerializeField] private float vfxLifetime = 2f;

    [Header("Button VFX")]
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip buttonPressSfx;

    private bool glowed = false;

    public string InteractionPrompt =>
        "Press E to call the next number";

    public bool CanInteract =>
        GameManager.Instance != null &&
        GameManager.Instance.CurrentPhase == GamePhase.Reception;

    public void Interact()
    {
        if (!CanInteract)
            return;

        // Play physical red button press animation.
        physicalButton?.Press();

        // Play button VFX.
        PlayButtonVfx();
        PlayButtonSound();

        if (!glowed)
        {
            GetComponent<ObjectiveHighlightTarget>()?
            .CompleteObjective();
            glowed = true;
        }

        // Call the current queue number.
        receptionManager?.CallCurrentNumber();
    }

    private void PlayButtonVfx()
    {
        if (buttonVfxPrefab == null)
            return;

        Transform spawn =
            vfxSpawnPoint != null
                ? vfxSpawnPoint
                : transform;

        GameObject vfx = Instantiate(
            buttonVfxPrefab,
            spawn.position,
            spawn.rotation
        );

        Destroy(vfx, vfxLifetime);
    }

    private void PlayButtonSound()
    {
        if (buttonAudioSource == null || buttonPressSfx == null)
            return;

        buttonAudioSource.PlayOneShot(buttonPressSfx);
    }
}