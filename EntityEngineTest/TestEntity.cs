using EntityEnginev2.Components;
using EntityEnginev2.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineTest
{
    public class TestEntity : Entity
    {
        public Body Body;
        public Body Body2;
        public Body Body3;
        public Physics Physics;
        public Collision Collision;

        public TestEntity(EntityState es, string name)
            : base(es, name)
        {
            Body = new Body(this, "Body", Vector2.One);
            AddComponent(Body);

            Body2 = new Body(this, "Body2", Vector2.One * 2);
            AddComponent(Body2);

            Body3 = new Body(this, "Body3", Vector2.One * 3);
            AddComponent(Body3);

            Physics = new Physics(this, "Physics");
            Physics.Drag = .9f;
            AddComponent(Physics);

            Collision = new Collision(this, "Collision");
            AddComponent(Collision);
        }
    }
}