using Grand.Core.Domain.Orders;
using Grand.Core.Plugins;
using Grand.Services.Shipping;
using Grand.Services.Shipping.Tracking;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Plugin.Shipping.Rodonaves
{
    public class RodonavesComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        public ShippingRateComputationMethodType ShippingRateComputationMethodType => throw new NotImplementedException();

        public IShipmentTracker ShipmentTracker => throw new NotImplementedException();

        public Task<decimal?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            throw new NotImplementedException();
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            throw new NotImplementedException();
        }

        public Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {
            throw new NotImplementedException();
        }
    }
}
