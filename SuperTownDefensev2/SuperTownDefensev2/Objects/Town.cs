using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperTownDefensev2.Objects.Components;

namespace SuperTownDefensev2.Objects
{
    public sealed class Town : Entity
    {
        //Components
        public Body Body;

        public TileRender TileRender;
        public Animation DeadCityAnim;
        public Collision Collision;
        public Cursor Cursor;
        public Health Health;
        public Targets Targets;
        public Gun Gun;

        private Sound _firebombsound;

        //Data
        private int _rapidfire;

        private DoubleInput _firekey;
        private DoubleInput _debugkey;

        public Town(EntityState es, XmlParser xp)
            : base(es, "Town")
        {
            Body = new Body(this, "Body");
            AddComponent(Body);

            TileRender = new TileRender(this, "TileRender");
            AddComponent(TileRender);

            DeadCityAnim = new Animation(this, "DeadCityAnim");
            AddComponent(DeadCityAnim);

            Collision = new Collision(this, "Collision");
            AddComponent(Collision);

            Health = new Health(this, "Health");
            AddComponent(Health);

            Gun = new Gun(this, "Gun");
            AddComponent(Gun);

            Targets = new Targets(this, "Targets");
            AddComponent(Targets);

            _firebombsound = new Sound(this, "FireBombSound");
            AddComponent(_firebombsound);

            _firekey = new DoubleInput(this, "FireKey", Keys.Enter, Buttons.A, PlayerIndex.One);
            AddComponent(_firekey);

            _debugkey = new DoubleInput(this, "DebugKey", Keys.Tab, Buttons.B, PlayerIndex.One);
            AddComponent(_debugkey);

            ParseXml(xp, "GameState->" + Name);

            //Add our custom data here.
            Body.Position.X = StateRef.GameRef.Viewport.Width / 2 - TileRender.DrawRect.Width / 2;

            //Set our rotation origins
            TileRender.Origin = new Vector2(TileRender.TileSize.X / 2f, TileRender.TileSize.Y / 2f);
            DeadCityAnim.Origin = TileRender.Origin;

            //TODO: Health.Hurtevent changes color
            Cursor = new Cursor(es, this, xp);
            es.AddEntity(Cursor);
        }

        public override void Update()
        {
            base.Update();
            if (Cursor.CanFire && _firekey.RapidFire() && Health.Alive)
            {
                Gun.Fire(Body.Position, Cursor.RotationAngle, TileRender.Origin, TileRender.Scale.X);
            }
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
        }
    }
}