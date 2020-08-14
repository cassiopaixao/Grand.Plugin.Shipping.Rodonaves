using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Shipping;
using Grand.Core.Plugins;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Shipping;
using Grand.Services.Shipping.Tracking;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grand.Plugin.Shipping.Rodonaves
{
    public class RodonavesComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public RodonavesComputationMethod(ISettingService settingService, ILocalizationService localizationService, ILanguageService languageService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _languageService = languageService;
        }

        public ShippingRateComputationMethodType ShippingRateComputationMethodType {
            get { return ShippingRateComputationMethodType.Realtime; }
        }

        public IShipmentTracker ShipmentTracker {
            get { return null; }
        }

        public Task<decimal?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult(default(decimal?));
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "";
        }

        public async Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            response.ShippingOptions.Add(new ShippingOption() {
                Name = _localizationService.GetResource("Plugins.Shipping.Rodonaves.PluginName"),
                Description = _localizationService.GetResource("Plugins.Shipping.Rodonaves.PluginDescription"),
                Rate = 10.0M,
                ShippingRateComputationMethodSystemName = "Shipping.Rodonaves",
            });

            return await Task.FromResult(response);
        }

        public Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public async Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {
            return await Task.FromResult(new List<string>());
        }

        public override async Task Install()
        {
            // settings
            await _settingService.SaveSetting(new RodonavesSettings());

            // locales
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginName", "Envio via Rodonaves");
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginDescriptor", "Valor de frete via Rodonaves");

            await base.Install();
        }

        public override async Task Uninstall()
        {
            // settings
            await _settingService.DeleteSetting<RodonavesSettings>();

            // locales
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginName");
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginDescriptor");

            await base.Uninstall();
        }

    }
}
