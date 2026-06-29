namespace Chigen.Core.Models
{
    public class SpecimenInfo
    {
        public string Type { get; set; } = "Peripheral Blood";
        public string CollectionDate { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");
        public string ReceivedDate { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");
    }
}
