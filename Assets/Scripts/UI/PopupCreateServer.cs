using Sanicball.Logic;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using FishNet.Transporting;
using FishNet.Managing;

namespace Sanicball.UI
{
    public class PopupCreateServer : MonoBehaviour
    {
        [SerializeField]
        private InputField maxPlayersInput;
        [SerializeField]
        private InputField nameInput;
        [SerializeField]
        private Toggle isPublicInput;
        [SerializeField]
        private UI.Popup connectingPopupPrefab = null;
        [SerializeField]
        private UI.PopupHandler popupHandler = null;
        
        public void Start(){
            popupHandler = PopupHandler.Instance;
        }

        public void Create()
        {
            if (popupHandler != null)
            {
                popupHandler.OpenPopup(connectingPopupPrefab);
                PopupConnecting.ShowMessage("Creating Server...");
            }
            MatchManager.CreateLobby();
        }
    }
}

