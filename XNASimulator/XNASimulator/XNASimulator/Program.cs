using System.Windows.Forms;
using KruispuntGroep4.Simulator.Communication;

namespace KruispuntGroep4.Simulator
{
	/// <summary>
	/// Class used to be the first called class.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
        public static void Main(string[] args)
        {
			Application.EnableVisualStyles();
			Application.Run(new CommunicationForm());
        }
	}
}