using Grand.Core;
using Grand.Core.Domain.Customers;
using Grand.Plugin.Shipping.Rodonaves.Domain;
using Grand.Services.Customers;
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
using System.Xml.Linq;

namespace Grand.Plugin.Shipping.Rodonaves.Services
{
    public class RodonavesService : IRodonavesService
    {
        private static readonly string BASE_API_URI = "https://01wapi.rte.com.br";
        private static readonly string AUTH_ENDPOINT = "/token";
        private static readonly string SIMULATE_QUOTATION_ENDPOINT = "/api/v1/simula-cotacao";
        private static readonly string CITY_BY_ZIPCODE_ENDPOINT = "/api/v1/busca-por-cep";

        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IWorkContext _workContext;
        private readonly IShippingService _shippingService;
        private readonly HttpClient _client;
        private readonly RodonavesSettings _rodonavesSettings;
        private AuthModel _authModel;

        public RodonavesService(
            RodonavesSettings rodonavesSettings,
            IShippingService shippingService,
            IWorkContext workContext,
            ICustomerAttributeParser customerAttributeParser)
        {
            _rodonavesSettings = rodonavesSettings;
            _shippingService = shippingService;
            _workContext = workContext;
            _customerAttributeParser = customerAttributeParser;

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

            if (!httpResponse.IsSuccessStatusCode)
                throw new Exception("Error simulating quotation at RTE Rodonaves. See logs for details.");

            var quoteResponse = JsonConvert.DeserializeObject<QuoteSimulationModel>(await httpResponse.Content.ReadAsStringAsync());

            return quoteResponse.Value;
        }

        public async Task Auth()
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

            if (!res.IsSuccessStatusCode)
                throw new Exception("Error simulating quotation at RTE Rodonaves. See logs for details.");

            _authModel = JsonConvert.DeserializeObject<AuthModel>(await res.Content.ReadAsStringAsync());
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authModel.Access_token);
        }

        public async Task<CityModel> CityByZipCode(string zipCode)
        {
            await Auth();
            var fields = new Dictionary<string, string>() {
                { "zipCode", zipCode },
            };
            var req = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString(CITY_BY_ZIPCODE_ENDPOINT, fields));
            var res = await _client.SendAsync(req);

            if (!res.IsSuccessStatusCode)
                throw new Exception("Error simulating quotation at RTE Rodonaves. See logs for details.");

            var city = JsonConvert.DeserializeObject<CityModel>(await res.Content.ReadAsStringAsync());
            return city;
        }

        private async Task<QuoteSimulationRequest> BuildQuoteSimulationRequest(GetShippingOptionRequest getShippingOptionRequest)
        {
            var originZipCode = ExtractNumbers(getShippingOptionRequest.ZipPostalCodeFrom);
            var destinationZipCode = ExtractNumbers(getShippingOptionRequest.ShippingAddress.ZipPostalCode);
            var originCityTask = await CityByZipCode(originZipCode);
            var destinationCityTask = await CityByZipCode(destinationZipCode);

            var totalWeight = await _shippingService.GetTotalWeight(getShippingOptionRequest);
            var totalValue = 200.0M;
            var customerTaxIdRegistration = await CustomerCnpjValue(_workContext.CurrentCustomer);

            var originCity = originCityTask;
            var destinationCity = destinationCityTask;

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

        private async Task<string> CustomerCnpjValue(Customer customer)
        {
            var cnpjValue = "";
            var customCustomerAttributes = customer.GenericAttributes.FirstOrDefault(x => x.Key.Equals("CustomCustomerAttributes"));
            if (customCustomerAttributes != null && !string.IsNullOrEmpty(customCustomerAttributes.Value))
            {
                var cnpjCustomerAttribute = (await _customerAttributeParser.ParseCustomerAttributes(customCustomerAttributes.Value)).FirstOrDefault(x => x.Name == "CNPJ");

                XDocument parsedXml = XDocument.Parse(customCustomerAttributes.Value);
                var elements = parsedXml.Root.Elements("CustomerAttribute");
                var cnpjAttribute = elements.FirstOrDefault(x => x.Attribute("ID").Value == cnpjCustomerAttribute.Id);
                cnpjValue = !(cnpjAttribute is null) ? cnpjAttribute.Element("CustomerAttributeValue").Element("Value").Value : "";
            }
            return ExtractNumbers(cnpjValue);
        }
        private string ExtractNumbers(string value)
        {
            return Regex.Replace(value, @"[^\d]", "");
        }
    }
}
