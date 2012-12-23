using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.GUI;
using SuperTownDefensev2.Objects;

namespace SuperTownDefensev2.States.Game
{
    public sealed class GameState : EntityState
    {
        private Image _bgimage;
        private bool _alreadystarted;
        private Town _town;

        public GameState(EntityGame stg)
            : base(stg, "GameState")
        {
        }

        public override void Start()
        {
            if (!_alreadystarted)
            {
                base.Start();
                _alreadystarted = true;

                XmlParser xp = new XmlParser(@"States/Game/Game.xml");

                _bgimage = new Image(this, "BGImage");
                _bgimage.ParseXml(xp, Name + "->" + _bgimage.Name);
                AddEntity(_bgimage);

                _town = new Town(this, xp);
                AddEntity(_town);
            }
        }

        public override void Show(string name)
        {
            base.Show(name);
            Start();
        }
    }
}