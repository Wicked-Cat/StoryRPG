namespace Engine.Models
{
    public class Time
    {
        public enum Years { MoonsTears, BoarsTusks, Catfish, }
        public enum Seasons { Winter, Spring, Summer, Fall } //months
        public enum Days { Mondas, Turdas, Wendas, Tirdas, Fridas, Sardas, Sundas }
        public enum TimeOfDay { Dawn, Day, Dusk, Night}
        public TimeOfDay CurrentTimeOfDay { get; set; }
        public Days CurrentDay { get; set; }
        public Seasons CurrentSeason { get; set; }
        public Years CurrentYear { get; set; }
        public int DaysPassed;
        public int Week;
        public int Day;
        public int Hour;
        public int Minute;
        public bool IsMonthEnd => Day > 31;

        public Time(int daysPassed, int day, int hour, int minute, Days currentDay, Seasons currentSeason, Years currentYear)
        {
            DaysPassed = daysPassed;
            Day = day;
            Hour = hour;
            Minute = minute;
            CurrentDay = currentDay;
            CurrentSeason = currentSeason;
            CurrentYear = currentYear;
        }

        public string WriteTime()
        {
            return $"{CurrentSeason} of {CurrentYear}, {CurrentDay}, {(Hour >= 10 ? $"{Hour}" : $"0{Hour}")} : {(Minute >= 10 ? $"{Minute}" : $"0{Minute}")} ({CurrentTimeOfDay})";
            //t{(damage > 1 ? "s" : "")}.");
        }
        public TimeOfDay ProgressTimeOfDay(TimeOfDay timeOfDay, int hour)
        {
            if (Hour >= 0 && Hour < 5)
                return TimeOfDay.Night;
            else if (Hour >= 5 && Hour < 7)
                return TimeOfDay.Dawn;
            else if (Hour >= 7 && Hour < 19)
                return TimeOfDay.Day;
            else if (Hour >= 19 && Hour < 21)
                return TimeOfDay.Dusk;
            else if (Hour >= 21 && Hour <= 24)
                return TimeOfDay.Night;

            return TimeOfDay.Night;

        }
        public Days ProgressDay(Days day)
        {
            switch(day)
            {
                case Days.Mondas:
                    return Days.Turdas;
                    break;
                case Days.Turdas:
                    return Days.Wendas;
                    break;
                case Days.Wendas:
                    return Days.Tirdas;
                    break;
                case Days.Tirdas:
                    return Days.Fridas;
                    break;
                case Days.Fridas:
                    return Days.Sardas;
                    break;
                case Days.Sardas:
                    return Days.Sundas;
                    break;
                case Days.Sundas:
                    return Days.Mondas;
                    break;
            }

            return Days.Mondas;
        }
        public Seasons ProgressSeason(Seasons season)
        {
            switch (season)
            {
                case Seasons.Spring:
                    return Seasons.Summer;
                    break;
                case Seasons.Summer:
                    return Seasons.Fall;
                    break;
                case Seasons.Fall:
                    return Seasons.Winter;
                    break;
                case Seasons.Winter:
                    return Seasons.Spring;
                    break;
            }

            return Seasons.Summer;
        }
        public Years ProgressYear(Years years)
        {
            switch (years)
            {
                case Years.MoonsTears:
                    return Years.BoarsTusks;
                    break;
                case Years.BoarsTusks:
                    return Years.Catfish;
                    break;
                case Years.Catfish:
                    return Years.MoonsTears;
                    break;
            }

            return Years.Catfish;
        }
    }
}
