using Grand.Core.Configuration;

namespace Grand.Plugin.Shipping.Rodonaves
{
    public class RodonavesSettings : ISettings
    {
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
    }
}
