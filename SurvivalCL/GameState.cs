using System;

namespace SurvivalCL
{
    public enum Season { Spring, Summer, Autumn, Winter }
    public enum DayPart { Night, Dawn, Day, Dusk }
    public enum WeatherType { Sunny, Cloudy, Rainy, Storm }

    public class GameState
    {
        // Calendar constants
        public const int DaysPerYear = 400;
        public const int SeasonsPerYear = 4;
        public const int DaysPerSeason = 100;
        public const int MonthsPerYear = 16;
        public const int DaysPerMonth = 25;
        public const int HoursPerDay = 20;
        public const int MinutesPerDay = 1200;
        public const int MinutesPerHour = 60;

        public GameState(bool v)
        {
            StartGame();
        }

        // State
        public int Year { get; private set; } = 1;
        public int DayOfYear { get; private set; } = 1; // 1..400
        public int Hour { get; private set; } = 0;      // 0..19
        public int Minute { get; private set; } = 0;    // 0..59

        // Weather
        public WeatherType Weather { get; private set; } = WeatherType.Sunny;
        public float Temperature { get; private set; } = 20f; // Celsius
        public float RainLevel { get; private set; } = 0f;    // mm/h
        public bool Storm { get; private set; } = false;

        // Derived properties
        public int Month => ((DayOfYear - 1) / DaysPerMonth) + 1; // 1..16
        public int DayOfMonth => ((DayOfYear - 1) % DaysPerMonth) + 1; // 1..25
        public Season CurrentSeason => (Season)((DayOfYear - 1) / DaysPerSeason);

        // Dynamic day part durations (in minutes)
        public int NightDuration { get; private set; }
        public int DawnDuration { get; private set; }
        public int DayDuration { get; private set; }
        public int DuskDuration { get; private set; }

        public DayPart CurrentDayPart => GetDayPart();

        // Advance time by minutes
        public void AdvanceTime(int minutes)
        {
            Minute += minutes;
            while (Minute >= MinutesPerHour)
            {
                Minute -= MinutesPerHour;
                Hour++;
            }
            while (Hour >= HoursPerDay)
            {
                Hour -= HoursPerDay;
                NextDay();
            }
        }

        public void SetTemperature(float temperature)
        {
            Temperature = Math.Clamp(temperature, -30f, 50f); // realistic bounds
        }

        private void NextDay()
        {
            DayOfYear++;
            if (DayOfYear > DaysPerYear)
            {
                DayOfYear = 1;
                Year++;
            }
            UpdateWeather();
            UpdateDayPartDurations();
        }

        // Update dynamic day part durations based on DayOfYear
        private void UpdateDayPartDurations()
        {
            // Mediterranean-like: longest day at day 200 (summer solstice), shortest at day 0/400 (winter solstice)
            // Use a sine wave to smoothly vary day length between 7 and 13 hours (in 20-hour day)
            double radians = 2 * Math.PI * (DayOfYear - 100) / DaysPerYear; // -100 to set max at day 200
            double dayHours = 10 + 3 * Math.Sin(radians); // 7 to 13 hours

            DayDuration = (int)(dayHours * MinutesPerHour);
            DawnDuration = 60; // 1 hour
            DuskDuration = 60; // 1 hour
            NightDuration = MinutesPerDay - DayDuration - DawnDuration - DuskDuration;
        }

        // Day part calculation (simplified, can be made more complex)
        private DayPart GetDayPart()
        {
            int totalMinutes = Hour * MinutesPerHour + Minute;
            int night1 = NightDuration / 2;
            int dawnStart = night1;
            int dawnEnd = dawnStart + DawnDuration;
            int dayStart = dawnEnd;
            int dayEnd = dayStart + DayDuration;
            int duskStart = dayEnd;
            int duskEnd = duskStart + DuskDuration;
            // night2 = MinutesPerDay - duskEnd

            if (totalMinutes < dawnStart)
                return DayPart.Night;
            if (totalMinutes < dawnEnd)
                return DayPart.Dawn;
            if (totalMinutes < dayEnd)
                return DayPart.Day;
            if (totalMinutes < duskEnd)
                return DayPart.Dusk;
            return DayPart.Night;
        }

        // Weather simulation (simplified, can be expanded)
        private void UpdateWeather()
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            // Example: more rain in winter, more sun in summer
            switch (CurrentSeason)
            {
                case Season.Winter:
                    Weather = rand.NextDouble() < 0.5 ? WeatherType.Rainy : WeatherType.Cloudy;
                    Temperature = rand.Next(-5, 10);
                    RainLevel = Weather == WeatherType.Rainy ? rand.Next(2, 10) : 0;
                    Storm = rand.NextDouble() < 0.1;
                    break;
                case Season.Spring:
                case Season.Autumn:
                    Weather = rand.NextDouble() < 0.3 ? WeatherType.Rainy : (rand.NextDouble() < 0.5 ? WeatherType.Cloudy : WeatherType.Sunny);
                    Temperature = rand.Next(10, 20);
                    RainLevel = Weather == WeatherType.Rainy ? rand.Next(1, 5) : 0;
                    Storm = rand.NextDouble() < 0.05;
                    break;
                case Season.Summer:
                    Weather = rand.NextDouble() < 0.1 ? WeatherType.Rainy : (rand.NextDouble() < 0.3 ? WeatherType.Cloudy : WeatherType.Sunny);
                    Temperature = rand.Next(20, 35);
                    RainLevel = Weather == WeatherType.Rainy ? rand.Next(1, 3) : 0;
                    Storm = rand.NextDouble() < 0.02;
                    break;
            }
            if (Storm) Weather = WeatherType.Storm;
        }

        internal void StartGame()
        {
            Year = 1;
            DayOfYear = 1;
            Hour = 6;      // Set to 6 for morning/day start
            Minute = 0;

            Weather = WeatherType.Sunny;
            Temperature = 25f;
            RainLevel = 0f;
            Storm = false;

            UpdateDayPartDurations();
        }
    }
}