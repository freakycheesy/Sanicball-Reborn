using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using Sanicball.Data;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Sanicball.Logic
{
    /// <summary>
    /// Manages game state - scenes, players, all that jazz
    /// </summary>
    public partial class MatchManager : NetworkBehaviour
    {
        #region Scene changing / race loading

        [Server]
        public void GoToLobby()
        {
            if (inLobby) return;

            loadingStage = false;
            loadingLobby = true;
            SanicNetworkManager.Singleton.ServerChangeScene(SanicNetworkManager.singleton.onlineScene);
            //if(BootstrapSceneManager.Scene != null) BootstrapSceneManager.LoadScene(lobbyScene.RuntimeKey);
            //else
            //BootstrapSceneManager.LoadScene(lobbyScene.RuntimeKey);
        }
        [Server]
        public void GoToStage()
        {
            CurrentStage = ActiveData.GetStageByBarcode(CurrentSettings.StageBarcode);

            loadingStage = true;
            loadingLobby = false;

            foreach (var p in Players)
            {
                p.ReadyToRace = false;
            }

            ActiveData.LoadLevel(CurrentStage);
        }

        public static StageInfo CurrentStage;

        //Check if we were loading the lobby or the race
        public void OnLevelHasLoaded(Scene scene, LoadSceneMode mode)
        {
            if (loadingLobby)
            {
                InitLobby();
                loadingLobby = false;
            }
            if (loadingStage)
            {
                InitRace();
                loadingStage = false;
            }
            foreach (var camera in Resources.FindObjectsOfTypeAll<UniversalAdditionalCameraData>())
            {
                camera.renderPostProcessing = true;
            }
        }

        //Initiate the lobby after loading lobby scene
        public void InitLobby()
        {
            inLobby = true;

            if (showSettingsOnLobbyLoad)
            {
                //Let the player pick settings first time entering the lobby
                LobbyReferences.Active.MatchSettingsPanel.Show();
                showSettingsOnLobbyLoad = false;
            }
            MatchManagerSpawned?.Invoke(this, Time.time);
        }

        //Initiate a race after loading the stage scene
        public void InitRace()
        {
            inLobby = false;

            var raceManager = Instantiate(raceManagerPrefab);
            raceManager.Init(CurrentSettings, this, joiningRaceInProgress);
            joiningRaceInProgress = false;
        }

        public void QuitMatch(string reason = null)
        {
            StartCoroutine(QuitMatchInternal(reason));
        }

        public IEnumerator QuitMatchInternal(string reason)
        {
            SanicNetworkManager.LeaveLobby();

            if (reason != null)
            {
                yield return null;

                FindAnyObjectByType<UI.PopupHandler>().OpenPopup(disconnectedPopupPrefab);
                FindAnyObjectByType<UI.PopupDisconnected>().Reason = reason;
            }

            //Addressables.LoadSceneAsync(menuScene);
        }

        #endregion Scene changing / race loading
    }
}
