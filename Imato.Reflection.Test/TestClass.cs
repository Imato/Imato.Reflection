using System;

namespace Imato.Reflection.Test
{
    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime Date { get; set; }
        public bool Flag { get; set; }
        public Nested? Test { get; set; }
    }
}