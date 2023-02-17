using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class Level : GameObject
    {
        private string Tileset = "Levels/TileSets/tileset.jpg"; // <---- Add your tileset image right here (there must be no dead space inbetween the tiles)

        private int TilesetCollumbs;
        private int TilesetRows;

        Player player;

        TiledObject exampleBombEnemy;
        TiledObject exampleSnakeEnemy;
        TiledObject exampleGhostEnemy;
        TiledObject gameManager;

        float baseScrollSpeed;
        float scrollSpeedIncrease;
        float scrollSpeedIncreaseTimer;
        float lastSpeedIncrease;
        float maxScrollSpeed;

        float lastSpawnedEnemy;
        float enemySpawnTimer;

        public Level(string filename)
        {
            Map leveldata = MapParser.ReadMap(filename);

            Sprite Tilesets = new Sprite(Tileset, false, false); // creates a sprite object to get the width and height of the total tileset
            TilesetRows = Tilesets.width / leveldata.TileWidth; // devides the width of the tilesets total width by that of the tiles
            TilesetCollumbs = Tilesets.height / leveldata.TileHeight; // ^ but for height

            SpawnTiles(leveldata);
            SpawnObjects(leveldata);
        }

        void SpawnTiles(Map leveldata)
        {
            if (leveldata.Layers == null || leveldata.Layers.Length == 0)
                return;
            for (int i = 0; i < leveldata.Layers.Length; i++) // loops through all the layers and creates Tiles
            {
                Layer mainlayer = leveldata.Layers[i];
                short[,] tileNumbers = mainlayer.GetTileArray();
                for (int row = 0; row < mainlayer.Height; row++)
                {
                    for (int col = 0; col < mainlayer.Width; col++)
                    {
                        int tileNumber = tileNumbers[col, row];
                        if (tileNumber > 0)
                        {
                            Tiles tile = new Tiles(Tileset, TilesetRows, TilesetCollumbs);
                            tile.SetFrame(tileNumber - 1);
                            tile.x = col * tile.width;
                            tile.y = row * tile.height;

                            if (leveldata.Layers[i].Name == "Background") // makes all the tiles in the background layer have the name "Background" so you can check if its in the background
                            {
                                tile.name = "Background";
                            }

                            AddChild(tile);

                        }
                    }
                }
            }

        }

        void SpawnObjects(Map leveldata) // checks for objects and their location
        {

            if (leveldata.ObjectGroups == null || leveldata.ObjectGroups.Length == 0)
                return;

            ObjectGroup objects = leveldata.ObjectGroups[0];
            if (objects.Objects == null)
                return;

            foreach (TiledObject obj in objects.Objects)
            {

                switch (obj.Name)
                {
                    case "Player": // if the objects Name is equal to Player it creates a player
                        player = new Player("cart.png", 1, 1, obj);
                        player.x = obj.X;
                        player.y = obj.Y;
                        player.scale = 0.8f;

                        AddChild(player);
                        break;

                    case "ExampleBombEnemy": // if the objects Name is equal to ExampleBombEnemy it creates the template bomb enemy
                        Enemy exampleBomb;
                        exampleBomb = new BombEnemy(obj);
                        exampleBomb.x = obj.X;
                        exampleBomb.y = obj.Y;

                        exampleBombEnemy = obj;

                        AddChild(exampleBomb);
                        break;

                    case "ExampleSnakeEnemy": // if the objects Name is equal to ExampleSnakeEnemy it creates the template snake enemy
                        Enemy exampleSnake;
                        exampleSnake = new SnakeEnemy(obj);
                        exampleSnake.x = obj.X;
                        exampleSnake.y = obj.Y;

                        exampleSnakeEnemy = obj;

                        AddChild(exampleSnake);
                        break;

                    case "ExampleGhostEnemy": // if the objects Name is equal to ExampleGhostEnemy it creates the template ghost enemy
                        Enemy exampleGhost;
                        exampleGhost = new GhostEnemy(obj);
                        exampleGhost.x = obj.X;
                        exampleGhost.y = obj.Y;

                        exampleGhostEnemy = obj;

                        AddChild(exampleGhost);
                        break;

                    case "GameManager": //I didn't wanna make another class for it... L
                        gameManager = obj;
                        baseScrollSpeed = gameManager.GetFloatProperty("baseScrollSpeed", 1.0f);
                        scrollSpeedIncrease = gameManager.GetFloatProperty("scrollSpeedIncrease", 0.1f);
                        scrollSpeedIncreaseTimer = gameManager.GetFloatProperty("scrollSpeedIncreaseTimer", 1000f);
                        maxScrollSpeed = gameManager.GetFloatProperty("maxScrollSpeed", 100f);

                        enemySpawnTimer = gameManager.GetFloatProperty("enemySpawnTimer", 1000f);
                        break;
                }
            }
        }

        void Update()
        {
            SpawnEnemy();

            ScrollLevel();
        }

        void ScrollLevel()
        {
            foreach (GameObject child in GetChildren())
            {
                if (child is Tiles)
                {
                    child.x -= baseScrollSpeed;

                    if (child.x + 64 < x)
                    {
                        child.x += game.width * 1.5f;
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

                switch (new Random().Next(1, 4))
                {
                    case 1: enemy = new BombEnemy(exampleBombEnemy);
                        break;
                    case 2: enemy = new GhostEnemy(exampleGhostEnemy);
                        break;
                    case 3: enemy = new SnakeEnemy(exampleSnakeEnemy);
                        break;
                    default: enemy = new BombEnemy(exampleBombEnemy);
                        break;
                }

                enemy.x = 960;
                enemy.y = new Random().Next(48, 464);

                enemy.SetTarget(player);

                AddChild(enemy);

                lastSpawnedEnemy = Time.time;
            }
        }
    }
}

