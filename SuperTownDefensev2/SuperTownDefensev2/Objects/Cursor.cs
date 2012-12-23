using System;
using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SuperTownDefensev2.Objects
{
    public sealed class Cursor : Entity
    {
        //Components
        public Body Body;

        public Physics Physics;
        public ImageRender ImageRender;

        //Data
        public float RotationAngle { get; private set; }

        private Town _town;
        private DoubleInput _aimleftkey;
        private DoubleInput _aimrightkey;
        private DoubleInput _quickaimkey;
        private float _angleconstraint;
        private Color _normalcolor;
        private Color _inactivecolor;
        private float _safetyangle;
        private float _rotationspeed;
        private float _quickaimmultiplier;

        public bool CanFire
        {
            get { return ImageRender.Color == _normalcolor; }
        }

        public Cursor(EntityState es, Town t, XmlParser xp)
            : base(es, "Cursor")
        {
            _town = t;

            //Current data path is GameState->Town->Cursor
            var path = es.Name + "->" + _town.Name + "->" + Name;

            Body = new Body(this, "Body");
            AddComponent(Body);

            Physics = new Physics(this, "Physics");
            AddComponent(Physics);

            ImageRender = new ImageRender(this, "ImageRender");
            AddComponent(ImageRender);

            _aimleftkey = new DoubleInput(this, "AimLeftKey", Keys.A, Buttons.DPadLeft, PlayerIndex.One);
            AddComponent(_aimleftkey);

            _aimrightkey = new DoubleInput(this, "AimRightKeys", Keys.D, Buttons.DPadRight, PlayerIndex.One);
            AddComponent(_aimrightkey);

            _quickaimkey = new DoubleInput(this, "QuickAimKey", Keys.LeftShift, Buttons.RightShoulder, PlayerIndex.One);
            AddComponent(_quickaimkey);

            ParseXml(xp, path);

            ImageRender.Origin = new Vector2(ImageRender.Texture.Width / 2f, ImageRender.Texture.Height / 2f);
            Body.Position = _town.Body.Position +
                            (_town.TileRender.Origin - Vector2.UnitY * 40 - ImageRender.Origin) *
                            ImageRender.Scale;
        }

        public override void Update()
        {
            base.Update();
            float anglespeedmodifer = _quickaimkey.Down() ? _quickaimmultiplier : 1;

            if (_aimleftkey.Down())
                RotationAngle -= _rotationspeed * anglespeedmodifer;
            else if (_aimrightkey.Down())
                RotationAngle += _rotationspeed * anglespeedmodifer;

            if (RotationAngle > _angleconstraint)
                RotationAngle = _angleconstraint;
            else if (RotationAngle < -_angleconstraint)
                RotationAngle = -_angleconstraint;

            if (Math.Abs(RotationAngle) <= _safetyangle)
            {
                ImageRender.Color = _inactivecolor;
                Body.Angle = MathHelper.PiOver2 / 2;
            }
            else
            {
                ImageRender.Color = _normalcolor;
                Body.Angle = 0;
            }

            var origin = _town.Body.Position + _town.TileRender.Origin * _town.TileRender.Scale;
            var unrotatedposition = _town.Body.Position + (_town.TileRender.Origin - Vector2.UnitY * 40 - ImageRender.Origin) * ImageRender.Scale;

            Body.Position = new Vector2(
                (float)(Math.Cos(RotationAngle) * (unrotatedposition.X - origin.X) - Math.Sin(RotationAngle) * (unrotatedposition.Y - origin.Y) + origin.X),
                (float)(Math.Sin(RotationAngle) * (unrotatedposition.X - origin.X) + Math.Cos(RotationAngle) * (unrotatedposition.Y - origin.Y) + origin.Y)
            );
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            _normalcolor = xp.GetColor(path + "->NormalColor");
            _inactivecolor = xp.GetColor(path + "->InactiveColor");
            _safetyangle = xp.GetFloat(path + "->SafetyAngle");
            _angleconstraint = xp.GetFloat(path + "->AngleConstraint");
            _rotationspeed = xp.GetFloat(path + "->RotationSpeed");
            _quickaimmultiplier = xp.GetFloat(path + "->QuickAimMultiplier");
        }
    }
}