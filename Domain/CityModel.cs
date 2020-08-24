namespace Grand.Plugin.Shipping.Rodonaves.Domain
{
    /// <summary>
    /// Represents a City response
    /// </summary>
    public class CityModel
    {
        public int CityId { get; set; }
        public string CityDescription { get; set; }
        public UnitFederation UnitFederation { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string ZipCode { get; set; }
        public object Number { get; set; }
        public object Supplement { get; set; }
        public string SitLoc { get; set; }
        public bool SectorAttended { get; set; }
        public bool CityAttended { get; set; }
        public object NotAttendedMessage { get; set; }
    }

    /// <summary>
    /// Represents a UnitFederation
    /// </summary>
    public class UnitFederation
    {
        public string Description { get; set; }
    }

}
