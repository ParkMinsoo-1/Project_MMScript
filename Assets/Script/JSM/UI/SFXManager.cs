using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("효과음 리스트")]
    [SerializeField] private List<AudioClip> sfxClips = new();

    [Header("풀 설정")]
    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<AudioSource> pool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private void CreateNewAudioSource()
    {
        var go = Instantiate(audioSourcePrefab, transform);
        var source = go.GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D
        pool.Enqueue(source);
    }

    public void PlaySFX(int index, float volume = 1f)
    {
        if (index < 0 || index >= sfxClips.Count)
        {
            Debug.LogWarning($"SFXManager: Invalid clip index {index}");
            return;
        }

        AudioClip clip = sfxClips[index];
        PlayClip(clip, volume);
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (clip == null) return;

        AudioSource source = pool.Count > 0 ? pool.Dequeue() : CreateAndReturn();
        source.clip = clip;
        source.volume = volume;
        source.Play();

        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }

    private AudioSource CreateAndReturn()
    {
        CreateNewAudioSource();
        return pool.Dequeue();
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.clip = null;
        pool.Enqueue(source);
    }
}
