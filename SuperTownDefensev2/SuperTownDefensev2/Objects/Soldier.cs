using System;
using System.Collections.Generic;
using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperTownDefensev2.Objects
{
    public sealed class Soldier : Enemy
    {
        //Components
        public Body Body;

        public Physics Physics;
        public Animation Animation;
        public Collision Collision;
        public Health Health;

        private Sound _attacksound, _hitsound;
        private Timer _attacktimer;
        private GibEmitter _ge;

        //Data
        public bool IsAttacking { get; private set; }

        private Random _rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
        private float _speed;

        public Soldier(EntityState es, string name, XmlParser xp) : base(es, name)
        {
            Name = name + ID;

            Body = new Body(this, "Body");
            AddComponent(Body);

            Physics = new Physics(this, "Physics");
            AddComponent(Physics);

            Animation = new Animation(this, "Animation");
            Animation.Start();
            AddComponent(Animation);

            Collision = new Collision(this, "Collision");
            AddComponent(Collision);

            Health = new Health(this, "Health");
            Health.DiedEvent += OnDeath;
            AddComponent(Health);

            _attacktimer = new Timer(this, "AttackTimer");
            _attacktimer.LastEvent += OnAttackTimer;
            AddComponent(_attacktimer);

            _attacksound = new Sound(this, "AttackSound");
            AddComponent(_attacksound);

            _hitsound = new Sound(this, "HitSound");
            AddComponent(_hitsound);

            _ge = new GibEmitter(this, "GibEmitter");
            AddComponent(_ge);

            string path = es.Name + "->" + "Soldier";
            ParseXml(xp, path);

            Animation.Flip = (_rand.RandomBool()) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Physics.Velocity.X = (Animation.Flip == SpriteEffects.None) ? -_speed : _speed;
            Body.Position.X = (Animation.Flip == SpriteEffects.None) ? es.GameRef.Viewport.Right + 10 : -10;
            Body.Position.Y = 520 - _rand.Next(-10, 10);

            //TODO: Set origin
            //TODO: Set Health.DiedEvent to emit blood particles and Destroy
        }

        public override void Update()
        {
            base.Update();

            //Stop the soldier at a point around the city.
            if (!IsAttacking)
            {
                float leftstop = StateRef.GameRef.Viewport.Width / 2 - 60 - _rand.Next(0, 40);
                float rightstop = StateRef.GameRef.Viewport.Width / 2 + 60 + _rand.Next(0, 40);

                //If we are facing left
                if (Animation.Flip == SpriteEffects.None)
                {
                    if (Body.Position.X < rightstop)
                    {
                        Physics.Velocity = Vector2.Zero;
                        IsAttacking = true;
                    }
                }

                //If we are facing right
                else
                {
                    if (Body.Position.X > leftstop)
                    {
                        Physics.Velocity = Vector2.Zero;
                        IsAttacking = true;
                    }
                }
                if (IsAttacking)
                {
                    _attacktimer.Start();
                }
            }
        }

        public void OnAttackTimer()
        {
            foreach (var enemy in Enemies)
            {
                if (enemy.GetComponent<Health>().Alive)
                {
                    enemy.GetComponent<Health>().Hurt(1);
                    _attacksound.Pitch = (float)_rand.NextDouble() * _rand.Next(-1, 2);
                    _attacksound.Play();
                }
            }
        }

        public void OnDeath(Entity e = null)
        {
            KilledByPlayer = true;

            _ge.Emit(20);
            _hitsound.Play();
            Destroy();
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            _speed = xp.GetFloat(path + "->Speed", 1);
        }

        class GibEmitter : Emitter
        {
            private List<Color> colors;
            private int _minttl, _maxttl;
            private Random _rand = new Random(DateTime.Now.Millisecond);

            public GibEmitter(Entity e, string name)
                : base(e, name)
            {
            }


            protected override Particle GenerateNewParticle()
            {
                int index = 0;
                int ttl = _rand.Next(_minttl, _maxttl);

                var p = new GibParticle(index, Entity.GetComponent<Body>().Position + Entity.GetComponent<Render>().Origin * Entity.GetComponent<Render>().Scale, ttl, this);
                p.Body.Angle = (float)_rand.NextDouble() - .5f * 1.5f;
                p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
                p.TileRender.Layer = .5f;
                p.TileRender.Scale = new Vector2(2f);
                p.TileRender.Color = colors[_rand.Next(0, colors.Count)];
                return p;
            }

            public override void ParseXml(XmlParser xp, string path)
            {
                base.ParseXml(xp, path);
                string rootnode = path + "->" + Name;
                _minttl = xp.GetInt(rootnode + "->MinTTL");
                _maxttl = xp.GetInt(rootnode + "->MaxTTL");

                colors = new List<Color>()
                    {
                        xp.GetColor(rootnode + "->Colors->Color1"),
                        xp.GetColor(rootnode + "->Colors->Color2"),
                        xp.GetColor(rootnode + "->Colors->Color3"),
                        xp.GetColor(rootnode + "->Colors->Color4")
                    };
            }

            private class GibParticle : Particle
            {
                public GibParticle(int index, Vector2 position, int ttl, Emitter e) : base(index, position, ttl, e)
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
}