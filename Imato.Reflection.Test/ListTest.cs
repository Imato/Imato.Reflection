using System.Collections.Generic;

namespace Imato.Reflection.Test
{
    public class ListTest
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<Nested> List { get; set; } = new List<Nested>();
    }
}