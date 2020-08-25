using Grand.Framework.Controllers;
using Grand.Framework.Mvc.Filters;
using Grand.Framework.Security.Authorization;
using Grand.Plugin.Shipping.Rodonaves.Models;
using Grand.Services.Configuration;
using Grand.Services.Localization;
using Grand.Services.Security;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Grand.Plugin.Shipping.Rodonaves.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    [PermissionAuthorize(PermissionSystemName.ShippingSettings)]
    public class ShippingRodonavesController : BaseShippingController
    {
        #region fields
        private readonly RodonavesSettings _rodonavesSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region ctor
        public ShippingRodonavesController(
            RodonavesSettings rodonavesSettings,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._rodonavesSettings = rodonavesSettings;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }
        #endregion

        #region methods
        public IActionResult Configure()
        {
            var model = new RodonavesSettingsModel();
            model.ApiUsername = _rodonavesSettings.ApiUsername;
            model.ApiPassword = _rodonavesSettings.ApiPassword;
            model.CustomerTaxId = _rodonavesSettings.CustomerTaxId;

            return View("~/Plugins/Shipping.Rodonaves/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Configure(RodonavesSettingsModel model)
        {
            _rodonavesSettings.ApiUsername = model.ApiUsername;
            _rodonavesSettings.ApiPassword = model.ApiPassword;
            _rodonavesSettings.CustomerTaxId = model.CustomerTaxId;
            await _settingService.SaveSetting(_rodonavesSettings);
            await _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return await Task.FromResult(Configure());
        }
        #endregion
    }
}
