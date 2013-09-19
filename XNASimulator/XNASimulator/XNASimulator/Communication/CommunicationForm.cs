#region Usings
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using KruispuntGroep4.Globals;
using KruispuntGroep4.Simulator.Main;
using KruispuntGroep4.Simulator.ObjectControllers;
#endregion

namespace KruispuntGroep4.Simulator.Communication
{
	/// <copyright file="CommunicationForm.cs" company="NHL Hogeschool">
	/// Copyright (c) 2013 All Rights Reserved
	/// 
	/// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
	/// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
	/// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
	/// PARTICULAR PURPOSE.
	/// 
	/// </copyright>
	/// <author>Rinse Cramer</author>
	/// <email>rinsecramer@gmail.com</email>
	/// <date>19-09-2013</date>
	/// <summary>CommunicationForm is a form with
	/// inputfields for host address/port & messages,
	/// buttons for start/stop view & sending messages
	/// and a console for displaying messages</summary>
	class CommunicationForm : Form
	{
		#region Constructor, private attributes, dispose and initializing methods
		/// <summary>
		/// Constructor
		/// </summary>
		public CommunicationForm()
		{
			// Initialize frontend
			InitializeComponent();
			
			// Initialize backend
			InitializeBackgroundWorkerRead();
			InitializeBackgroundWorkerWrite();
		}

		// Appoint delegates
		delegate void DisplayMessageDelegate(string message);
		delegate void SwitchTextButtonStartDelegate();

		// Appoint private attributes
		private BackgroundWorker _bwRead;
		private BackgroundWorker _bwWrite;
		private Button _btnSend, _btnStart;
		private TcpClient _client;
		private IContainer _components = null;
		private LaneControl _laneControl;
		private TableLayoutPanel _tableLayoutPanel1,
			_tableLayoutPanel2, _tableLayoutPanel3,
			_tableLayoutPanel4;
		private TextBox _tbAddress, _tbConsole,
			_tbMessage, _tbPort;

		/// <summary>
		/// Dispose the form
		/// </summary>
		/// <param name="disposing">Whether the form is disposing</param>
		protected override void Dispose(bool disposing)
		{
			// If the form is disposing and there are components left
			if (disposing && _components != null)
			{
				// Dispose these components
				_components.Dispose();
			}

			// Let my father execute the same function
			base.Dispose(disposing);
		}

		/// <summary>
		/// Initialize backend: background worker read
		/// </summary>
		private void InitializeBackgroundWorkerRead()
		{
			_bwRead.DoWork += new DoWorkEventHandler(DoWorkReading);
			_bwRead.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedReading);
		}

