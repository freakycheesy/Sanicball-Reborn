using Sanicball;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Sanicball.UI;
using Sanicball.Logic;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;

namespace Sanicball
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        //public GUISkin skin;
        public static MusicPlayer Instance;
        public MusicPlayerCanvas playerCanvasPrefab;
        public bool playerCanvasLobbyOffset = false;
        private MusicPlayerCanvas playerCanvas;

        public bool startPlaying = false;
        public bool fadeIn = false;

        public static List<Song> Playlist = new();
        public AudioSource fastSource;

        [System.NonSerialized]
        public bool fastMode = false;

        private int currentSongID;
        private bool isPlaying;
        private string currentSongCredits;

        //Song credits
        private float slidePosition;
        private float slidePositionMax = 20;

        private AudioSource aSource;

        public void Play()
        {
            Song song = Playlist[currentSongID];
            Play($"{song.name} ({song.BARCODE})");
        }

        public void Play(string credits)
        {
            if (!ActiveData.GameSettings.music) return;
            playerCanvas.Show(credits);
            isPlaying = true;
            aSource.Play();
            Debug.Log("Playing Song");
        }

        public void Pause()
        {
            aSource.Pause();
        }

        private void Start()
        {
            Instance = this;
            playerCanvas = Instantiate(playerCanvasPrefab);
            if (playerCanvasLobbyOffset) 
            {
                playerCanvas.lobbyOffset = true;
            }

            aSource = GetComponent<AudioSource>();

            slidePosition = slidePositionMax;
            ShuffleSongs();

            if (ActiveData.ESportsFullyReady)
            {
                if (!MatchManager.Instance.InLobby) {
                    List<Song> p = new();
                    Song s = ActiveData.ESportsMusic;
                    p.Add(s);
                    p.Insert(0,s);
                    Playlist = p;
                }
            }

            currentSongID = 0;
            aSource.resource = Playlist[currentSongID].resource;
            isPlaying = aSource.isPlaying;
            if (startPlaying && ActiveData.GameSettings.music)
            {
                Play();
            }
            if (fadeIn)
            {
                aSource.volume = 0f;
            }
            if (!ActiveData.GameSettings.music)
            {
                fastSource.Stop();
            }
        }
        private float audioTime;
        public bool CanPlay()
        {
            bool canPlay = Time.timeScale > 0;
            if (canPlay)
            {
                if (RaceManager.Instance)
                {
                    canPlay = RaceManager.Instance.CurrentState.HasFlag(RaceState.Racing) || RaceManager.Instance.CurrentState.HasFlag(RaceState.Finished);
                }
            }
            return canPlay;
        }
        private void Update()
        {
            if (CanPlay())
            {
                audioTime = aSource.time;
                MusicPlayerLogic();
            }
            else
            {
                aSource.time = audioTime;
            }
        }

        public bool NeedsChangingMusic() {
            return GameInput.IsChangingSong() || aSource.time > aSource.clip.length || (!aSource.isPlaying && aSource.time <= 0);
        }

        void MusicPlayerLogic()
        {
            if (fadeIn && aSource.volume < 0.5f)
            {
                aSource.volume = Mathf.Min(aSource.volume + Time.deltaTime * 0.1f, 0.5f);
            }
            //If it's not playing but supposed to play, change song
            if (NeedsChangingMusic())
            {
                if (currentSongID < Playlist.Count - 1)
                {
                    currentSongID++;
                }
                else
                {
                    currentSongID = 0;
                }
                aSource.resource = Playlist[currentSongID].resource;
                slidePosition = slidePositionMax;
                Play();
            }

            if (fastMode && fastSource.volume < 1)
            {
                fastSource.volume = Mathf.Min(1, fastSource.volume + Time.deltaTime * 0.25f);
                aSource.volume = 0.5f - fastSource.volume / 2;
            }
            if (!fastMode && fastSource.volume > 0)
            {
                fastSource.volume = Mathf.Max(0, fastSource.volume - Time.deltaTime * 0.5f);
                aSource.volume = 0.5f - fastSource.volume / 2;
            }
            if (aSource.time > 0)
            {
                slidePosition = Mathf.Lerp(slidePosition, 0, Time.deltaTime * 4);
            }
            else
            {
                slidePosition = Mathf.Lerp(slidePosition, slidePositionMax, Time.deltaTime * 2);
            }
        }

        private void ShuffleSongs()
        {
            Debug.Log("Shuffling Songs");
            for (int i = Playlist.Count; i > 1; i--)
            {
                int j = Random.Range(0, i);
                Song tmp = Playlist[j];
                Playlist[j] = Playlist[i - 1];
                Playlist[i - 1] = tmp;
            }
            Debug.Log("Shuffled Songs");
        }
    }

    [System.Serializable]
    public class Song
    {
        public string BARCODE;
        public string name;
        public AudioResource resource;
    }
}