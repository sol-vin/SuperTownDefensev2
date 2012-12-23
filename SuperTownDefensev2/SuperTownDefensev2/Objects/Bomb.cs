using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
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

        //Data
        public bool IsExploding { get; private set; }

        private float _gravity;
        private int _damage;

        public Bomb(EntityState es, string name, XmlParser xp)
            : base(es, name)
        {
            string path = es.Name + "->" + "Bomb";

            Body = new Body(this, "Body");
            AddComponent(Body);

            Physics = new Physics(this, "Physics");
            AddComponent(Physics);

            Collision = new Collision(this, "Collision");
            AddComponent(Collision);

            ImageRender = new ImageRender(this, "ImageRender");
            AddComponent(ImageRender);

            _explodeanim = new Animation(this, "ExplodeAnim");
            _explodeanim.LastFrameEvent += Destroy;
            AddComponent(_explodeanim);

            _explodesound = new Sound(this, "ExplodeSound");
            AddComponent(_explodesound);

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
                }
            }
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            _gravity = xp.GetFloat(path + "->Gravity", .1f);
            _damage = xp.GetInt(path + "->Damage", 1);
        }
    }
}