namespace api.Entities
{
    public class AppUser
    {
        public int id { get; set; }
        public string userName { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }

    }
}