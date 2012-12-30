using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.Object;
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

        private GibEmitter _ge;
        private Animation _explodeanim;
        private Sound _hitsound;

        //Data
        private readonly Random _rand = new Random(DateTime.Now.Millisecond*DateTime.Now.Second);
        private bool _hasdroppedbomb;
        private readonly XmlParser _xp;

        public Helicopter(EntityState es, string name, XmlParser xp)
            : base(es, name)
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

            _ge = new GibEmitter(this, "GibEmitter");
            AddComponent(_ge);

            _explodeanim = new Animation(this, "ExplodeAnim");
            _explodeanim.LastFrameEvent += Destroy;
            AddComponent(_explodeanim);

            _hitsound = new Sound(this, "HitSound");
            AddComponent(_hitsound);

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
            _ge.Emit(10);
            _hitsound.Play();
            Animation.Active = false;
            Animation.Default = false;
            _explodeanim.Active = true;
            _explodeanim.Default = true;
            _explodeanim.Start();
        }

        public override void Update()
        {
            base.Update();
            if (Health.Alive && !_hasdroppedbomb && GetComponent<Render>().DrawRect.Right > StateRef.GameRef.Viewport.Width/2 - 25 &&
                GetComponent<Render>().DrawRect.Left < StateRef.GameRef.Viewport.Width/2 + 25)
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

    internal class GibEmitter : Emitter
    {
        private int _minttl, _maxttl;
        private Random _rand = new Random(DateTime.Now.Millisecond);

        public GibEmitter(Entity e, string name)
            : base(e, name)
        {
        }


        protected override Particle GenerateNewParticle()
        {
            int index = _rand.Next(0, 3);
            int ttl = _rand.Next(_minttl, _maxttl);

            var p = new GibParticle(index,
                                    Entity.GetComponent<Body>().Position +
                                    Entity.GetComponent<Render>().Origin*Entity.GetComponent<Render>().Scale, ttl, this);
            p.Body.Angle = (float) _rand.NextDouble()*MathHelper.TwoPi;
            p.Physics.Thrust(((float) _rand.NextDouble() + 1f)*2.5f);
            p.TileRender.Layer = .5f;
            p.TileRender.Scale = new Vector2(1 + (float) _rand.NextDouble() - .5f);
            return p;
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            string rootnode = path + "->" + Name;
            _minttl = xp.GetInt(rootnode + "->MinTTL");
            _maxttl = xp.GetInt(rootnode + "->MaxTTL");
        }

        private class GibParticle : Particle
        {
            public GibParticle(int index, Vector2 position, int ttl, Emitter e)
                : base(index, position, ttl, e)
            {
            }

            public override void Update()
            {
                base.Update();
                Physics.Velocity.Y += .1f;
            }
        }
    }
}

