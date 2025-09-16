using System.Text;

namespace FitBridge_Application.Specifications.Gym
{
    public static class StringCapitalizationConverter
    {
        public static string ToUpperFirstChar(string jsonString)
        {
            var sb = new StringBuilder();
            if (jsonString.Length > 0)
            {
                var firstChar = jsonString[0];
                sb.Append(char.ToUpper(firstChar));
                if (jsonString.Length > 1)
                {
                    sb.Append(jsonString[1..]);
                }
            }
            return sb.ToString();
        }
    }
}