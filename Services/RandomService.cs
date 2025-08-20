using Microsoft.EntityFrameworkCore;
using ProvaPub.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository;
using System;

namespace ProvaPub.Services
{
    public class RandomService: IRandomService
    {
        private readonly TestDbContext _ctx;
        private readonly Random _random;
        public RandomService(TestDbContext ctx)
        {
            _ctx = ctx;
            _random = new Random();
        }
        public async Task<int> GetRandom()
        {
            int number;
            do
            {
                number = _random.Next(0, 100);

            } while (await _ctx.Numbers.AnyAsync(s => s.Number == number));
            _ctx.Numbers.Add(new RandomNumber() { Number = number });
            _ctx.SaveChanges();

            return number;
        }
    }
}
