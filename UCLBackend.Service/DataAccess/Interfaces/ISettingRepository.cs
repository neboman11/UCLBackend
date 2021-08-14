using System.Collections.Generic;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.DataAccess.Interfaces
{
    public interface ISettingRepository
    {
        string GetSetting(string setting);
        Dictionary<PlayerLeague, double> GetMinSalaries();
    }
}