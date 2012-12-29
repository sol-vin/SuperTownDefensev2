using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using EntityEnginev2.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperTownDefensev2.Objects;

namespace SuperTownDefensev2.States.Game
{
    public sealed class GameState : EntityState
    {
        public static int Score;
        public static float Difficulty = 1;

        private Image _bgimage;
        private bool _alreadystarted;
        private Town _town;
        private EnemySpawner _es;
        private Text _scoretext;
        private Text _healthtext;
        private Text _difficultytext;
        private bool _alreadyraiseddifficulty;

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

                _scoretext = new Text(this, "ScoreText");
                _scoretext.ParseXml(xp, Name + "->" + _scoretext.Name);
                _scoretext.Body.Position = new Vector2(
                    GameRef.Viewport.Width / 2 - _scoretext.TextRender.DrawRect.Width / 2, 10);
                AddEntity(_scoretext);

                _healthtext = new Text(this, "HealthText");
                _healthtext.ParseXml(xp, Name + "->" + _healthtext.Name);
                _healthtext.Body.Position = new Vector2(
                    GameRef.Viewport.Width / 2 - _healthtext.TextRender.DrawRect.Width / 2, 50);
                AddEntity(_healthtext);

                _difficultytext = new Text(this, "DifficultyText");
                _difficultytext.ParseXml(xp, Name + "->" + _difficultytext.Name);
                _difficultytext.Body.Position = new Vector2(
                    GameRef.Viewport.Width / 2 - _difficultytext.TextRender.DrawRect.Width / 2, 90);
                AddEntity(_difficultytext);

                _es = new EnemySpawner(this, xp);
                EntityRemoved += _es.RemoveEnemy;
                _es.Targets.Add(_town);
                AddEntity(_es);

                _town.Targets.List = _es.Enemies;
            }
        }

        public override void Show(string name)
        {
            base.Show(name);
            Start();
           
        }

        public override void Update()
        {
            base.Update();
            _scoretext.TextRender.Text = Score.ToString();
            _scoretext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _scoretext.TextRender.DrawRect.Width / 2, 10);

            _healthtext.TextRender.Text = ((int)_town.Health.HitPoints).ToString();
            _healthtext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _healthtext.TextRender.DrawRect.Width / 2, 50);

            _difficultytext.TextRender.Text = Difficulty.ToString() + " : " + _es.TotalEnemiesKilled.ToString(); 
            _difficultytext.Body.Position = new Vector2(GameRef.Viewport.Width / 2 - _difficultytext.TextRender.DrawRect.Width / 2, 90);

            
        }
    }
}