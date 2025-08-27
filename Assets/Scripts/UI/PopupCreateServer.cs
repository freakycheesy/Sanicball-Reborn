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
        private InputField portInput;
        [SerializeField]
        private Toggle isPublicInput;
        [SerializeField]
        private Text portOutput;
        [SerializeField]
        private UI.Popup connectingPopupPrefab = null;
        [SerializeField]
        private UI.PopupHandler popupHandler = null;
        
        public void Start(){
            popupHandler = PopupHandler.Instance;
        }

        public void Create(){
            StartCoroutine(PopupParser());
        }

        public IEnumerator PopupParser()
        {
            int maxPlayers;
            int.TryParse(maxPlayersInput.text, out maxPlayers);
            int port;
            int.TryParse(portInput.text, out port);
            portOutput.text = "";
            if(port < 1024) {
                portOutput.text = "Port must be at least 1024.";
                yield break;
            }else if(port > 49151) {
                portOutput.text = "Port must be at most 49151.";
                yield break;
            }
            NetworkManager.Instances[0].ServerManager.StartConnection((ushort)port);
            NetworkManager.Instances[0].ClientManager.StartConnection();


            if (popupHandler != null)
            {
                popupHandler.OpenPopup(connectingPopupPrefab);
                PopupConnecting.ShowMessage("Creating Server...");
            }
        }
    }
}

