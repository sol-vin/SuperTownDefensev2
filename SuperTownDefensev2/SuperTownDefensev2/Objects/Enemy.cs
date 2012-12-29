using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using SuperTownDefensev2.States.Game;

namespace SuperTownDefensev2.Objects
{
    public class Enemy : Entity
    {
        public int Points;
        public List<Entity> Enemies;
        public bool KilledByPlayer;

        public Enemy(EntityState es, string name)
            : base(es, name)
        {
            Enemies = new List<Entity>();
        }

        public override void Destroy(Entity e = null)
        {
            base.Destroy(e);
            if (KilledByPlayer)
                GameState.Score += Points;
        }

        public void RemoveEnemy(Entity e)
        {
            Enemies.Remove(e);
        }

        public void AddEnemy(Entity e)
        {
            Enemies.Add(e);
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            Points = xp.GetInt(path + "->Points", 0);
        }
    }
}
