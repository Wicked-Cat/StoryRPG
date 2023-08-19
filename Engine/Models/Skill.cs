using Newtonsoft.Json;

namespace Engine.Models
{
    public class Skill : BaseNotificationClass
    {
        #region Private Variables
        private string _name;
        private string _description;
        private int _baseLevel;
        private double _effectiveLevel;
        private double _experience;
        private Rank _currentRank;
        #endregion

        #region Public Variables
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public string DisplayName => $"{CurrentRank} {Name}";
        [JsonIgnore]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        public enum Categories { Survival, Combat, Mystic, Social, Crafting, Curse}
        [JsonIgnore]
        public Categories Category { get; set; }
        public int BaseLevel
        {
            get { return _baseLevel; }
            set
            {
                _baseLevel = value;
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged();
            }
        }
        public double LevelMultiplier { get; set; }
        [JsonIgnore]
        public double EffectiveLevel => Convert.ToInt32(BaseLevel * LevelMultiplier);
        public double Experience
        {
            get { return _experience; }
            set
            {
                _experience = value;
                OnPropertyChanged();
            }
        }
        public double ExperienceMultiplier { get; set; }
        public enum Rank { Not, Dabbling, Novice, Competent, Skilled, Adept, Expert, 
            Master, Grandmaster, Legendary, Fledgeling, Blooded, Elder, Ancient}
        [JsonIgnore]
        public Rank CurrentRank
        {
            get { return _currentRank; }
            set 
            {
                _currentRank = value;
                OnPropertyChanged(nameof(CurrentRank));
            }
        }
        #endregion

        #region Constructor
        public Skill(string name, string description)
        {
            Name = name;
            Description = description;
            BaseLevel = 1;
            LevelMultiplier = 1;
            ExperienceMultiplier = 1;
        }
        #endregion

        public Rank DetermineRank()
        {
            if(BaseLevel >= 0 && BaseLevel < 10)
                return Rank.Not;
            if (BaseLevel >= 10 && BaseLevel < 20)
                return Rank.Dabbling;
            if (BaseLevel >= 20 && BaseLevel < 30)
                return Rank.Novice;
            if (BaseLevel >= 30 && BaseLevel < 40)
                return Rank.Competent;
            if (BaseLevel >= 40 && BaseLevel < 50)
                return Rank.Skilled;
            if (BaseLevel >= 50 && BaseLevel < 60)
                return Rank.Adept;
            if (BaseLevel >= 60 && BaseLevel < 70)
                return Rank.Expert;
            if (BaseLevel >= 70 && BaseLevel < 80)
                return Rank.Master;
            if (BaseLevel >= 80 && BaseLevel < 90)
                return Rank.Grandmaster;
            if (BaseLevel >= 90 && BaseLevel <= 100)
                return Rank.Legendary;

            return Rank.Fledgeling;
        }
        public void AddExperience(double num)
        {
            for (int i = 0; i < num; i++)
            {
                Experience++;
                if (Experience >= (BaseLevel * 3))
                {
                    LevelUp();
                }
            }
        }
        public void LevelUp()
        {
            Experience -= (BaseLevel * 3);
            BaseLevel++;
            if (BaseLevel > 100)
            {
                BaseLevel = 100;
                Experience = 0;
            }
            CurrentRank = DetermineRank();
        }

        public Skill Clone()
        {
            Skill skill = new Skill(Name, Description);
            skill.Category = Category;
            return skill;
        }
    }
}
