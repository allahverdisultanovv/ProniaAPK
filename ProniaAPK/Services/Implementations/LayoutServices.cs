using Microsoft.EntityFrameworkCore;
using ProniaAPK.DAL;

namespace ProniaAPK.Services.Implementations
{
    public class LayoutServices
    {
        private readonly AppDBContext _context;

        public LayoutServices(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return settings;
        }
    }
}
