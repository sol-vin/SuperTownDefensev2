using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.GUI;
using EntityEnginev2.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SuperTownDefensev2.States.Menu
{
    public sealed class MenuState : EntityState
    {
        private Image _bgimage;
        private Text _starttext;
        private DoubleInput _startkey;

        public MenuState(SuperTownGame stg)
            : base(stg, "MenuState")
        {
            Start();
            ChangeState += stg.GameState.Show;
        }

        public override void Start()
        {
            base.Start();
            var menuparser = new XmlParser(@"States/Menu/menu.xml");

            _bgimage = new Image(this, "BGImage");
            _bgimage.ParseXml(menuparser, "Menu->BGImage");
            AddEntity(_bgimage);

            _starttext = new Text(this, "StartText");
            _starttext.ParseXml(menuparser, "Menu->StartText");
            float x = GameRef.Viewport.Width / 2 - _starttext.TextRender.DrawRect.Width / 2;
            _starttext.Body.Position = new Vector2(x, 500);
            AddEntity(_starttext);

            _startkey = new DoubleInput(null, "StartKey", Keys.Enter, Buttons.Start, PlayerIndex.One);
        }

        public override void Update()
        {
            base.Update();
            if (_startkey.Down())
            {
                ChangeToState("GameState");
            }
        }
    }
}