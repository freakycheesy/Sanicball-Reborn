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

        private MatchManager manager { get; set; }

        private void Start()
        {
            MatchManager.MatchSettingsChanged += (a, _) => Manager_MatchSettingsChanged(a as MatchManager);

            //Invoke callback immediately to set initial settings
            MatchManager.MatchManagerSpawned += (a, _)=>Manager_MatchSettingsChanged(a as MatchManager);
        }

        private void Manager_MatchSettingsChanged(MatchManager matchManager)
        {
            manager = matchManager;
            MatchSettings s = manager ? manager.State.CurrentSettings : new();
            StageInfo stage = new();
            if (!ActiveData.Instance.TryGetStageByBarcode(s.StageBarcode, out stage)) stage = ActiveData.Instance.GetStageByBarcode(MatchSettings.DEFAULTSTAGE);
            var stageId = ActiveData.Instance.GetIndexFromStage(stage);
            if(stageLayoutCamera) targetStageCamPos = new Vector3(stageId * 50, stageLayoutCamera.transform.position.y, stageLayoutCamera.transform.position.z);
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