		/// <summary>
		/// Initialize backend: background worker write
		/// </summary>
		private void InitializeBackgroundWorkerWrite()
		{
			_bwWrite.DoWork += new DoWorkEventHandler(DoWorkWriting);
			_bwWrite.ProgressChanged += new ProgressChangedEventHandler(ProgressChangedWriting);
			_bwWrite.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedWriting);
		}

		/// <summary>
		/// Initialize frontend: buttons, table layout panels,
		/// textboxes and other visual candy
		/// </summary>
		private void InitializeComponent()
		{
			// Initialize private attributes
			_bwRead = new BackgroundWorker();
			_bwRead.WorkerReportsProgress = true;
			_bwWrite = new BackgroundWorker();
			_bwWrite.WorkerReportsProgress = true;
			_bwWrite.WorkerSupportsCancellation = true;
			_btnSend = new Button();
			_btnStart = new Button();
			_tableLayoutPanel1 = new TableLayoutPanel();
			_tableLayoutPanel2 = new TableLayoutPanel();
			_tableLayoutPanel3 = new TableLayoutPanel();
			_tableLayoutPanel4 = new TableLayoutPanel();
			_tbAddress = new TextBox();
			_tbConsole = new TextBox();
			_tbMessage = new TextBox();
			_tbPort = new TextBox();

			// Temporary suspend the layout logic for the form
			SuspendLayout();

			// Create address label
			Label lblAddress = new Label();
			// Position this label to the right
			// in the table layout panel
			lblAddress.Anchor = AnchorStyles.Right;
			lblAddress.AutoSize = true;
			lblAddress.Text = Strings.AddressKey;

			// Create port label
			Label lblPort = new Label();
			// Position this label to the right
			// in the table layout panel
			lblPort.Anchor = AnchorStyles.Right;
			lblPort.AutoSize = true;
			lblPort.Text = Strings.PortKey;
			
 			// Set first table layout panel
			_tableLayoutPanel1.Anchor =
				((AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right)));
			_tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_tableLayoutPanel1.ColumnCount = 4;
			_tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
			_tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
			_tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			_tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			_tableLayoutPanel1.Controls.Add(lblAddress, 0, 0);
			_tableLayoutPanel1.Controls.Add(_tbAddress, 1, 0);
			_tableLayoutPanel1.Controls.Add(lblPort, 2, 0);
			_tableLayoutPanel1.Controls.Add(_tbPort, 3, 0);
			_tableLayoutPanel1.Location = new Point(0, 0);
			_tableLayoutPanel1.Size = new Size(315, 23);

			// Position this label to the left and
			// the right in the table layout panel
			_tbAddress.Anchor =
				(AnchorStyles)((AnchorStyles.Left |
				AnchorStyles.Right));
			_tbAddress.Text = GetAddress();

			// Position this label to the left and
			// the right in the table layout panel
			_tbPort.Anchor =
				(AnchorStyles)((AnchorStyles.Left |
				AnchorStyles.Right));
			_tbPort.Text = Strings.PortValue;

			// Set second table layout panel
			_tableLayoutPanel2.Anchor =
				((AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right)));
			_tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_tableLayoutPanel2.ColumnCount = 1;
			_tableLayoutPanel2.Controls.Add(_btnStart, 0, 0);
			_tableLayoutPanel2.Location = new Point(0, 23);
			_tableLayoutPanel2.Size = new Size(315, 30);

			// Position this button to all directions
			// in the table layout panel
			_btnStart.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnStart.Click += new EventHandler(_btnStart_Click);
			SwitchTextButtonStart();

			// Set third table layout panel
			_tableLayoutPanel3.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tableLayoutPanel3.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_tableLayoutPanel3.ColumnCount = 2;
			_tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			_tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			_tableLayoutPanel3.Controls.Add(_tbMessage, 0, 0);
			_tableLayoutPanel3.Controls.Add(_btnSend, 1, 0);
			_tableLayoutPanel3.Location = new Point(0, 53);
			_tableLayoutPanel3.Size = new Size(315, 30);

			// Position this textbox to the left,
			// the right and the bottom
			// in the table layout panel
			_tbMessage.Anchor =
				(AnchorStyles)(((AnchorStyles.Bottom |
				AnchorStyles.Left) |
				AnchorStyles.Right));
			_tbMessage.Text = Strings.Message;

			// Position this button to all directions
			// in the table layout panel 
			_btnSend.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnSend.Click += new EventHandler(_btnSend_Click);
			_btnSend.Enabled = false;
			_btnSend.Text = Strings.Send;

			// Set fourth table layout panel
			_tableLayoutPanel4.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tableLayoutPanel4.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_tableLayoutPanel4.ColumnCount = 1;
			_tableLayoutPanel4.Controls.Add(_tbConsole, 0, 0);
			_tableLayoutPanel4.Location = new Point(0, 83);
			_tableLayoutPanel4.Size = new Size(315, 635);

			// Position this textbox to all directions
			// in the table layout panel
			_tbConsole.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tbConsole.Multiline = true;
			_tbConsole.ScrollBars = ScrollBars.Vertical;
			
			// Set other visual candy
			AutoScaleDimensions = new SizeF(120F, 120F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(new Point(315, 732));
			ComponentResourceManager resources = new ComponentResourceManager(typeof(CommunicationForm));
			Controls.Add(_tableLayoutPanel1);
			Controls.Add(_tableLayoutPanel2);
			Controls.Add(_tableLayoutPanel3);
			Controls.Add(_tableLayoutPanel4);
			FormBorderStyle = FormBorderStyle.Fixed3D;
			Icon = resources.GetObject(Strings.Icon) as Icon;
			Location = Settings.Default.Location;
			MaximizeBox = false;
			StartPosition = FormStartPosition.Manual;
			Text = Strings.TitleCommunication;

			// Resume the layout logic for the form
			ResumeLayout(false);
		}
		#endregion

		#region Events
		/// <summary>
		/// Button 'Verzenden',
		/// to send the specified message to the host
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event args</param>
		private void _btnSend_Click(object sender, EventArgs e)
		{
			// The message is the text from textbox Message
			string message = _tbMessage.Text;

			// Write the message
			WriteMessage(message);
		}

		/// <summary>
		/// Button 'Start simulatie', to run new view and to
		/// connect to the specified host address and port
		/// </summary>
		/// <param name="sender">Object</param>
		/// <param name="e">Event args</param>
		private void _btnStart_Click(object sender, EventArgs e)
		{
			// If the button reads 'Start simulatie'
			if (_btnStart.Text.Equals(Strings.StartView))
			{
				// Disable buttons and textboxes
				_btnSend.Enabled = false;
				_tbAddress.Enabled = false;
				_tbMessage.Enabled = false;
				_tbPort.Enabled = false;

				// Switch the text of button Start
				SwitchTextButtonStart();

				// Start running a view
				new Thread(() =>
				{
					new MainGame(this).Run();
				}).Start();

				// Collect host information from textboxes
				string address = _tbAddress.Text;
				int port = int.Parse(_tbPort.Text);
				Tuple<string, int> host = new Tuple<string, int>(address, port);

				// Connect in the background worker write,
				// so the UI stays responsive
				_bwWrite.RunWorkerAsync(host);
			}
			else /* Else if the button reads 'Stop simulatie' */
			{
				// User wants to cancel while connecting,
				// so request cancellation of
				// background workers read and write
				_bwWrite.CancelAsync();
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Read a message from the host,
		/// if the TCP client is connected,
		/// else an empty message is returned
		/// </summary>
		/// <returns></returns>
		public string ReadMessage()
		{
			string message = string.Empty;

			// If the TCP client exists
			if (_client != null)
			{
				// If the TCP client is connected
				if (_client.Connected)
				{
					// Create a stream reader for the TCP client stream
					StreamReader reader = new StreamReader(_client.GetStream());

					// The message is received from the host
					message = reader.ReadLine();

					// Display message
					DisplayMessage(Strings.Received + message);
				}
			}

			return message;
		}

		/// <summary>
		/// Set lane control, used by a view
		/// </summary>
		/// <param name="laneControl"></param>
		public void SetLaneControl(LaneControl laneControl)
		{
			// Private lane control is specified lane control
			_laneControl = laneControl;
		}

		/// <summary>
		/// Write the specified message to the host,
		/// if the TCP client is connected
		/// </summary>
		/// <param name="message"></param>
		public void WriteMessage(string message)
		{
			// If the TCP client exists
			if (_client != null)
			{
				// If the TCP client is connected and the message isn't empty
				if (_client.Connected && message != string.Empty)
				{
					// Create a stream writer for the TCP client stream
					StreamWriter writer = new StreamWriter(_client.GetStream());

					// Write the message
					writer.WriteLine(message);

					// Ensure that the buffer is empty
					writer.Flush();

					// Display the message in the Console listbox
					DisplayMessage(Strings.Sent + _tbMessage.Text);
				}
			}
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Override OnFormClosed, so that all views are closed as well.
		/// </summary>
		/// <param name="e">Form closed event args</param>
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			// Save settings
			Settings.Default.Location = Location;
			Settings.Default.Save();

			// Let my father execute the same function
			base.OnFormClosed(e);

			// Close all application windows
			Application.Exit();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Change a traffic light in a view
		/// </summary>
		private void ChangeLight()
		{
			// Change traffic light with color value and spawn lane ID
			_laneControl.ChangeTrafficLight(Strings.TrafficLightValue, Strings.SpawnLaneID);
		}

		/// <summary>
		/// Display the specified message in the Console listbox
		/// </summary>
		/// <param name="message">Message</param>
		private void DisplayMessage(string message)
		{
			// Initialize the time
			string time = DateTime.Now.ToString(Strings.Time);

			// If the caller comes from a different thread
			if (InvokeRequired)
			{
				// Execute the associated delegate asynchronously
				BeginInvoke(new DisplayMessageDelegate(DisplayMessage), message);

				// Can't display the message yet
				return;
			}

			// Append the time and the message
			// to the Console listbox,
			// ending with a new line
			_tbConsole.AppendText(
				time +
				Strings.Space +
				message +
				Environment.NewLine
			);
		}

		/// <summary>
		/// Read a message from the host
		/// </summary>
		/// <param name="sender">Background worker read</param>
		/// <param name="e">Do work event args</param>
		private void DoWorkReading(object sender, DoWorkEventArgs e)
		{
			ReadMessage();
		}

		/// <summary>
		/// Connect to the host
		/// </summary>
		/// <param name="sender">Background worker write</param>
		/// <param name="e">Tuple with address string and port int</param>
		private void DoWorkWriting(object sender, DoWorkEventArgs e)
		{
			// Initialize background worker write and host tuple
			BackgroundWorker backgroundWorkerWrite = sender as BackgroundWorker;
			Tuple<string, int> host = e.Argument as Tuple<string, int>;

			// Connect while there is no TCP client created
			while (_client == null)
			{
				// Try connecting
				try
				{
					// If the user requested cancellation,
					// set Cancel property and break
					if (backgroundWorkerWrite.CancellationPending)
					{
						e.Cancel = true;
						break;
					}

					// Connecting to specified host address and port
					_client = new TcpClient(host.Item1, host.Item2);
				}
				catch (SocketException) /* When there is no connection possible */
				{
					// Report the user that there is an error while connecting
					backgroundWorkerWrite.ReportProgress(0, Strings.Error);

					// Sleep for a second, so the error spam is limited
					Thread.Sleep(1000);
				}
			}

			// If the cancel property is false
			if (!e.Cancel)
			{
				// When there is a TCP client created,
				// give the user a connected result
				e.Result = Strings.Connected;

				// Read messages in the background, so the UI stays responsive
				_bwRead.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Get the IP address
		/// </summary>
		private string GetAddress()
		{
			// Initialize IP address and host;
			string address = string.Empty;
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			// For each IP address, check if it's an inter network address
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily.ToString().Equals(Strings.AddressType))
				{
					// Set address
					address = ip.ToString();
				}
			}

			// If there is no internet, use localhost
			if (address.Equals(string.Empty))
			{
				// Set address as localhost
				address = Strings.AddressValue;
			}

			// Return address
			return address;
		}

		/// <summary>
		/// Display an error message in case of socket exception while connecting
		/// </summary>
		/// <param name="sender">Background worker write</param>
		/// <param name="e">Error message as property user state</param>
		private void ProgressChangedWriting(object sender, ProgressChangedEventArgs e)
		{
			// Display error message
			DisplayMessage(e.UserState as string);
		}

		/// <summary>
		/// When the background work is done and the connection is lost,
		/// an error message is displayed
		/// </summary>
		/// <param name="sender">Background worker read</param>
		/// <param name="e">Whether the connection with the host was lost</param>
		private void RunWorkerCompletedReading(object sender, RunWorkerCompletedEventArgs e)
		{
			// Read messages in the background, so the UI stays responsive
			bwRead.RunWorkerAsync();
		}

		/// <summary>
		/// When the background work is done, enables buttons and textboxes
		/// When the user didn't interfere, a success message is displayed
		/// </summary>
		/// <param name="sender">Background worker write</param>
		/// <param name="e">Whether the user interfered as property cancelled</param>
		private void RunWorkerCompletedWriting(object sender, RunWorkerCompletedEventArgs e)
		{
			// Enable buttons and textboxes
			_tbAddress.Enabled = true;
			_tbMessage.Enabled = true;
			_tbPort.Enabled = true;

			// If the user interfered
			if (e.Cancelled)
			{
				// Switch the text of button Start
				SwitchTextButtonStart();
			}
			else /* Else if the user didn't interfere, display a success message */
			{
				// Enable button Send
				_btnSend.Enabled = true;

				// Disable button Start
				_btnStart.Enabled = false;

				// Display success message
				DisplayMessage(e.Result as string);
			}
		}

		/// <summary>
		/// Spawn a vehicle in a view
		/// </summary>
        private void SpawnVehicle()
        {
			// Spawn vehicle with type, spawn lane ID and destination lane ID
            _laneControl.SpawnVehicle(Strings.VehicleType, Strings.SpawnLaneID, Strings.DestinationLaneID);
		}

		/// <summary>
		/// Switch the text of button Start
		/// </summary>
		private void SwitchTextButtonStart()
		{
			// If the caller comes from a different thread
			if (InvokeRequired)
			{
				// Execute the associated delegate asynchronously
				BeginInvoke(new SwitchTextButtonStartDelegate(SwitchTextButtonStart));

				// Can't switch the text of button Start yet
				return;
			}

			// If the button start text isn't 'Start simulatie'
			if (!_btnStart.Text.Equals(Strings.StartView))
			{
				// The button start text is 'Start simulatie'
				_btnStart.Text = Strings.StartView;
			}
			else /* Else if the button start text is 'Start simulatie' */
			{
				// The button start text is 'Stop simulatie'
				_btnStart.Text = Strings.StopView;
			}
		}
		#endregion
	}
}