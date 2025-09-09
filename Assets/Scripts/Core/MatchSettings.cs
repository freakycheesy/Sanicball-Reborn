using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Sanicball.Data;

namespace SanicballCore
{
    public enum AISkillLevel
    {
        Retarded,
        Average,
        Dank
    }

    public enum StageRotationMode
    {
        None,
        Sequenced,
        Random,
    }

    public enum AllowedTiers
    {
        All,
        NormalOnly,
        OddOnly,
        HyperspeedOnly,
        NoHyperspeed,
    }

    public enum TierRotationMode
    {
        None,
        Cycle, // Cycle normal -> odd -> hyperspeed
        Random,  // 33% chance for normal, odd, hyper
        WeightedRandom // Most chance for normal, small chance for odd, smaller chance for hyper
    }

    [Serializable]
    public struct MatchSettings
    {
        [Newtonsoft.Json.JsonProperty]
        private string aiCharacters;

        public string StageBarcode { get; set; }
        public int Laps { get; set; }
        public int AICount { get; set; }
        public AISkillLevel AISkill { get; set; }
        public Dictionary<string, int> Aliases { get; set; }

        public int AutoStartTime { get; set; }
        public int AutoStartMinPlayers { get; set; }
        public int AutoReturnTime { get; set; }
        public float VoteRatio { get; set; }
        public StageRotationMode StageRotationMode { get; set; }
        public AllowedTiers AllowedTiers { get; set; }
        public TierRotationMode TierRotationMode { get; set; }
        public int DisqualificationTime { get; set; }
        /// <summary>
        /// Creates a MatchSettings object with the game's default settings.
        /// </summary>
        /// <returns></returns>
        public MatchSettings(string StageBarcode = "bk-tn.main.greenhillzone",
                int Laps = 2,
                int AICount = 0,
                AISkillLevel AISkill = AISkillLevel.Average,
                string aiCharacters = "1,2,3,4,5,6,7,8,9,10,11,12",
                int AutoStartTime = 60,
                int AutoStartMinPlayers = 2,
                int AutoReturnTime = 15,
                float VoteRatio = 1f,
                StageRotationMode StageRotationMode = StageRotationMode.None,
                AllowedTiers AllowedTiers = AllowedTiers.All,
                TierRotationMode TierRotationMode = TierRotationMode.None,
                int DisqualificationTime = 120
                )
        {
            this.StageBarcode = StageBarcode;
            this.Laps = Laps;
            this.AICount = AICount;
            this.AISkill = AISkill;
            this.aiCharacters = aiCharacters;

            this.AutoStartTime = AutoStartTime;
            this.AutoStartMinPlayers = AutoStartMinPlayers;
            this.AutoReturnTime = AutoReturnTime;
            this.VoteRatio = VoteRatio;
            this.StageRotationMode = StageRotationMode;
            this.AllowedTiers = AllowedTiers;
            this.TierRotationMode = TierRotationMode;
            this.DisqualificationTime = DisqualificationTime;
            this.Aliases = new();
        }

        /// <summary>
        /// Gets the AI character ID on a position. Returns default character if out of bounds.
        /// </summary>
        /// <param name="pos">Target position</param>
        /// <returns></returns>
        public int GetAICharacter(int pos)
        {
            string[] charIDs = aiCharacters.Split(',');

            if (pos >= 0 && pos < charIDs.Length)
            {
                return int.Parse(charIDs[pos]);
            }
            else
            {
                //Default to Knackles if trying to get a position out of bounds
                return 1;
            }
        }

        /// <summary>
        /// Sets the AI character ID on a position. Positive numbers only. Increases the list size if setting above current bounds.
        /// </summaryiiiii>
        /// <param name="pos">Target position</param>
        /// <param name="characterId">Character ID to use there</param>
        public void SetAICharacter(int pos, int characterId)
        {
            string[] charIDs = aiCharacters.Split(',');

            if (pos >= 0)
            {
                if (pos >= charIDs.Length)
                {
                    System.Array.Resize(ref charIDs, pos + 1);
                }
                charIDs[pos] = characterId.ToString();
                aiCharacters = string.Join(",", charIDs);
            }
        }

        /// <summary>
        /// Removes the last AI character from the list. Use for reducing the list size to avoid bloat.
        /// </summary>
        public void RemoveLastAICharacter()
        {
            string[] charIDs = aiCharacters.Split(',');
            if (charIDs.Length > 1)
            {
                System.Array.Resize(ref charIDs, charIDs.Length - 1);
                aiCharacters = string.Join(",", charIDs);
            }
        }
    }
}