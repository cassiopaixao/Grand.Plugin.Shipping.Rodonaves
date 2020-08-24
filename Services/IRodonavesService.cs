using Grand.Services.Shipping;
using System.Threading.Tasks;

namespace Grand.Plugin.Shipping.Rodonaves.Services
{
    public interface IRodonavesService
    {
        public Task<decimal> GetRate(GetShippingOptionRequest getShippingOptionRequest);
    }
}
