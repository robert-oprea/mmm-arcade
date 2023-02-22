using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

using GXPEngine;

class GameManager : AnimationSprite
{
    Random rnd = new Random();

    List<TiledObject> enemyTemplates = new List<TiledObject>();

    List<TiledObject> powerUpTemplates = new List<TiledObject>();

    Player player;

    //scrolling the level
    float baseScrollSpeed; //the scrolling speed
    float scrollSpeedIncrease; //by how much the scrolling speed increases
    float scrollSpeedIncreaseTimer; //how often the scrolling speed increases
    float lastSpeedIncrease; //the last time the scrolling speed increased
    float maxScrollSpeed; // the max speed at which the level can be scrolled

    //spawning enemies
    float baseEnemySpawnSpeed; //how often enemies spawn
    float enemySpawnSpeedIncrease; //by how much their spawn speed increases
    float enemySpawnSpeedIncreaseTimer; // how often their spawn speed increases
    float lastSpawnedEnemy; //when the last enemy was spawned
    float lastEnemySpawnSpeedIncrease; //when the last time the spawn speed increased
    float maxEnemySpawnSpeed; //the maximum speed at which enemies can be spawned

    //spawning powerups
    float powerupSpawnSpeed = 100;
    float lastSpawnedPowerup;

    int levelLength;

    public GameManager(TiledObject pGameManagerObj, List<TiledObject> enemies, List<TiledObject> powertUps, Player pPlayer) : base("square.png", 1, 1, -1, false, false)
    {
        Initialize(pGameManagerObj);

        enemyTemplates = enemies;

        powerUpTemplates = powertUps;

        player = pPlayer;

    }

    void Initialize(TiledObject obj)
    {
        baseScrollSpeed = obj.GetFloatProperty("baseScrollSpeed", 1.0f);
        scrollSpeedIncrease = obj.GetFloatProperty("scrollSpeedIncrease", 0.1f);
        scrollSpeedIncreaseTimer = obj.GetFloatProperty("scrollSpeedIncreaseTimer", 1f) * 1000;
        maxScrollSpeed = obj.GetFloatProperty("maxScrollSpeed", 100f);

        baseEnemySpawnSpeed = obj.GetFloatProperty("baseEnemySpawnSpeed", 1f) * 1000;
        enemySpawnSpeedIncrease = obj.GetFloatProperty("enemySpawnSpeedIncrease", 100f) * 1000;
        enemySpawnSpeedIncreaseTimer = obj.GetFloatProperty("enemySpawnSpeedIncreaseTimer", 1f) * 1000;
        maxEnemySpawnSpeed = obj.GetFloatProperty("maxEnemySpawnSpeed", 1f) * 1000;

        powerupSpawnSpeed = obj.GetFloatProperty("powerupSpawnSpeed", 1f) * 1000;

        levelLength = obj.GetIntProperty("levelLength", 16) * 32;

        alpha = 0;
    }

    void Update()
    {
        if (parent != null)
        {
            ScrollLevel();

            SpawnEnemy();

            SpawnPowerup();
        }
    }

    void ScrollLevel()
    {
        foreach (GameObject child in parent.GetChildren())
        {
            if (child is Tiles)
            {
                child.x -= baseScrollSpeed;

                if (child.x + 32 < x)
                {
                    child.x += levelLength;
                }
            }

            if (child is Enemy || child is Powerup)
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

    void SpawnPowerup()
    {
        if (Time.time > lastSpawnedPowerup + powerupSpawnSpeed)
        {
            int randomIndex = rnd.Next(0, powerUpTemplates.Count);
            TiledObject randomTemplate = powerUpTemplates[randomIndex];

            Powerup powerUp;
            switch (randomTemplate.Name)
            {
                case "ExampleRapidFire":

                    powerUp = new RapidFire(randomTemplate);
                    
                    break;

                default:

                    powerUp = new RapidFire(powerUpTemplates[0]);

                    break;
            }

            powerUp.x = game.width * 1.2f;
            powerUp.y = rnd.Next(48, 464);

            parent.AddChild(powerUp);

            lastSpawnedPowerup = Time.time;
        }
    }

    void SpawnEnemy()
    {
        if (Time.time > lastSpawnedEnemy + baseEnemySpawnSpeed)
        {
            int randomIndex = rnd.Next(0, enemyTemplates.Count);
            TiledObject randomTemplate = enemyTemplates[randomIndex];

            Enemy enemy;

            switch (randomTemplate.Name)
            {
                case "ExampleBombEnemy":
                    enemy = new BombEnemy(randomTemplate);
                    break;
                case "ExampleGhostEnemy":
                    enemy = new GhostEnemy(randomTemplate);
                    break;
                case "ExampleSnakeEnemy":
                    enemy = new SnakeEnemy(randomTemplate);
                    break;
                case "ExampleShieldEnemy":
                    enemy = new ShieldEnemy(randomTemplate);
                    break;
                default:
                    // if the template name doesn't match any known type, use the default
                    enemy = new GhostEnemy(enemyTemplates[0]);
                    break;
            }

            enemy.x = game.width * 1.2f;
            enemy.y = rnd.Next(48, 464);

            enemy.SetTarget(player);

            parent.AddChild(enemy);

            lastSpawnedEnemy = Time.time;
        }

        if (Time.time > lastEnemySpawnSpeedIncrease + enemySpawnSpeedIncreaseTimer && baseEnemySpawnSpeed > maxEnemySpawnSpeed)
        {
            baseEnemySpawnSpeed -= enemySpawnSpeedIncrease;
            lastEnemySpawnSpeedIncrease = Time.time;
        }
    }
}

