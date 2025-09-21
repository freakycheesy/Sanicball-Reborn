using System.Collections;
using Sanicball.Gameplay;
using Sanicball.Logic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class TrackTest : MonoBehaviour
{
    public Ball Player;
    [Header("Music To Cope for no MusicPlayer")]
    public AudioResource music;
    private AudioSource _source;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        if (!Application.isEditor || MatchManager.Instance != null)
        {
            Destroy(this.gameObject);
            yield return new();
        }
        Instantiate(Player, RaceBallSpawner.Instance.GetSpawnPoint(0, 1), RaceBallSpawner.Instance.transform.rotation);
        TryGetComponent(out _source);
        _source.resource = music;
        _source.Play();
    }
}
