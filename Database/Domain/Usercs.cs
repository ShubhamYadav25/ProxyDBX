namespace ProxyDBX.Database.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
