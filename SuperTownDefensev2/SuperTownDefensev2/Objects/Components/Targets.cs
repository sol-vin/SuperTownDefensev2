using System.Collections.Generic;
using EntityEnginev2.Engine;

namespace SuperTownDefensev2.Objects.Components
{
    public class Targets : Component
    {
        public List<Entity> List = new List<Entity>();

        public Targets(Entity entity, string name)
            : base(entity, name)
        {
        }
    }
}