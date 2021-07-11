using System.Linq;
using UCLBackend.DataAccess.Models;
using UCLBackend.Service.DataAccess.Interfaces;

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
    }
}
