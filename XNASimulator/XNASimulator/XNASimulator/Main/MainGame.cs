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
    /// <summary>
    /// The heart of the simulator
    /// </summary>
    class MainGame : Game
    {
		// Appoint private attributes
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Static ints for crossroad size
        public static int TilesHor = 24; //Total horizontal tiles
        public static int TilesVer = 22; //Total vertical tiles
        public static int MiddleSize = 6; // Lenght of the middle section (always square)
        public static int DetectionLoopLength = 4; //Number of tiles between the detection tiles
        public static int NrOfLanes = 8; //6 vehicle lanes and 2 sidewalks per direction
        public static int LaneLengthHor = (TilesHor - MiddleSize) / 2; //Number of tiles in horizontal lanes
        public static int LaneLengthVer = (TilesVer - MiddleSize) / 2; //Number of tiles in vertical lanes
        public static int TileTextureSize = 32; //32x32p textures

		private CommunicationForm communicationForm;

        private LevelBuilder levelBuilder;
        private LaneBuilder laneBuilder;
        private Lists lists;

		private VehicleControl vehicleControl;
        private TileControl tileControl;
        private LaneControl laneControl;

        private MouseState mouseStateCurrent;
        private MouseState mouseStatePrevious;
        private Vector2 mousePosition;

		/// <summary>
		/// The constructor
		/// </summary>
		/// <param name="communication">The CommunicationForm</param>
        public MainGame(CommunicationForm communication)
        {
			this.communicationForm = communication;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Strings.Content;

            graphics.PreferredBackBufferWidth = TilesHor * TileTextureSize;
            graphics.PreferredBackBufferHeight = TilesVer * TileTextureSize;

			Window.Title = Strings.Title;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Textures.Default = Content.Load<Texture2D>(Strings.SpriteDefault);

            lists = new Lists();

            levelBuilder = new LevelBuilder(lists);
            laneBuilder = new LaneBuilder(lists);

			vehicleControl = new VehicleControl(this.GraphicsDevice, lists, communicationForm);
            tileControl = new TileControl(lists);
            laneControl = new LaneControl(lists);

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
            Textures.RedLight = Content.Load<Texture2D>(Strings.TileRedLight);
            Textures.GreenLight = Content.Load<Texture2D>(Strings.TileGreenLight);
            Textures.YellowLight = Content.Load<Texture2D>(Strings.TileYellowLight);
            Textures.BlinkLight = Content.Load<Texture2D>(Strings.TileBlinkLight);

            Textures.Sidewalk2Red = Content.Load<Texture2D>(Strings.TileSidewalk2Red);
            Textures.Sidewalk2Green = Content.Load<Texture2D>(Strings.TileSidewalk2Green);
            Textures.SidewalkRightRed = Content.Load<Texture2D>(Strings.TileSidewalkRightRed);
            Textures.SidewalkDownRed = Content.Load<Texture2D>(Strings.TileSidewalkDownRed);
            Textures.SidewalkLightGreen = Content.Load<Texture2D>(Strings.TileSidewalkLightGreen);
            Textures.SidewalkLightRed = Content.Load<Texture2D>(Strings.TileSidewalkLightRed);

            Textures.Bikelane = Content.Load<Texture2D>(Strings.TileBikelane);
            Textures.BikeDetect = Content.Load<Texture2D>(Strings.TileBikeDetect);
            Textures.SidewalkDetect = Content.Load<Texture2D>(Strings.TileSidewalkDetect);
            Textures.Buslane = Content.Load<Texture2D>(Strings.TileBuslane);
            Textures.CarSortDown = Content.Load<Texture2D>(Strings.TileCarSortDown);
            Textures.CarSortLeft = Content.Load<Texture2D>(Strings.TileCarSortLeft);
            Textures.CarSortRight = Content.Load<Texture2D>(Strings.TileCarSortRight);

            Textures.Road = Content.Load<Texture2D>(Strings.TileRoad);
            Textures.RoadCenter = Content.Load<Texture2D>(Strings.TileRoadCenter);

            Textures.Crossing = Content.Load<Texture2D>(Strings.TileCrossing);
            Textures.Grass = Content.Load<Texture2D>(Strings.TileGrass);
            Textures.Sidewalk = Content.Load<Texture2D>(Strings.TileSidewalk);
            #endregion

            Textures.Car = Content.Load<Texture2D>(Strings.SpriteCar);
            Textures.Bus = Content.Load<Texture2D>(Strings.SpriteBus);
            Textures.Bike = Content.Load<Texture2D>(Strings.SpriteBike);
            Textures.Pedestrian = Content.Load<Texture2D>(Strings.SpritePedestrian);

            //Create the level
            this.LoadCrossroad(Strings.Grid);
            //Create the lanes
            laneBuilder.LoadLanes();

			// Send lane control to communicationForm form
			communicationForm.SetLaneControl(laneControl);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseButtonPress();

			laneControl.Update(gameTime);
            vehicleControl.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            tileControl.Draw(gameTime, spriteBatch);
            vehicleControl.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

		/// <summary>
		/// Called after the game loop has stopped running before exiting.
		/// </summary>
		protected override void EndRun()
		{
			// Unload the content
			UnloadContent();

			// Close the TCP client of CommunicationForm
			communicationForm.CloseClient();

			// Let my father execute the same function
			base.EndRun();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// Unload the content
			Content.Unload();
		}

		/// <summary>
		/// Load the crossroad
		/// </summary>
		/// <param name="path">The path of the crossroad text file</param>
        private void LoadCrossroad(string path)
        {
            if (File.Exists(path))
                levelBuilder.LoadLevel(path);
            else throw new Exception(Strings.ExceptionCrossroad);
        }

		/// <summary>
		/// Gather mouse button input, like an event
		/// </summary>
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