namespace DapperDemo.Entities
{
    public class Sidekick
    {
        public int Id { get; set; }
        public string SidekickName { get; set; }
        public Hero Hero { get; set; }
    }
}