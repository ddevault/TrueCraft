using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class {{ csname }}Block : BlockProvider
    {
        public static readonly byte BlockID = {{ id }};
        
        public override byte ID { get { return {{ id }}; } }

        public override double Hardness { get { return {{ hardness }}; } }

        public override string DisplayName { get { return "{{ display_name }}"; } }{% if tex_x %}

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>({{ tex_x }}, {{ tex_y }});
        }{% endif %}
    }
}
