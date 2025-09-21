using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class MatchSettingsDisplayPanel : MonoBehaviour
    {
        [Header("Fields")]
        public Text stageName;

        public Image stageImage;
        public Text lapCount;
        public Text aiOpponents;
        public Text aiSkill;

        private Vector3 targetStageCamPos;

        [SerializeField]
        private Animation settingsChangedAnimation = null;

        [SerializeField]
        private Camera stageLayoutCamera = null;

        public static MatchSettingsDisplayPanel Instance;

        private void Start()
        {
            Instance = this;

            //Invoke callback immediately to set initial settings
            MatchManager.MatchSettingsChanged +=  Manager_MatchSettingsChanged;
            MatchManager.MatchManagerSpawned +=  Manager_MatchSettingsChanged;
        }

        void OnDestroy()
        {
            MatchManager.MatchSettingsChanged-=Manager_MatchSettingsChanged;
            MatchManager.MatchManagerSpawned -= Manager_MatchSettingsChanged;
        }

        private void Manager_MatchSettingsChanged(object sender, float time)
        {
            Manager_MatchSettingsChanged(sender, (sender as MatchManager).state.CurrentSettings);
        }

        private void Manager_MatchSettingsChanged(object sender, MatchSettings settings)
        {
            MatchSettings s = settings;
            StageInfo stage;
            if (!ActiveData.Instance.TryGetStageByBarcode(s.StageBarcode, out stage)) stage = ActiveData.Instance.GetStageByBarcode(MatchSettings.DEFAULTSTAGE);
            var stageId = ActiveData.Instance.GetIndexFromStage(stage);
            if (stageLayoutCamera) targetStageCamPos = new Vector3(stageId * 50, stageLayoutCamera.transform.position.y, stageLayoutCamera.transform.position.z);
            stageName.text = stage.name;
            stageImage.sprite = stage.picture;
            lapCount.text = s.Laps + (s.Laps == 1 ? " lap" : " laps");
            aiOpponents.text = "";
            /*foreach (var i in s.aiCharacters)
            {
                aiOpponents.text += ActiveData.Characters[i].name + "\n";
            }*/
            aiSkill.text = "AI Skill: " + s.AISkill;

            settingsChangedAnimation?.Rewind();
            settingsChangedAnimation?.Play();
        }

        private void Update()
        {
            if (Vector3.Distance(stageLayoutCamera.transform.position, targetStageCamPos) > 0.1f)
            {
                stageLayoutCamera.transform.position = Vector3.Lerp(stageLayoutCamera.transform.position, targetStageCamPos, Time.deltaTime * 10f);
                if (Vector3.Distance(stageLayoutCamera.transform.position, targetStageCamPos) <= 0.1f)
                {
                    stageLayoutCamera.transform.position = targetStageCamPos;
                }
            }
        }
    }
}