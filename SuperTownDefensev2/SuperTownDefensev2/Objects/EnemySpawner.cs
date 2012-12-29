using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEnginev2.Components;
using EntityEnginev2.Data;
using EntityEnginev2.Engine;
using SuperTownDefensev2.States.Game;

namespace SuperTownDefensev2.Objects
{
    public sealed class EnemySpawner : Entity
    {
        public List<Entity> Enemies;
        public List<Entity> Targets;

        public Timer SoldierTimer;
        public Timer HelicopterTimer;

        private bool _alreadyraiseddifficulty;

        private int _soldiermaxtime;
        private int _helicoptermaxtime;
        private int _soldiermintime;
        private int _helicoptermintime;

        private int  _difficultystep;
        private float _difficultyrise;

        public int TotalEnemiesKilled { get; private set; }

        private XmlParser _xp;
        

        public EnemySpawner(EntityState es, XmlParser xp)
            : base(es, "EnemySpawner")
        {
            Enemies = new List<Entity>();
            Targets = new List<Entity>();
            _xp = xp;

            SoldierTimer = new Timer(this, "SoldierTimer");
            SoldierTimer.LastEvent += AddSoldier;
            AddComponent(SoldierTimer);

            HelicopterTimer = new Timer(this, "HelicopterTimer");
            HelicopterTimer.LastEvent += AddHelicopter;
            AddComponent(HelicopterTimer);

            ParseXml(xp, "GameState->" + Name);
        }

        public override void Update()
        {
            base.Update();
            int currentsoldiertime = _soldiermaxtime - (int)(_soldiermintime * GameState.Difficulty * .1);
            int currenthelitime = _helicoptermaxtime - (int)(_helicoptermintime * GameState.Difficulty * .1);

            if (currentsoldiertime <= _soldiermintime)
                currentsoldiertime = _soldiermintime;


            if (currenthelitime <= _helicoptermintime)
                currenthelitime = _helicoptermintime;

            SoldierTimer.Milliseconds = currentsoldiertime;
            HelicopterTimer.Milliseconds = currenthelitime;

            if (TotalEnemiesKilled != 0 && TotalEnemiesKilled % _difficultystep == 0 && _alreadyraiseddifficulty == false)
            {
                GameState.Difficulty += _difficultyrise;
                _alreadyraiseddifficulty = true;
            }

            if (TotalEnemiesKilled % _difficultystep != 0)
            {
                _alreadyraiseddifficulty = false;
            }
        }

        public void AddSoldier()
        {
            Soldier s = new Soldier(StateRef, "Soldier", _xp);

            Enemies.Add(s);
            AddEntity(s);
            s.Enemies = Targets;

            foreach (var entity in Targets)
            {
                entity.DestroyEvent += s.RemoveEnemy;
            }
        }

        public void AddHelicopter()
        {
            Helicopter helicopter = new Helicopter(StateRef, "Helicopter", _xp);
            Enemies.Add(helicopter);
            AddEntity(helicopter);
            helicopter.Enemies = Targets;

            foreach (var entity in Targets)
            {
                entity.DestroyEvent += helicopter.RemoveEnemy;
            }
        }

        public void RemoveEnemy(Entity e)
        {
            if(Enemies.Contains(e))
            {
                Enemies.Remove(e);
                if ((e as Enemy).KilledByPlayer)
                    TotalEnemiesKilled++;
            }
        }

        public override void ParseXml(XmlParser xp, string path)
        {
            base.ParseXml(xp, path);
            _soldiermaxtime = xp.GetInt(path + "->SoldierMaxTime");
            _helicoptermaxtime = xp.GetInt(path + "->HelicopterMaxTime");
            _soldiermintime = xp.GetInt(path + "->SoldierMinTime");
            _helicoptermintime = xp.GetInt(path + "->HelicopterMinTime");

            _difficultyrise = xp.GetFloat(path + "->DifficultyRise");
            _difficultystep = xp.GetInt(path + "->DifficultyStep");
        }
    }
}
