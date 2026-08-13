using UnityEngine;

public enum NpcVoiceType
{
    Male,
    Female1,
    Female2
}

public class NpcMumbleAudio : MonoBehaviour
{
    [Header("Voice")]
    [SerializeField] private NpcVoiceType voiceType;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Male Clips")]
    [SerializeField] private AudioClip[] maleClips;

    [Header("Female 1 Clips")]
    [SerializeField] private AudioClip[] female1Clips;

    [Header("Female 2 Clips")]
    [SerializeField] private AudioClip[] female2Clips;

    [Header("Variation")]
    [SerializeField] private Vector2 pitchRange =
        new Vector2(0.95f, 1.05f);

    public void PlayMumble()
    {
        if (audioSource == null)
            return;

        AudioClip[] clips = GetClips();

        if (clips == null || clips.Length == 0)
            return;

        AudioClip clip =
            clips[Random.Range(0, clips.Length)];

        if (clip == null)
            return;

        audioSource.Stop();

        audioSource.pitch =
            Random.Range(
                pitchRange.x,
                pitchRange.y
            );

        audioSource.PlayOneShot(clip);
    }

    public void StopMumble()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.pitch = 1f;
    }

    private AudioClip[] GetClips()
    {
        switch (voiceType)
        {
            case NpcVoiceType.Male:
                return maleClips;

            case NpcVoiceType.Female1:
                return female1Clips;

            case NpcVoiceType.Female2:
                return female2Clips;
        }

        return null;
    }
}