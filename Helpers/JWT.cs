namespace Task_Mangment_Api.Helpers
{
    public class JWT
    {
        public double Duration { get; set; }
        public string Secret { get; set; }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
    }
}
