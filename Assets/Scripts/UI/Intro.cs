using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.InputSystem;

namespace Sanicball.UI
{
    public class Intro : MonoBehaviour
    {
        public AssetReference MenuScene;

        public Image[] images;
        public float imgTime = 0.2f;
        public float fadeTime = 0.05f;
        private int curImg = 0;
        private bool isHoldingImage = false;
        private float holdImageTimer = 0;

        private bool isFadeOut = false;
        public InputActionProperty SkipMenuButton;
        private void OnEnable()
        {
            holdImageTimer = imgTime;
            SkipMenuButton.action.Enable();
            SkipMenuButton.action.performed += (_) => GoToMenu();
        }

        private void OnDisable()
        {
            holdImageTimer = imgTime;
            SkipMenuButton.action.Disable();
            SkipMenuButton.action.performed -= (_) => GoToMenu();
        }

        private void Update()
        {
            if (isHoldingImage)
            {
                holdImageTimer -= Time.deltaTime;
                if (holdImageTimer <= 0)
                {
                    isHoldingImage = false; //Stop the timer
                    isFadeOut = true;
                }
            }
            else
            {
                //Fade in or out
                if (isFadeOut)
                {
                    float a = images[curImg].color.a;
                    a -= fadeTime * Time.deltaTime;
                    images[curImg].color = new Color(1f, 1f, 1f, a);
                    if (a <= 0f)
                    {
                        NextImage();
                        isFadeOut = false;
                    }
                }
                else
                {
                    float a = images[curImg].color.a;
                    a += fadeTime * Time.deltaTime;
                    images[curImg].color = new Color(1f, 1f, 1f, a);
                    if (a >= 1f)
                    {
                        isHoldingImage = true;
                    }
                }
            }
        }

        private void NextImage()
        {
            if (curImg >= images.Length - 1)
            {
                GoToMenu();
                return;
            }
            images[curImg].enabled = false;
            curImg++;
            images[curImg].enabled = true;
            images[curImg].color = new Color(1f, 1f, 1f, 0f);
            holdImageTimer += imgTime;
        }

        public bool isLoadingMenu;
        private void GoToMenu()
        {
            if (isLoadingMenu) return;
            isLoadingMenu = true;
            Addressables.LoadSceneAsync(MenuScene, LoadSceneMode.Single);
            isHoldingImage = true;
        }
    }
}
