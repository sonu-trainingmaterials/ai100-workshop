namespace ProcessingLibrary
{
    public class ImageInsights
    {
        public string ImageId { get; set; }
        public string Caption { get; set; }
        public string[] Tags { get; set; }
        public dynamic Adult { get; set; }
        public dynamic Faces { get; set; }
        public dynamic Categories { get; set; }
        public dynamic Color { get; set; }
    }

}
