namespace FitBridge_Application.Commons.Utils
{
    public static class ConvertIntDayToString
    {
        public static string Convert(int dayOfWeek)
        {
            return dayOfWeek switch
            {
                8 => "Sunday",
                2 => "Monday",
                3 => "Tuesday",
                4 => "Wednesday",
                5 => "Thursday",
                6 => "Friday",
                7 => "Saturday",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Invalid day of the week")
            };
        }
    }
}
