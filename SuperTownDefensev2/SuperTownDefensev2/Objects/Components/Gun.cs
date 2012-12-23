using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using Microsoft.Xna.Framework;

namespace SuperTownDefensev2.Objects.Components
{
    public class Gun : Component
    {
        public float Thrust;
        private float _correctionangle;
        public int LastID { get; private set; }

        private XmlParser _xp;

        public Gun(Entity e, string name)
            : base(e, name)
        {
        }

        public void Fire(Vector2 position, float angle, Vector2 origin, float scale)
        {
            Bomb b = new Bomb(Entity.StateRef, "Bomb" + GetID(), _xp);
            b.Collision.Partners = Entity.GetComponent<Targets>().List;
            b.Body.Position = position + origin * scale - b.ImageRender.Origin * b.ImageRender.Scale.X;
            b.Body.Angle = angle - angle*_correctionangle;
            b.Physics.Thrust(Thrust);
            Entity.AddEntity(b);
        }

        public int GetID()
        {
            return LastID++;
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            _xp = xp;
            base.ParseXml(xp, path);
            string rootnode = path + "->" + Name;
            Thrust = xp.GetFloat(rootnode + "->Thrust", 8.5f);
            _correctionangle = xp.GetFloat(rootnode + "->CorrectionAngle", .17f);
        }
    }
}