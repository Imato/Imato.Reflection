using FastMember;

namespace Imato.Reflection
{
    public class Accessor
    {
        public string Name { get; set; } = null!;
        public TypeAccessor TypeAccessor { get; set; } = null!;
        public IEnumerable<Member> Members { get; set; } = null!;
    }
}