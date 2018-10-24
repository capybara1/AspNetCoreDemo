namespace AspNetCoreDemo.RestfulWebApiDemo.Resources
{
    public class Page
    {
        public int Offset { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public string Filter { get; set; }
    }
}
