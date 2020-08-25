using Grand.Plugin.Shipping.Rodonaves.Domain;
using Grand.Services.Catalog;
using Grand.Services.Directory;
using Grand.Services.Shipping;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grand.Plugin.Shipping.Rodonaves.Services
{
    public class RodonavesService : IRodonavesService
    {
        private readonly string BASE_API_URI = "https://01wapi.rte.com.br";
        private readonly string AUTH_ENDPOINT = "/token";
        private readonly string SIMULATE_QUOTATION_ENDPOINT = "/api/v1/simula-cotacao";
        private readonly string CITY_BY_ZIPCODE_ENDPOINT = "/api/v1/busca-por-cep";

        private readonly string RODONAVES_CURRENCY_CODE = "BRL";

        private readonly IShippingService _shippingService;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly HttpClient _client;
        private readonly RodonavesSettings _rodonavesSettings;
        private AuthModel _authModel;

        public RodonavesService(
            RodonavesSettings rodonavesSettings,
            IShippingService shippingService,
            IProductService productService,
            ICurrencyService currencyService)
        {
            _rodonavesSettings = rodonavesSettings;
            _shippingService = shippingService;
            _productService = productService;
            _currencyService = currencyService;

            _authModel = null;
            _client = new HttpClient { BaseAddress = new Uri(BASE_API_URI) };
            _client.DefaultRequestHeaders.Add("Host", Regex.Replace(BASE_API_URI, @"https?://", ""));
        }

        public async Task<decimal> GetRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            await Auth();
            var reqModel = await BuildQuoteSimulationRequest(getShippingOptionRequest);
            var serializedModel = JsonConvert.SerializeObject(reqModel);
            var requestBody = new StringContent(serializedModel, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync(
                SIMULATE_QUOTATION_ENDPOINT,
                requestBody);
            httpResponse.EnsureSuccessStatusCode();

            var quoteResponse = JsonConvert.DeserializeObject<QuoteSimulationModel>(await httpResponse.Content.ReadAsStringAsync());

            return await GetConvertedRateFromRodonavesToPrimaryCurrency(quoteResponse.Value);
        }

        public async Task<CityModel> CityByZipCode(string zipCode)
        {
            await Auth();
            var fields = new Dictionary<string, string>() {
                { "zipCode", ExtractNumbers(zipCode) },
            };
            var req = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString(CITY_BY_ZIPCODE_ENDPOINT, fields));
            var res = await _client.SendAsync(req);
            res.EnsureSuccessStatusCode();

            var city = JsonConvert.DeserializeObject<CityModel>(await res.Content.ReadAsStringAsync());
            return city;
        }

        private async Task Auth()
        {
            if (_authModel != null) return;

            var fields = new Dictionary<string, string>() {
                {"auth_type", "dev"},
                {"grant_type", "password"},
                {"username", _rodonavesSettings.ApiUsername},
                {"password", _rodonavesSettings.ApiPassword},
            };
            var req = new HttpRequestMessage(HttpMethod.Post, AUTH_ENDPOINT) { Content = new FormUrlEncodedContent(fields) };
            var res = await _client.SendAsync(req);
            res.EnsureSuccessStatusCode();

            _authModel = JsonConvert.DeserializeObject<AuthModel>(await res.Content.ReadAsStringAsync());
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authModel.Access_token);
        }

        private async Task<QuoteSimulationRequest> BuildQuoteSimulationRequest(GetShippingOptionRequest shippingOptionRequest)
        {
            var originCityTask = CityByZipCode(shippingOptionRequest.ZipPostalCodeFrom);
            var destinationCityTask = CityByZipCode(shippingOptionRequest.ShippingAddress.ZipPostalCode);

            var totalWeight = await _shippingService.GetTotalWeight(shippingOptionRequest);
            var totalValue = await GetDeclaredValueAsync(shippingOptionRequest);
            var customerTaxIdRegistration = ExtractNumbers(_rodonavesSettings.CustomerTaxId);

            var originCity = await originCityTask;
            var destinationCity = await destinationCityTask;

            var quoteSimulationRequest = new QuoteSimulationRequest() {
                OriginCityId = originCity.CityId,
                OriginZipCode = originCity.ZipCode,
                DestinationCityId = destinationCity.CityId,
                DestinationZipCode = destinationCity.ZipCode,
                TotalWeight = totalWeight,
                CustomerTaxIdRegistration = customerTaxIdRegistration,
                EletronicInvoiceValue = totalValue,
                Packs = new List<Pack>(),
            };
            return quoteSimulationRequest;
        }

        private string ExtractNumbers(string value)
        {
            return Regex.Replace(value, @"[^\d]", "");
        }

        private async Task<decimal> GetDeclaredValueAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            var productIds = shippingOptionRequest.Items.Select(x => x.ShoppingCartItem.ProductId);
            var products = await _productService.GetProductsByIds(productIds.ToArray());
            var declaredValue = await GetConvertedRateFromPrimaryToRodonavesCurrency(products.Sum(x => x.Price));
            return declaredValue;
        }

        private async Task<decimal> GetConvertedRateFromPrimaryToRodonavesCurrency(decimal value)
        {
            return await _currencyService.ConvertFromPrimaryStoreCurrency(value, await _currencyService.GetCurrencyByCode(RODONAVES_CURRENCY_CODE));
        }

        private async Task<decimal> GetConvertedRateFromRodonavesToPrimaryCurrency(decimal value)
        {
            return await _currencyService.ConvertToPrimaryStoreCurrency(value, await _currencyService.GetCurrencyByCode(RODONAVES_CURRENCY_CODE));
        }
    }
}
