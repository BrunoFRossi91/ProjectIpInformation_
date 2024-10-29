namespace ProjectIpInformation.Dtos
{
    public class IpInfoDto
    {
        public int Id { get; set; } 
        public string Ip { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
    }
}
