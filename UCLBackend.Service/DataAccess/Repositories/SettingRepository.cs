using System.Linq;
using UCLBackend.Service.DataAccess.Models;
using UCLBackend.Service.DataAccess.Interfaces;
using UCLBackend.Service.Data.Enums;
using System;
using System.Collections.Generic;

namespace UCLBBackend.DataAccess.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly UCLContext _context;

        public SettingRepository(UCLContext context)
        {
            _context = context;
        }

        public string GetSetting(string setting)
        {
            return _context.Settings.FirstOrDefault(x => x.Key == setting).Value;
        }

        public Dictionary<PlayerLeague, double> GetMinSalaries()
        {
            return _context.Settings.Where(x => x.Key.StartsWith("League") && x.Key.EndsWith("MinSalary")).ToDictionary(x =>
            {
                var key = x.Key.Split('.')[1];
                return (PlayerLeague)Enum.Parse(typeof(PlayerLeague), key);
            }, x => double.Parse(x.Value));
        }
    }
}
