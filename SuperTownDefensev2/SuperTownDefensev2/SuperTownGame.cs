using EntityEnginev2.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperTownDefensev2.States.Game;
using SuperTownDefensev2.States.Menu;

namespace SuperTownDefensev2
{
    public class SuperTownGame : EntityGame
    {
        public MenuState MenuState;
        public GameState GameState;

        public SuperTownGame(Game game, GraphicsDeviceManager g, SpriteBatch spriteBatch)
            : base(game, g, new Rectangle(0, 0, 600, 600), spriteBatch)
        {
            GameState = new GameState(this);
            MenuState = new MenuState(this);
            MenuState.Show();
        }
    }
}