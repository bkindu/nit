using System;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Compatibility facade — JavaScript/Jint kundli engine removed.
    /// All calculations run in pure C# <see cref="KundliCalculator"/> (debug that class).
    /// The scriptPath constructor argument is ignored (kept so existing controllers compile).
    /// </summary>
    public sealed class WorkingJavascriptKundliCalculator
    {
        private readonly KundliCalculator _calculator = new KundliCalculator();

        public WorkingJavascriptKundliCalculator(string scriptPath)
        {
            // scriptPath intentionally unused — no JavaScript astronomy path.
        }

        /// <summary>
        /// Same signature as before: (day, month, year, hour, minute, longitude, latitude, timezonePacked).
        /// </summary>
        public KundliResult CalculateKundli(
            int day, int month, int year,
            int hour, int minute,
            double longitude, double latitude,
            double timezoneOffset)
        {
            return _calculator.CalculateKundli(
                day, month, year, hour, minute, longitude, latitude, timezoneOffset);
        }
    }
}
