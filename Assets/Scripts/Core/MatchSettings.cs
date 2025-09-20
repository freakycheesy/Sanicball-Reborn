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
    public class MatchSettings
    {
        [Newtonsoft.Json.JsonProperty]
        private string aiCharacters { get; set; } = DEFAULTAICHARACTERS;

        public string StageBarcode { get; set; } = DEFAULTSTAGE;
        public int Laps { get; set; } = 2;
        public int AICount { get; set; } = 8;
        public AISkillLevel AISkill { get; set; } = AISkillLevel.Average;
        public Dictionary<string, int> Aliases { get; set; } = new();

        public int AutoStartTime { get; set; } = 60;
        public int AutoStartMinPlayers { get; set; } = 2;
        public int AutoReturnTime { get; set; } = 15;
        public float VoteRatio { get; set; } = 1;
        public StageRotationMode StageRotationMode { get; set; } = StageRotationMode.None;
        public AllowedTiers AllowedTiers { get; set; } = AllowedTiers.All;
        public TierRotationMode TierRotationMode { get; set; } = TierRotationMode.None;
        public int DisqualificationTime { get; set; } = 120;
        public const string DEFAULTSTAGE = "bk-tn.main.greenhillzone";
        public const string DEFAULTAICHARACTERS = "1,2,3,4,5,6,7,8,9,10,11,12";
        /// <summary>
        /// Creates a MatchSettings object with the game's default settings.
        /// </summary>
        /// <returns></returns>
        public MatchSettings(string startingStage = DEFAULTSTAGE, string startingAI= DEFAULTAICHARACTERS)
        {
            StageBarcode = startingStage;
            aiCharacters = startingAI;
        }

        public MatchSettings()
        {
            StageBarcode = DEFAULTSTAGE;
            aiCharacters = DEFAULTAICHARACTERS;
        }

        /// <summary>
        /// Gets the AI character ID on a position. Returns default character if out of bounds.
        /// </summary>
        /// <param name="pos">Target position</param>
        /// <returns></returns>
        public int GetAICharacter(int pos)
        {
            string[] charIDs = DEFAULTAICHARACTERS.Split(',');
            try
            {
                charIDs = aiCharacters.Split(',');
            }
            catch
            {
                charIDs = DEFAULTAICHARACTERS.Split(',');
            }

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