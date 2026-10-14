namespace Quark.CMS.Business.Models.Responses.NewsPhotoTypeResponses
{
    public class ListResponseItem
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsEnabled { get; set; }
        public string IsEnabledText { get; set; }
        public bool IsRequired { get; set; }
        public string IsRequiredText { get; set; }
        public int Order { get; set; }
    }
}
