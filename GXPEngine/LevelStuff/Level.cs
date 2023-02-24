using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledMapParser;

namespace GXPEngine
{
    class Level : GameObject
    {
        private string Tileset = "Levels/TileSets/tileset.png"; // <---- Add your tileset image right here (there must be no dead space inbetween the tiles)

        private int TilesetCollumbs;
        private int TilesetRows;

        Player player;

        List<TiledObject> enemyTemplates = new List<TiledObject>();

        List<TiledObject> powerUpTemplates = new List<TiledObject>();

        TiledObject gameManagerObj;
        GameManager gameManager;

        string levelName;

        public Level(string filename)
        {
            Map leveldata = MapParser.ReadMap(filename);
            levelName = filename;
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

                            if (leveldata.Layers[i].Name == "Collidable") // makes all the tiles in the background layer have the name "Background" so you can check if its in the background
                            {
                                tile.name = "Collidable";
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
                        player = new Player("player_sprites.png", 4, 4, obj);

                        AddChild(player);
                        break;

                    case "ExampleBombEnemy": // if the objects Name is equal to ExampleBombEnemy it creates the template bomb enemy

                        enemyTemplates.Add(obj);

                        break;

                    case "ExampleSnakeEnemy": // if the objects Name is equal to ExampleSnakeEnemy it creates the template snake enemy

                        enemyTemplates.Add(obj);

                        break;

                    case "ExampleGhostEnemy": // if the objects Name is equal to ExampleGhostEnemy it creates the template ghost enemy

                        enemyTemplates.Add(obj);

                        break;

                    case "ExampleShieldEnemy":

                        enemyTemplates.Add(obj);

                        break;
                    
                    case "ExampleBatEnemy":

                        enemyTemplates.Add(obj);

                        break;

                    case "ExampleRapidFire":

                        powerUpTemplates.Add(obj);

                        break;

                    case "ExampleHealthPot":
                        
                        powerUpTemplates.Add(obj);

                        break;

                    case "ExampleInvincibility":

                        powerUpTemplates.Add(obj);

                        break;

                    case "GameManager":

                        gameManagerObj = obj;

                        break;

                    case "pressStart":

                        startScreen startScreen = new startScreen(obj);

                        AddChild(startScreen);
                        
                        break;
                }
            }
        }

        void Update()
        {
            if (player != null && gameManager == null && levelName == "Levels/Placeholder.tmx")
            {
                gameManager = new GameManager(gameManagerObj, enemyTemplates, powerUpTemplates, player);

                AddChild(gameManager);
            }
        }
    }
}

