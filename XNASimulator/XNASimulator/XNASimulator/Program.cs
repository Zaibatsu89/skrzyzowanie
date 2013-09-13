using System.Threading;
using KruispuntGroep6.Simulator.Main;

namespace KruispuntGroep6.Simulator
{
	/// <summary>
	/// Class used to be the first called class.
	/// </summary>
	public static class Program
	{
		private static MainGame game;	// MainGame used to be the instance of Simulator.

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
        public static void Main(string[] args)
        {
            game = new MainGame();
            game.Run();
        }
	}
}