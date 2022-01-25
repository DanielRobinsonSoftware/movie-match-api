#nullable enable
using System;

namespace MovieMatch
{
    public static class Guard
    {
        public static T NotNull<T>(string name, T value)
        {
            if (value == null)
                throw new ArgumentNullException($"{name} cannot be set to null");

            return value;
        }
    }
}