using System;
using System.IO;
using KruispuntGroep4.Globals;
using KruispuntGroep4.Main;
using KruispuntGroep4.Simulator.Communication;
using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.ObjectControllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KruispuntGroep4.Simulator.Main
{
    //TODO: 
    //Better Pathing
    //Bus stoplights
    //Bike detectiontile
    //Use vehicle textures
    //Add pedestrians

    //Bugs:

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Static ints for crossroad size
        public static int TilesHor = 22; //Total horizontal tiles
        public static int TilesVer = 20; //Total vertical tiles
        public static int MiddleSize = 6; // Lenght of the middle section (always square)
        public static int DetectionLoopLength = 4; //Number of tiles between the detection tiles
        public static int NrOfLanes = 8; //6 vehicle lanes and 2 sidewalks per direction
        public static int LaneLengthHor = (TilesHor - MiddleSize) / 2; //Number of tiles in horizontal lanes
        public static int LaneLengthVer = (TilesVer - MiddleSize) / 2; //Number of tiles in vertical lanes
        public static int TileTextureSize = 32; //32x32p textures

		private CommunicationForm communicationForm;

        private LevelBuilder levelBuilder;
        private LaneBuilder laneBuilder;
        private Audio audio;
        private Lists lists;

		private VehicleControl vehicleControl;
        private TileControl tileControl;
        private LaneControl laneControl;

        private MouseState mouseStateCurrent;
        private MouseState mouseStatePrevious;
        private Vector2 mousePosition;

        public MainGame(CommunicationForm communication)
        {
			this.communicationForm = communication;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = TilesHor * TileTextureSize;
            graphics.PreferredBackBufferHeight = TilesVer * TileTextureSize;

			Window.Title = Strings.TitleGame;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Textures.Default = Content.Load<Texture2D>("Sprites/Default32x32");

            lists = new Lists();

            levelBuilder = new LevelBuilder(lists);
            laneBuilder = new LaneBuilder(lists);

			vehicleControl = new VehicleControl(this.GraphicsDevice, lists, communicationForm);
            tileControl = new TileControl(lists);
            laneControl = new LaneControl(lists);

			

            audio = new Audio(Services);

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Textures
            Textures.RedLight = Content.Load<Texture2D>("Tiles/32p/RedLight32x32");
            Textures.GreenLight = Content.Load<Texture2D>("Tiles/32p/GreenLight32x32");
            Textures.YellowLight = Content.Load<Texture2D>("Tiles/32p/YellowLight32x32");
            Textures.BlinkLight = Content.Load<Texture2D>("Tiles/32p/BlinkLight32x32");

            Textures.Sidewalk2Red = Content.Load<Texture2D>("Tiles/32p/Sidewalk2Red32x32");
            Textures.Sidewalk2Green = Content.Load<Texture2D>("Tiles/32p/Sidewalk2Green32x32");
            Textures.SidewalkRightRed = Content.Load<Texture2D>("Tiles/32p/SidewalkRightRed32x32");
            Textures.SidewalkDownRed = Content.Load<Texture2D>("Tiles/32p/SidewalkDownRed32x32");

            Textures.Bikelane = Content.Load<Texture2D>("Tiles/32p/SortBike32x32");
            Textures.Buslane = Content.Load<Texture2D>("Tiles/32p/SortBus32x32");
            Textures.CarSortDown = Content.Load<Texture2D>("Tiles/32p/SortDown32x32");
            Textures.CarSortLeft = Content.Load<Texture2D>("Tiles/32p/SortLeft32x32");
            Textures.CarSortRight = Content.Load<Texture2D>("Tiles/32p/SortRight32x32");

            Textures.Road = Content.Load<Texture2D>("Tiles/32p/Road32x32");
            Textures.RoadCenter = Content.Load<Texture2D>("Tiles/32p/RoadCenter32x32");

            Textures.Crossing = Content.Load<Texture2D>("Tiles/32p/Crossing32x32");
            Textures.Grass = Content.Load<Texture2D>("Tiles/32p/Grass32x32");
            Textures.Sidewalk = Content.Load<Texture2D>("Tiles/32p/Sidewalk32x32");
            #endregion

            Textures.Car = Content.Load<Texture2D>("Sprites/Car32x32");
            Textures.Bus = Content.Load<Texture2D>("Sprites/Bus32x32");
            Textures.Truck = Content.Load<Texture2D>("Sprites/Truck32x32");
            Textures.Bike = Content.Load<Texture2D>("Sprites/Bike32x32");

            //Create the level
            this.LoadCrossroad("Content/Grids/Crossroad.txt");
            //Create the lanes
            laneBuilder.LoadLanes();

			// Send lane control to communicationForm form
			communicationForm.SetLaneControl(laneControl);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            
            MouseButtonPress();

            vehicleControl.Update(gameTime);
            laneControl.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            tileControl.Draw(gameTime, spriteBatch);
            vehicleControl.Draw(gameTime, spriteBatch);
            laneControl.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadCrossroad(string path)
        {
            if (File.Exists(path))
                levelBuilder.LoadLevel(path);
            else throw new Exception("No Level Detected");
        }

        private void MouseButtonPress()
        {
            mouseStateCurrent = Mouse.GetState();

            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
            {
                mousePosition = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                tileControl.CheckMouseCollision(mousePosition);
            }

            mouseStatePrevious = mouseStateCurrent;
        }
    }
}