using ProniaAPK.ViewModels;

namespace ProniaAPK.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task<List<BasketItemVM>> GetBasketAsync();
    }
}
