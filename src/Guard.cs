#nullable enable
using System;

namespace MovieMatch
{
    public static class Guard
    {
        public static string NotNull(string name, string? value)
        {
            if (value == null)
                throw new ArgumentNullException($"{name} cannot be set to null");

            return value;
        }
    }
}