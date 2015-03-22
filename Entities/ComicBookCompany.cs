namespace DapperDemo.Entities
{
    public class ComicBookCompany
    {
        // normally there would be only one Id field
        // but for demonstrating a different convention, I'm putting two of them in here
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
    }
}