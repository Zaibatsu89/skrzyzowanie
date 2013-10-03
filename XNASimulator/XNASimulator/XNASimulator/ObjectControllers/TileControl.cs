using KruispuntGroep4.Simulator.Globals;
using KruispuntGroep4.Simulator.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KruispuntGroep4.Simulator.ObjectControllers
{
    /// <summary>
    /// Handles changes in Tiles
    /// </summary>
    class TileControl
    {
        private Lists lists;

        private int LevelWidth
        {
            get { return lists.Tiles.GetLength(0); }
        } //horizontal tiles
        private int LevelHeight
        {
            get { return lists.Tiles.GetLength(1); }
        } //vertical tiles

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="lists">Lists of all Vehicles, Lanes and Tiles</param>
        public TileControl(Lists lists)
        {
            this.lists = lists;
        }

        /// <summary>
        /// Draws all tiles, called from MainGame.Draw
        /// </summary>
        /// <param name="gameTime">a game update cycle</param>
        /// <param name="spriteBatch">a collection of sprites/textures</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < LevelHeight; ++y)
            {
                for (int x = 0; x < LevelWidth; ++x)
                {
                    // If there is a visible tile in that position
                    if (!Texture2D.Equals(lists.Tiles[x, y].Texture, null))
                    {
                        // Draw it in screen space.                     
                        spriteBatch.Draw(lists.Tiles[x, y].Texture,
                                        lists.Tiles[x, y].DrawPosition,
                                        null,
                                        Color.White,
                                        Rotation.getRotation(lists.Tiles[x, y].Rotation),
                                        lists.Tiles[x, y].Origin,
                                        1.0f,
                                        SpriteEffects.None,
                                        1.0f);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a mouse click is clicking on a tile
        /// </summary>
        /// <param name="mouseposition">Position of mouse cursor</param>
        public void CheckMouseCollision(Vector2 mouseposition)
        {
            Rectangle mouseArea = new Rectangle((int)mouseposition.X, (int)mouseposition.Y, 1, 1);

            foreach (Tile tile in lists.Tiles)
            {
                if (tile.CollisionRectangle.Contains(mouseArea))
                {
                    this.ChangeLights(tile);
                }
            }
        }

        /// <summary>
        /// Gives the ability to manually change traffic lights
        /// from red to green and back for demonstration
        /// purposes.
        /// </summary>
        /// <param name="tile"></param>
        public void ChangeLights(Tile tile)
        {
            if (tile.isGreen == false && tile.Texture.Equals(Textures.RedLight))
            {
                tile.Texture = Textures.GreenLight;
                tile.isGreen = true;
            }
            else if (tile.isGreen == true && tile.Texture.Equals(Textures.GreenLight))
            {
                tile.Texture = Textures.RedLight;
                tile.isGreen = false;
            }
        }

        /// <summary>
        /// The changing of lights handled by the Controller
        /// </summary>
        /// <param name="laneID">ID of the lane containing the traffic light</param>
        /// <param name="colour">New value of the traffic light</param>
        public void ChangeLights(string laneID, LightsEnum colour)
        {
            Lane lane;

            lists.Lanes.TryGetValue(laneID, out lane);

            //Discard the sidewalk lanes
            if (!laneID[1].Equals('0') && !laneID[1].Equals('7'))
            {
                switch (colour)
                {
                    case LightsEnum.Blink: lane.trafficLight.Texture = Textures.BlinkLight;
                        break;
                    case LightsEnum.Red: lane.trafficLight.Texture = Textures.RedLight;
                        break;
                    case LightsEnum.Green: lane.trafficLight.Texture = Textures.GreenLight;
                        break;
                    case LightsEnum.Yellow: lane.trafficLight.Texture = Textures.YellowLight;
                        break;
                }
            }
            else
            {
                switch (colour)
                {
                    case LightsEnum.Red: lane.trafficLight.Texture = Textures.SidewalkLightRed;
                        break;
                    case LightsEnum.Green: lane.trafficLight.Texture = Textures.SidewalkLightGreen;
                        break;
                }
            }
        }
    }
}