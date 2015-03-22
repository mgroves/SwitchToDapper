namespace DapperDemo.Entities
{
    public class Hero
    {
        // normally there would be only one Id field
        // but for demonstrating a different convention, I'm putting two of them in here
        public int Id { get; set; }
        public int HeroId { get; set; }
        public string HeroName { get; set; }
        public ComicBookCompany ComicBookCompany { get; set; }
    }
}