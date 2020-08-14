using Grand.Core.Configuration;

namespace Grand.Plugin.Shipping.Rodonaves
{
    public class RodonavesSettings : ISettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
