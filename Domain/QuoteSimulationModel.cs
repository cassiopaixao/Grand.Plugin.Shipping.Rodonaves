using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Shipping.Rodonaves.Domain
{
    /// <summary>
    /// Represents a quote simulation response
    /// </summary>
    public class QuoteSimulationModel
    {
        public decimal Value { get; set; }
        public int DeliveryTime { get; set; }
        public string ProtocolNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool Cubed { get; set; }
        public string Message { get; set; }
        public DateTime ExpirationDay { get; set; }
    }

}
