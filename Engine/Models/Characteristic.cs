using Newtonsoft.Json;

namespace Engine.Models
{
    public class Characteristic : BaseNotificationClass
    {
        #region Private Variables
        private string _name;
        private string _description;
        private int _baseLevel;
        private double _levelMultiplier;
        private double _effectiveLevel;
        private double _experience;
        private double _experienceMultiplier;
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
        public int BaseLevel
        {
            get { return _baseLevel; }
            set
            {
                _baseLevel = value;
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged();
                OnPropertyChanged(nameof(EffectiveLevel));
            }
        }
        public double LevelMultiplier { get; set; }
        [JsonIgnore]
        public double EffectiveLevel => Convert.ToInt32(BaseLevel * LevelMultiplier);
        [JsonIgnore]
        public double DifferenceBetween => EffectiveLevel - BaseLevel;
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
        public enum Rank { VeryLow, Low, BelowAverage, Average, AboveAverage, High, VeryHigh, Unnatural, Unused}
        [JsonIgnore]
        public Rank CurrentRank
        {
            get { return _currentRank; }
            set
            {
                _currentRank = value;
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor
        public Characteristic (string name, string description)
        {
            Name = name;
            Description = description;
            BaseLevel = 1;
            LevelMultiplier = 1;
            ExperienceMultiplier = 1;
        }

        #endregion

        public override string ToString()
        {
            double r = Math.Round(DifferenceBetween, 1);
            return string.Format($"{CurrentRank} {Name} {EffectiveLevel} ({(r > 0 ? $"+{r}" : $"{r}")})");
        }

        public Rank DetermineRank()
        {
            OnPropertyChanged(nameof(CurrentRank));
            if (BaseLevel >= 0 && BaseLevel < 10)
                return Rank.VeryLow;
            if (BaseLevel >= 10 && BaseLevel < 20)
                return Rank.Low;
            if (BaseLevel >= 20 && BaseLevel < 30)
                return Rank.BelowAverage;
            if (BaseLevel >= 30 && BaseLevel < 40)
                return Rank.Average;
            if (BaseLevel >= 40 && BaseLevel < 50)
                return Rank.Average;
            if (BaseLevel >= 50 && BaseLevel < 60)
                return Rank.Average;
            if (BaseLevel >= 60 && BaseLevel < 70)
                return Rank.AboveAverage;
            if (BaseLevel >= 70 && BaseLevel < 80)
                return Rank.High;
            if (BaseLevel >= 80 && BaseLevel < 90)
                return Rank.VeryHigh;
            if (BaseLevel >= 90 && BaseLevel <= 100)
                return Rank.Unnatural;

            return Rank.Unused;
        }

        public void AddExperience(double num)
        {
            for (int i = 0; i < num; i++)
            {
                Experience++;
                if (Experience >= (BaseLevel * 10))
                {
                    LevelUp();
                }
                if(BaseLevel == 100)
                {
                    Experience = 0;
                }
            }
        }
        public void LevelUp()
        {
            Experience -= (BaseLevel * 10);
            BaseLevel++;
            if (BaseLevel > 100)
            {
                BaseLevel = 100;
                Experience = 0;
            }
            CurrentRank = DetermineRank();
        }
        public Characteristic Clone()
        {
            Characteristic characteristic = new Characteristic(Name, Description);
            return characteristic;
        }
    }
}
