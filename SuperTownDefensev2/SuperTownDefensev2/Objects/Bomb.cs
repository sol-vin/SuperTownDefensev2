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
    public sealed class Bomb : Entity
    {
        //Components
        public Body Body;
        public Physics Physics;
        public Collision Collision;
        public  ImageRender ImageRender;

        private Animation _explodeanim;
        private Sound _explodesound;

        private ExplosionEmitter _explosionemitter;
        private SmokeEmitter _smokeemitter;

        //Data
        public bool IsExploding { get; private set; }

        private float _gravity;
        public float Damage;

        public Bomb(EntityState es, string name, XmlParser xp)
            : base(es, name)
        {
            string path = es.Name + "->" + "Bomb";

            Name = Name + ID;

            Body = new Body(this, "Body");
            AddComponent(Body);

            Physics = new Physics(this, "Physics");
            AddComponent(Physics);

            Collision = new Collision(this, "Collision");
            Collision.CollideEvent += CollisionHandler;
            AddComponent(Collision);

            ImageRender = new ImageRender(this, "ImageRender");
            AddComponent(ImageRender);

            _explodeanim = new Animation(this, "ExplodeAnim");
            _explodeanim.LastFrameEvent += Destroy;
            AddComponent(_explodeanim);

            _explodesound = new Sound(this, "ExplodeSound");
            AddComponent(_explodesound);

            _explosionemitter = new ExplosionEmitter(this);
            AddComponent(_explosionemitter);

            _smokeemitter = new SmokeEmitter(this);
            AddComponent(_smokeemitter);

            ParseXml(xp, path);

            //TODO: Hook up Collision.CollideEvent to a handler
            _explodeanim.Origin = new Vector2(_explodeanim.TileSize.X / 2.0f, _explodeanim.TileSize.Y / 2.0f);
            ImageRender.Origin = new Vector2(ImageRender.Texture.Width / 2f, ImageRender.Texture.Height / 2f);
        }

        public override void Update()
        {
            base.Update();
            if (!IsExploding)
            {
                Physics.Velocity.Y += _gravity;
                Physics.FaceVelocity();
                _smokeemitter.Emit(1);
                if (Body.Position.Y > 510)
                    IsExploding = true;
            }
            if (IsExploding)
            {
                Physics.Velocity = Vector2.Zero;
                if (!_explodeanim.Active)
                {
                    ImageRender.Active = false;
                    ImageRender.Default = false;
                    _explodeanim.Active = true;
                    _explodeanim.Default = true;
                    _explodeanim.Start();
                    Body.Position -= _explodeanim.Origin * _explodeanim.Scale;
                    Collision.Bounds.Width = (int)_explodeanim.TileSize.X;
                    Collision.Bounds.Height = (int)_explodeanim.TileSize.Y;
                    _explosionemitter.Emit(20);
                    _explodesound.Play();
                }
            }
        }

        public void CollisionHandler(Entity e)
        {
            e.GetComponent<Health>().Hurt(Damage);
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            _gravity = xp.GetFloat(path + "->Gravity", .1f);
            Damage = xp.GetFloat(path + "->Damage", 1);
        }

        class ExplosionEmitter : Emitter
        {
            Random _rand = new Random(DateTime.Now.Millisecond);
            public ExplosionEmitter(Entity e) : base(e, "ExplosionEmitter")
            {
            
            }

            protected override Particle GenerateNewParticle()
            {
                int index = _rand.Next(0, 3);
                int ttl = _rand.Next(50, 80);

                ExplosionParticle p = new ExplosionParticle(index, Entity.GetComponent<Body>().Position + Entity.GetComponent<Render>().Origin * Entity.GetComponent<Render>().Scale, ttl, this);
                p.Body.Angle = (float)_rand.NextDouble() - .5f*1.5f;
                p.Physics.Thrust(((float)_rand.NextDouble() + 1f) * 2.5f);
                p.TileRender.Layer = .5f;
                return p;
            }
        }

        class ExplosionParticle : FadeParticle
        {
        
            public ExplosionParticle(int index, Vector2 position, int ttl, Emitter e) : base(index,position,10,ttl,e)
            {
            
            }

            public override void Update()
            {
                base.Update();
                Physics.Velocity.Y += .1f;
            }
        }

        private class SmokeEmitter : Emitter
        {
            private Random _rand = new Random(DateTime.Now.Millisecond);
            private List<Color> Colors;

            public SmokeEmitter(Entity e)
                : base(e, "SmokeEmitter")
            {
                Colors = new List<Color>();
                Colors.Add(Color.Gray);
                Colors.Add(Color.DarkGray);
                Colors.Add(Color.LightGray);
                Colors.Add(Color.LightSlateGray);
                Colors.Add(Color.SlateGray);
            }

            protected override Particle GenerateNewParticle()
            {
                int index = _rand.Next(0, 3);
                int ttl = _rand.Next(40, 80);
                //Rotate the point based on the center of the sprite
                // p = unrotated point, o = rotation origin
                //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
                //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy

                var origin = Entity.GetComponent<Body>().Position + Entity.GetComponent<Render>().Origin * Entity.GetComponent<Render>().Scale;

                //var unrotatedposition = new Vector2(
                //    Entity.GetComponent<Render>().DrawRect.X + (Entity.GetComponent<Render>().DrawRect.Width/2f) * Entity.GetComponent<Render>().Scale.X,
                //    Entity.GetComponent<Render>().DrawRect.Bottom);

                var unrotatedposition = new Vector2(Entity.GetComponent<Body>().Position.X + (Entity.GetComponent<Render>().DrawRect.Width/2*Entity.GetComponent<Render>().Scale.X), Entity.GetComponent<Render>().DrawRect.Bottom);

                var angle = Entity.GetComponent<Body>().Angle;

                var position = new Vector2(
                    (float)
                    (Math.Cos(angle)*(unrotatedposition.X - origin.X) - Math.Sin(angle)*(unrotatedposition.Y - origin.Y) +
                     origin.X),
                    (float)
                    (Math.Sin(angle)*(unrotatedposition.X - origin.X) + Math.Cos(angle)*(unrotatedposition.Y - origin.Y) +
                     origin.Y)
                    );

                FadeParticle p = new FadeParticle(index, position, 10, ttl, this);
                p.Body.Angle = (float) _rand.NextDouble()/2 - .25f;
                p.Physics.Thrust((float) _rand.NextDouble() + .1f);
                p.TileRender.Layer = .5f;
                p.TileRender.Color = Colors[_rand.Next(0, Colors.Count)];
                return p;
            }
        }
    }
}