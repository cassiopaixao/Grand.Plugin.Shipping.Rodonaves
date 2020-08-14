using Grand.Framework.Controllers;
using Grand.Framework.Mvc.Filters;
using Grand.Plugin.Shipping.Rodonaves.Models;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Plugin.Shipping.Rodonaves.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class ShippingRodonavesController : BaseShippingController
    {
        #region fields
        private readonly RodonavesSettings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region ctor
        public ShippingRodonavesController(RodonavesSettings settings, ISettingService settingService, ILocalizationService localizationService)
        {
            this._settings = settings;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }
        #endregion

        #region methods
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.Username = _settings.Username;
            model.Password = _settings.Password;

            return View("~/Plugins/Shipping.Rodonaves/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            model.Username = _settings.Username;
            model.Password = _settings.Password;
            _settingService.SaveSetting(_settings);
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        #endregion
    }
}
