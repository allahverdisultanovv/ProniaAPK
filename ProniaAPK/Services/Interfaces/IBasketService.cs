using ProniaAPK.ViewModels;

namespace ProniaAPK.Services.Interfaces
{
    public interface IBasketService
    {
        Task<List<BasketItemVM>> GetBasketAsync();
    }
}
