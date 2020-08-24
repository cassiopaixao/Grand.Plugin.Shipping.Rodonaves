using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Shipping.Rodonaves.Domain
{
    /// <summary>
    /// Represents a shipping simulate quotation request
    /// </summary>
    internal class QuoteSimulationRequest
    {
        /// <summary>
        /// Origin's ZipCode / CEP. e.g.: 14010000.
        /// </summary>
        public string OriginZipCode { get; set; }

        /// <summary>
        /// Origin's CityId. To the get CityId, use the 'busca-cidade' method. e.g.: 8997
        /// </summary>
        public int OriginCityId { get; set; }

        /// <summary>
        /// Destination's ZipCode / CEP. e.g.: 14010000.
        /// </summary>
        public string DestinationZipCode { get; set; }

        /// <summary>
        /// Destination's CityId. To the get CityId, use the 'busca-cidade' method. e.g.: 8997
        /// </summary>
        public int DestinationCityId { get; set; }

        /// <summary>
        /// Package total weight (in kilos). e.g.: 1.0
        /// </summary>
        public decimal TotalWeight { get; set; }

        /// <summary>
        /// NF-e invoice value (in BRL). e.g..: 10.50
        /// </summary>
        public decimal EletronicInvoiceValue { get; set; }

        /// <summary>
        /// TaxId (CPF/CNPJ) of the loggedin user. e.g.: 12345678900
        /// </summary>
        public string CustomerTaxIdRegistration { get; set; }

        /// <summary>
        /// List of packages. Use empty list when user doesn't know the packages' dimensions.
        /// </summary>
        public List<Pack> Packs { get; set; }
    }

    /// <summary>
    /// Represents a pack on shipping quote request
    /// </summary>
    internal class Pack
    {
        /// <summary>
        /// Total amount of packages
        /// </summary>
        public int AmountPackages { get; set; }

        /// <summary>
        /// Package's weight
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Package's length (in centimeters). eg: 50.0 (50cm), 147.0 (1.47m), 19.50 (19.5cm)
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Package's height (in centimeters). eg: 50.0 (50cm), 147.0 (1.47m), 19.50 (19.5cm)
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Package's width (in centimeters). eg: 50.0 (50cm), 147.0 (1.47m), 19.50 (19.5cm)
        /// </summary>
        public decimal Width { get; set; }

    }
}
