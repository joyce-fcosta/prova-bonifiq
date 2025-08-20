using System;
using ProvaPub.Interfaces;

namespace ProvaPub.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

