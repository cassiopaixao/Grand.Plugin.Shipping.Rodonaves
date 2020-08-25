using Grand.Core;
using Grand.Core.Domain.Orders;
using Grand.Core.Domain.Shipping;
using Grand.Core.Plugins;
using Grand.Plugin.Shipping.Rodonaves.Services;
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
        private readonly IRodonavesService _rodonavesService;
        private readonly IWebHelper _webHelper;

        public RodonavesComputationMethod(
            ISettingService settingService,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IRodonavesService rodonavesService,
            IWebHelper webHelper)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _languageService = languageService;
            _rodonavesService = rodonavesService;
            _webHelper = webHelper;
        }

        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Realtime;

        public IShipmentTracker ShipmentTracker => null;

        public Task<decimal?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult(default(decimal?));
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "";
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ShippingRodonaves/Configure";
        }

        public Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public async Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {
            return await Task.FromResult(new List<string>());
        }

        public async Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest shippingOptionRequest)
        {
            if (shippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();
            try
            {
                var rate = await _rodonavesService.GetRate(shippingOptionRequest);

                response.ShippingOptions.Add(new ShippingOption() {
                    Name = _localizationService.GetResource("Plugins.Shipping.Rodonaves.PluginName"),
                    Description = _localizationService.GetResource("Plugins.Shipping.Rodonaves.PluginDescription"),
                    Rate = rate,
                    ShippingRateComputationMethodSystemName = "Shipping.Rodonaves",
                });
            }
            catch(Exception ex)
            {
                // Log message
            }
            
            return response;
        }

        public override async Task Install()
        {
            // settings
            await _settingService.SaveSetting(new RodonavesSettings());

            // locales
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginName", "RTE Rodonaves");
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginDescription", "Receba em seu endereço");
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.Fields.ApiUsername", "Username (API)");
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.Fields.ApiPassword", "Password (API)");
            await this.AddOrUpdatePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.Fields.CustomerTaxId", "CPF/CNPJ do usuário");

            await base.Install();
        }

        public override async Task Uninstall()
        {
            // settings
            await _settingService.DeleteSetting<RodonavesSettings>();

            // locales
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginName");
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.PluginDescription");
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.Fields.ApiUsername");
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.Fields.ApiPassword");
            await this.DeletePluginLocaleResource(_localizationService, _languageService, "Plugins.Shipping.Rodonaves.Fields.CustomerTaxId");

            await base.Uninstall();
        }

    }
}
