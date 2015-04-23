using System.Collections.Generic;

namespace DapperDemo.Entities
{
    public class Starship
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CrewMember> CrewMembers { get; set; }
    }
}