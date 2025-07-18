﻿namespace Lims.ToolsForClient.Extensions
{
    public static class DateTimeExtensions
    { /// <summary>
      ///     A T extension method that check if the value is between (exclusif) the minValue and maxValue.
      /// </summary>
      /// <param name="this">The @this to act on.</param>
      /// <param name="minValue">The minimum value.</param>
      /// <param name="maxValue">The maximum value.</param>
      /// <returns>true if the value is between the minValue and maxValue, otherwise false.</returns>
      /// ###
      /// <typeparam name="T">Generic type parameter.</typeparam>
        public static bool Between(this DateTimeOffset @this, DateTimeOffset minValue, DateTimeOffset maxValue)
        {
            return minValue.CompareTo(@this) == -1 && @this.CompareTo(maxValue) == -1;
        }
    }
}
