namespace Engine.Models
{
    public class Time
    {
        public string CurrentTimeOfDay { get; set; }
        public string CurrentDay { get; set; }
        public string CurrentSeason { get; set; }
        public string CurrentYear { get; set; }
        public int DaysPassed;
        public int Week;
        public int Day;
        public int Hour;
        public int Minute;
        public bool IsMonthEnd => Day > 31;

        public Time(int daysPassed, int day, int hour, int minute, string currentDay, string currentSeason, string currentYear)
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
        public string ProgressTimeOfDay(string timeOfDay, int hour)
        {
            if (Hour >= 0 && Hour < 5)
                return "Night";
            else if (Hour >= 5 && Hour < 7)
                return "Dawn";
            else if (Hour >= 7 && Hour < 19)
                return "Day";
            else if (Hour >= 19 && Hour < 21)
                return "Dusk";
            else if (Hour >= 21 && Hour <= 24)
                return "Night";

            return "Night";

        }
        public string ProgressDay(string day)
        {
            switch(day)
            {
                case "Wolf day":
                    return "Salmon day";
                case "Salmon day":
                    return "Lion day";
                case "Lion day":
                    return "Buffalo day";
                case "Buffalo day":
                    return "Goose day";
                case "Goose day":
                    return "Antelope day";
                case "Antelope day":
                    return "Hare day";
                case "Hare day":
                    return "Wolf day";
            }

            return "Salmon day";
        }
        public string ProgressSeason(string season)
        {
            switch (season)
            {
                case "Spring":
                    return "Summer";
                case "Summer":
                    return "Fall";
                case "Fall":
                    return "Winter";
                case "Winter":
                    return "Spring";
            }

            return "Summer";
        }
        public string ProgressYear(string years)
        {
            switch (years)
            {
                case "Moon's tears":
                    return "Boar's tusks";
                case "Boar's tusks":
                    return "Cat and fish";
                case "Cat and fish":
                    return "Moon's tears";
            }

            return "Cat and fish";
        }
       /* public Days GetDayByInt(int day)
        {

        }*/
    }
}
