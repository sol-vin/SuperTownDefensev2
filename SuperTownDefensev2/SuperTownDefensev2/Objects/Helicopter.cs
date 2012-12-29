using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefensev2.Objects
{
    public sealed class Helicopter : Enemy
    {
        //Components
        public Body Body;
        public Physics Physics;
        public Animation Animation;
        public Collision Collision;
        public Health Health;
        //TODO: Add helicopter gib emitter
        //TODO: Add Explode animation

        //Data
        private readonly Random _rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
        private bool _hasdroppedbomb;
        private readonly XmlParser _xp;

        public Helicopter(EntityState es, string name, XmlParser xp) : base(es, name)
        {
            Name = name + ID;
            _xp = xp;

            Body = new Body(this, "Body");
            AddComponent(Body);

            Physics = new Physics(this, "Physics");
            AddComponent(Physics);

            Animation = new Animation(this, "Animation");
            AddComponent(Animation);

            Collision = new Collision(this, "Collision");
            AddComponent(Collision);

            Health = new Health(this, "Health");
            Health.DiedEvent += OnDeath;
            AddComponent(Health);

            string path = es.Name + "->Helicopter";
            ParseXml(xp, path);

            Animation.Flip = (_rand.RandomBool()) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Physics.Velocity.X = (Animation.Flip == SpriteEffects.None) ? -.4f : .4f;
            Body.Position.Y = 300 - _rand.Next(-10, 10);
            Body.Position.X = (Animation.Flip == SpriteEffects.None) ? es.GameRef.Viewport.Right + 10 : -10;
        }

        public void OnDeath(Entity e)
        {
            KilledByPlayer = true;
            Destroy();
        }

        public override void Update()
        {
            base.Update();
            if (!_hasdroppedbomb && Animation.DrawRect.Right > StateRef.GameRef.Viewport.Width / 2 - 25 &&
                Animation.DrawRect.Left < StateRef.GameRef.Viewport.Width / 2 + 25)
            {
                _hasdroppedbomb = true;
                Bomb b = new Bomb(StateRef, "Bomb", _xp);
                b.Body.Position = Body.Position + Animation.Origin*Animation.Scale;
                b.Body.Angle = MathHelper.Pi;
                b.Collision.Partners = Enemies;
                b.Damage = .5f;
                AddEntity(b);
            }
        }
    }
}
