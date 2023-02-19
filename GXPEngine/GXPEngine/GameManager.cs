using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class GameManager : AnimationSprite
    {
        Random rnd = new Random();

        TiledObject exampleBombEnemy;
        TiledObject exampleSnakeEnemy;
        TiledObject exampleGhostEnemy;

        Player player;

        //scrolling the level
        float baseScrollSpeed; //the scrolling speed
        float scrollSpeedIncrease; //by how much the scrolling speed increases
        float scrollSpeedIncreaseTimer; //how often the scrolling speed increases
        float lastSpeedIncrease; //the last time the scrolling speed increased
        float maxScrollSpeed; // the max speed at which the level can be scrolled

        //spawning enemies
        float enemySpawnTimer; //how often enemies spawn
        float enemySpawnSpeedIncrease; //by how much their spawn speed increases
        float enemySpawnSpeedIncreaseTimer; // how often their spawn speed increases
        float lastSpawnedEnemy; //when the last enemy was spawned
        float lastEnemySpawnSpeedIncrease; //when the last time the spawn speed increased
        float maxEnemySpawnSpeed; //the maximum speed at which enemies can be spawned

        int levelLength;

        public GameManager(TiledObject obj, TiledObject pExampleGhostEnemy, TiledObject pExampleBombEnemy, TiledObject pExampleSnakeEnemy, Player pPlayer) : base("square.png", 1, 1, -1, false, false)
        {
            Initialize(obj);

            exampleGhostEnemy = pExampleGhostEnemy;
            exampleBombEnemy = pExampleBombEnemy;
            exampleSnakeEnemy = pExampleSnakeEnemy;
            player = pPlayer;
            
        }

        void Initialize(TiledObject obj)
        {
            baseScrollSpeed = obj.GetFloatProperty("baseScrollSpeed", 1.0f);
            scrollSpeedIncrease = obj.GetFloatProperty("scrollSpeedIncrease", 0.1f);
            scrollSpeedIncreaseTimer = obj.GetFloatProperty("scrollSpeedIncreaseTimer", 1f) * 1000;
            maxScrollSpeed = obj.GetFloatProperty("maxScrollSpeed", 100f);

            enemySpawnTimer = obj.GetFloatProperty("enemySpawnTimer", 1f) * 1000;
            enemySpawnSpeedIncrease = obj.GetFloatProperty("enemySpawnSpeedIncrease", 100f) * 1000;
            enemySpawnSpeedIncreaseTimer = obj.GetFloatProperty("enemySpawnSpeedIncreaseTimer", 1f) * 1000;
            maxEnemySpawnSpeed = obj.GetFloatProperty("maxEnemySpawnSpeed", 1f) * 1000;

            levelLength = obj.GetIntProperty("levelLength", 16);

            alpha = 0;
        }

        void Update()
        {
            if (parent != null)
            {
                ScrollLevel();

                SpawnEnemy();
            }
        }
        
        void ScrollLevel()
        {
            foreach (GameObject child in parent.GetChildren())
            {
                if (child is Tiles)
                {
                    child.x -= baseScrollSpeed;

                    if (child.x + 64 < x)
                    {
                        child.x += levelLength * 32;
                    }
                }

                if (child is Enemy)
                {
                    child.x -= baseScrollSpeed;

                    if (child.x + 64 < x)
                    {
                        child.LateDestroy();
                    }
                }
            }

            if (Time.time > lastSpeedIncrease + scrollSpeedIncreaseTimer && baseScrollSpeed < maxScrollSpeed)
            {
                baseScrollSpeed += scrollSpeedIncrease;
                lastSpeedIncrease = Time.time;
            }
        }

        void SpawnEnemy()
        {
            if (Time.time > lastSpawnedEnemy + enemySpawnTimer)
            {
                Enemy enemy;

                switch (rnd.Next(1, 4))
                {
                    case 1:
                        enemy = new BombEnemy(exampleBombEnemy);
                        break;
                    case 2:
                        enemy = new GhostEnemy(exampleGhostEnemy);
                        break;
                    case 3:
                        enemy = new SnakeEnemy(exampleSnakeEnemy);
                        break;
                    default:
                        enemy = new BombEnemy(exampleBombEnemy);
                        break;
                }

                enemy.x = 960;
                enemy.y = rnd.Next(48, 464);

                enemy.SetTarget(player);

                parent.AddChild(enemy);

                lastSpawnedEnemy = Time.time;

                if (Time.time > lastEnemySpawnSpeedIncrease + enemySpawnSpeedIncreaseTimer && enemySpawnTimer > maxEnemySpawnSpeed)
                {
                    enemySpawnTimer -= enemySpawnSpeedIncrease;
                    lastEnemySpawnSpeedIncrease = Time.time;
                }
            }
        }
    }
}
