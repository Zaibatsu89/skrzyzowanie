#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using KruispuntGroep4.Globals;
using KruispuntGroep4.Simulator.Globals;
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
	/// <date>23-09-2013</date>
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
			InitializeBackgroundWorkerInput();
			InitializeBackgroundWorkerRead();
			InitializeBackgroundWorkerWrite();
		}

		// Appoint delegates
		delegate void DisplayMessageDelegate(string message);

		// Appoint private attributes
		private BackgroundWorker _bwInput;
		private BackgroundWorker _bwRead;
		private BackgroundWorker _bwWrite;
		private Button _btnInput, _btnSpeedDown,
			_btnSpeedUp, _btnStart;
		private TcpClient _client;
		private IContainer _components = null;
		private string[] _json;
		private LaneControl _laneControl;
		private Label _lblSpeedValue;
		private int _multiplier;
		private TableLayoutPanel _tableLayoutPanel1,
			_tableLayoutPanel2, _tableLayoutPanel3,
			_tableLayoutPanel4, _tableLayoutPanel5;
		private TextBox _tbAddress, _tbConsole,
			_tbPort;

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
		/// Initialize backend: background worker input
		/// </summary>
		private void InitializeBackgroundWorkerInput()
		{
			_bwInput.DoWork += new DoWorkEventHandler(DoWorkInput);
			_bwInput.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedInput);
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
			_bwInput = new BackgroundWorker();
			_bwRead = new BackgroundWorker();
			_bwWrite = new BackgroundWorker();
			_bwWrite.WorkerReportsProgress = true;
			_bwWrite.WorkerSupportsCancellation = true;
			_btnInput = new Button();
			_btnSpeedDown = new Button();
			_btnSpeedUp = new Button();
			_btnStart = new Button();
			_lblSpeedValue = new Label();
			_multiplier = 1;
			_tableLayoutPanel1 = new TableLayoutPanel();
			_tableLayoutPanel2 = new TableLayoutPanel();
			_tableLayoutPanel3 = new TableLayoutPanel();
			_tableLayoutPanel4 = new TableLayoutPanel();
			_tableLayoutPanel5 = new TableLayoutPanel();
			_tbAddress = new TextBox();
			_tbConsole = new TextBox();
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

 			// Set first table layout panel
			_tableLayoutPanel1.Anchor =
				((AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right)));
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

			// Position this button to all directions
			// in the table layout panel
			_btnInput.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnInput.Click += new EventHandler(_btnInput_Click);
			_btnInput.Text = Strings.Input;

			// Set second table layout panel
			_tableLayoutPanel2.Anchor =
				((AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right)));
			_tableLayoutPanel2.Controls.Add(_btnInput, 0, 0);
			_tableLayoutPanel2.Location = new Point(0, 23);
			_tableLayoutPanel2.Size = new Size(315, 30);

			// Position this button to all directions
			// in the table layout panel
			_btnStart.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnStart.Click += new EventHandler(_btnStart_Click);
			_btnStart.Enabled = false;
			// Switch the text of button Start
			SwitchTextButtonStart();

			// Set third table layout panel
			_tableLayoutPanel3.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tableLayoutPanel3.Controls.Add(_btnStart, 0, 0);
			_tableLayoutPanel3.Location = new Point(0, 53);
			_tableLayoutPanel3.Size = new Size(315, 30);

			// Position this button to all directions
			// in the table layout panel
			_btnSpeedDown = new Button();
			_btnSpeedDown.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnSpeedDown.Click += new EventHandler(_btnSpeedDown_Click);
			_btnSpeedDown.Enabled = false;
			_btnSpeedDown.Text = Strings.SpeedDown;

			// Position this button to all directions
			// in the table layout panel
			_btnSpeedUp = new Button();
			_btnSpeedUp.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnSpeedUp.Click += new EventHandler(_btnSpeedUp_Click);
			_btnSpeedUp.Enabled = false;
			_btnSpeedUp.Text = Strings.SpeedUp;

			// Create speed key label
			Label lblSpeedKey = new Label();
			// Position this label to the right
			// in the table layout panel
			lblSpeedKey.Anchor = AnchorStyles.Right;
			lblSpeedKey.AutoSize = true;
			lblSpeedKey.Text = Strings.SpeedKey;

			// Create speed value label
			_lblSpeedValue = new Label();
			// Position this label to the left
			// in the table layout panel
			_lblSpeedValue.Anchor = AnchorStyles.Left;
			_lblSpeedValue.AutoSize = true;
			_lblSpeedValue.Text = Strings.SpeedValue;

			// Set fourth table layout panel
			_tableLayoutPanel4.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tableLayoutPanel4.ColumnCount = 4;
			_tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			_tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			_tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			_tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
			_tableLayoutPanel4.Controls.Add(lblSpeedKey, 0, 0);
			_tableLayoutPanel4.Controls.Add(_lblSpeedValue, 1, 0);
			_tableLayoutPanel4.Controls.Add(_btnSpeedDown, 2, 0);
			_tableLayoutPanel4.Controls.Add(_btnSpeedUp, 3, 0);
			_tableLayoutPanel4.Location = new Point(0, 83);
			_tableLayoutPanel4.Size = new Size(315, 30);

			// Position this textbox to all directions
			// in the table layout panel
			_tbConsole.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tbConsole.Multiline = true;
			_tbConsole.ScrollBars = ScrollBars.Vertical;

			// Set fifth table layout panel
			_tableLayoutPanel5.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tableLayoutPanel5.Controls.Add(_tbConsole, 0, 0);
			_tableLayoutPanel5.Location = new Point(0, 113);
			_tableLayoutPanel5.Size = new Size(315, 605);
			
			// Set other visual candy
			AutoScaleDimensions = new SizeF(120F, 120F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(new Point(315, 732));
			ComponentResourceManager resources =
				new ComponentResourceManager(typeof(CommunicationForm));
			Controls.Add(_tableLayoutPanel1);
			Controls.Add(_tableLayoutPanel2);
			Controls.Add(_tableLayoutPanel3);
			Controls.Add(_tableLayoutPanel4);
			Controls.Add(_tableLayoutPanel5);
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
		/// Button 'Laad invoerbestand',
		/// to read JSON input file
		/// </summary>
		/// <param name="sender">Input Button</param>
		/// <param name="e">Event args</param>
		private void _btnInput_Click(object sender, EventArgs e)
		{
			// Create open file dialog
			FileDialog dialog = new OpenFileDialog();
			DialogResult result = dialog.ShowDialog();

			// If the dialog result is OK
			if (result.Equals(DialogResult.OK))
			{
				// Disable button Input
				_btnInput.Enabled = false;

				// Read JSON from selected file
				_json = File.ReadAllLines(dialog.FileName);

				if (_json[0].StartsWith(Strings.BraceOpen))
				{
					// Convert JSON object to message
					_json[0] = JsonObjectToMessage();
				}
				else if (_json[0].StartsWith(Strings.BracketOpen))
				{
					// Convert JSON array to messages
					_json = JsonArrayToMessages();
				}

				// Enable button Start
				_btnStart.Enabled = true;
			}
		}

		/// <summary>
		/// Button to decrease multiplier times two and send multiplier
		/// </summary>
		/// <param name="sender">Speed down Button</param>
		/// <param name="e">Event args</param>
		private void _btnSpeedDown_Click(object sender, EventArgs e)
		{
			// Decrease the multiplier times two
			_multiplier /= 2;

			// Send multiplier to host
			WriteMultiplierMessage();

			// Speed value Label text is multiplier to string
			_lblSpeedValue.Text = _multiplier.ToString();

			// If the multiplier is less than 2
			if (_multiplier < 2)
			{
				// Disable speed down Button
				_btnSpeedDown.Enabled = false;
			}
			else /* Else if the multiplier equals or is greater than 2 */
			{
				// Enable speed up Button
				_btnSpeedUp.Enabled = true;
			}
		}

		/// <summary>
		/// Button to increase multiplier times two and send multiplier
		/// </summary>
		/// <param name="sender">Speed up Button</param>
		/// <param name="e">Event args</param>
		private void _btnSpeedUp_Click(object sender, EventArgs e)
		{
			// Increase the multiplier times two
			_multiplier *= 2;

			// Send multiplier to host
			WriteMultiplierMessage();

			// Speed value Label text is multiplier to string
			_lblSpeedValue.Text = _multiplier.ToString();

			// If the multiplier equals or is greater than 512
			if (_multiplier >= 512)
			{
				// Disable speed up Button
				_btnSpeedUp.Enabled = false;
			}
			else /* Else if the multiplier is less than 512 */
			{
				// Enable speed down Button
				_btnSpeedDown.Enabled = true;
			}
		}

		/// <summary>
		/// Button 'Start simulatie', to run new view and to
		/// connect to the specified host address and port
		/// </summary>
		/// <param name="sender">Start Button</param>
		/// <param name="e">Event args</param>
		private void _btnStart_Click(object sender, EventArgs e)
		{
			// If the button reads 'Start simulatie'
			if (_btnStart.Text.Equals(Strings.StartView))
			{
				// Disable buttons and enable read only textboxes
				_btnInput.Enabled = false;
				_tbAddress.ReadOnly = true;
				_tbPort.ReadOnly = true;

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
				// background worker write
				_bwWrite.CancelAsync();
			}
		}
		#endregion

		#region Public methods
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
		/// Write the specified detection message to the host
		/// </summary>
		/// <param name="vType">Vehicle type</param>
		/// <param name="loop">Loop: close or far</param>
		/// <param name="empty">Empty: true or false</param>
		/// <param name="laneIDfrom">Direction and number</param>
		/// <param name="laneIDto">Direction and number</param>
		public void WriteDetectionMessage(VehicleTypeEnum vType, LoopEnum loop, string empty, string laneIDfrom, string laneIDto)
		{
			// Initialize the detection message
			string message = string.Empty;

			// Create the message
			message =
			DynamicJson.Serialize(new object[]
			{
				new
				{
					light = laneIDfrom,
					type = vType.ToString(),
					loop = loop.ToString(),
					empty = empty,
					to = laneIDto
				}
			});
			
			// Write the message
			WriteMessage(message);
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
		/// Display the specified message in the Console listbox
		/// </summary>
		/// <param name="message">Message</param>
		private void DisplayMessage(string message)
		{
			// If the caller comes from a different thread
			if (InvokeRequired)
			{
				// Execute the associated delegate asynchronously
				BeginInvoke(new DisplayMessageDelegate(DisplayMessage), message);

				// Can't display the message yet
				return;
			}

			// Append the message to the Console listbox,
			// ending with a new line
			_tbConsole.AppendText(
				message +
				Environment.NewLine
			);
		}

		/// <summary>
		/// Read the JSON input file and spawn vehicles
		/// </summary>
		/// <param name="sender">Background worker input</param>
		/// <param name="e">Do work event args</param>
		private void DoWorkInput(object sender, DoWorkEventArgs e)
		{
			// Initialize previous time
			int previousTime = -1;

			// For each JSON string in JSON array
			foreach (string json in _json)
			{
				// Initialize whether Godzilla exists
				// and create vehicle type
				bool isGodzilla = false;
				VehicleTypeEnum vehicleType = VehicleTypeEnum.car;

				// Create vehicle type parameter value from JSON string
				string type = json.Split(',')[1];

				// What type is it?
				switch (type)
				{
					case Strings.VehicleTypeBicycle: /* It is a bicycle */
						vehicleType = VehicleTypeEnum.bicycle;
						break;
					case Strings.VehicleTypeBus: /* It is a bus */
						vehicleType = VehicleTypeEnum.bus;
						break;
					case Strings.VehicleTypeCar: /* It is a car */
						vehicleType = VehicleTypeEnum.car;
						break;
					case Strings.VehicleTypePedestrian: /* It is a pedestrian */
						vehicleType = VehicleTypeEnum.pedestrian;
						break;
					default: /* It is a Godzilla */
						isGodzilla = true;
						break;
				}

				// If the vehicle type isn't Godzilla
				if (!isGodzilla)
				{
					// Create from and to parameter values from JSON string
					string from = json.Split(',')[2];
					string to = (json.Split(',')[3]).Split(']')[0];

					// Spawn the vehicle
					_laneControl.SpawnVehicle(vehicleType, from, to);
				}

				// Create time parameter value from JSON string
				int time = int.Parse((json.Split('[')[1]).Split(',')[0]);

				// If the time is after the previous time
				if (time > previousTime)
				{
					// Sleep for a second
					Thread.Sleep(1000);
				}

				// The previous time is time
				previousTime = time;
			}
		}

		/// <summary>
		/// Read a message from the host
		/// </summary>
		/// <param name="sender">Background worker read</param>
		/// <param name="e">Do work event args</param>
		private void DoWorkReading(object sender, DoWorkEventArgs e)
		{
			// The result is the received message
			e.Result = ReadMessage();
		}

		/// <summary>
		/// Connect to the host
		/// </summary>
		/// <param name="sender">Background worker write</param>
		/// <param name="e">Tuple with address string and port int</param>
		private void DoWorkWriting(object sender, DoWorkEventArgs e)
		{
			// Create background worker write and host tuple
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
				// Read messages in the background, so the UI stays responsive
				_bwRead.RunWorkerAsync();

				// When there is a TCP client created,
				// give the user a connected result
				e.Result = Strings.Connected;
			}
		}

		/// <summary>
		/// Get the IP address
		/// </summary>
		private string GetAddress()
		{
			// Initialize IP address and create host;
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
		/// Convert JSON array to messages
		/// </summary>
		/// <returns>Readable JSON messages</returns>
		private string[] JsonArrayToMessages()
		{
			// Initialize list of messages
			List<string> messages = new List<string>();

			// Parse JSON string array as string
			var json = DynamicJson.Parse(string.Join(string.Empty, _json));

			// Extract parameter values in dynamic JSON format
			var count = ((dynamic[])json).Count();
			var time = ((dynamic[])json).Select(d => d.time);
			var type = ((dynamic[])json).Select(d => d.type);
			var from = ((dynamic[])json).Select(d => d.from);
			var to = ((dynamic[])json).Select(d => d.to);

			// For every JSON object
			for (int i = 0; i < count; i++)
			{
				// Create strings from parameter values
				string strTime = time.ElementAt(i);
				string strType = type.ElementAt(i);
				string strFrom = from.ElementAt(i);
				string strTo = to.ElementAt(i);

				// Create message with parameter values in upper case
				string message = string.Empty;
				message += Strings.BracketOpen;
				message += strTime;
				message += Strings.Comma;
				message += strType;
				message += Strings.Comma;
				message += strFrom;
				message += Strings.Comma;
				message += strTo;
				message += Strings.BracketClose;

				// Add this message to the list of messages
				messages.Add(message);
			}

			// Convert the list of messages to a string array,
			// store it in the private _json attribute
			// and return it
			return _json = messages.ToArray();
		}

		/// <summary>
		/// Converts dynamic JSON object string to readable message
		/// </summary>
		/// <param name="json">String used to contain a dynamic JSON object</param>
		/// <returns>String used to contain a readable message</returns>
		private string JsonObjectToMessage()
		{
			// Parse JSON string
			var json = DynamicJson.Parse(_json[0]);

			// Create strings from parameter values
			string time = json.time;
			string type = json.type;
			string from = json.from;
			string to = json.to;

			// Create a message with parameter values in upper case
			string message = string.Empty;
			message += Strings.BracketOpen;
			message += time;
			message += Strings.Comma;
			message += type;
			message += Strings.Comma;
			message += from;
			message += Strings.Comma;
			message += to;
			message += Strings.BracketClose;

			// Return this message
			return message;
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
		/// Read a message from the host,
		/// if the TCP client is connected,
		/// else an empty message is returned
		/// </summary>
		/// <returns></returns>
		private string ReadMessage()
		{
			// Initialize the message
			string message = string.Empty;

			// If the TCP client exists
			if (_client != null)
			{
				// If the TCP client is connected
				if (_client.Connected)
				{
					// Create a stream reader for the TCP client stream
					StreamReader reader = new StreamReader(_client.GetStream());

					// Try to receive the message
					try
					{
						// The message is received from the host
						message = reader.ReadLine();
					}
					catch (IOException e) /* Catch IO exception */
					{
						// The message is the exception message
						message = e.Message;
					}

					// Display the message
					DisplayMessage(Strings.Received + Strings.Space + message);
				}
			}

			// Return the message
			return message;
		}

		/// <summary>
		/// Display a message saying all input JSONs are sent
		/// </summary>
		/// <param name="sender">Background worker input</param>
		/// <param name="e">Run worker completed event args</param>
		private void RunWorkerCompletedInput(object sender, RunWorkerCompletedEventArgs e)
		{
			// Disable speed down/up Buttons
			_btnSpeedDown.Enabled = false;
			_btnSpeedUp.Enabled = false;

			// Display all input JSONs sent message
			DisplayMessage(Strings.Sent + Strings.Space + Strings.AllInputJsons);
		}

		/// <summary>
		/// When the background work is done and the connection is lost,
		/// an error message is displayed
		/// </summary>
		/// <param name="sender">Background worker read</param>
		/// <param name="e">Whether the connection with the host was lost</param>
		private void RunWorkerCompletedReading(object sender, RunWorkerCompletedEventArgs e)
		{
			// Create the received message from the result
			string message = e.Result as string;

			// If the message probably is valid JSON
			if (message.StartsWith(Strings.BraceOpen) ||
				message.StartsWith(Strings.BracketOpen))
			{
				// Parse JSON string
				var json = DynamicJson.Parse(message);

				// Extract parameter values in dynamic JSON format
				var count = ((dynamic[])json).Count();
				var light = ((dynamic[])json).Select(d => d.light);
				var state = ((dynamic[])json).Select(d => d.state);

				// For every JSON object
				for (int i = 0; i < count; i++)
				{
					// Create strings from parameter values
					string strLight = light.ElementAt(i);
					string strState = state.ElementAt(i).ToLower();

					// Initialize lights enum
					LightsEnum lightsEnum = LightsEnum.Off;

					// What state is it?
					switch (strState)
					{
						case Strings.LightStateBlink: /* It is blinking */
							lightsEnum = LightsEnum.Blink;
							break;
						case Strings.LightStateGreen: /* It is green */
							lightsEnum = LightsEnum.Green;
							break;
						case Strings.LightStateOff: /* It is off */
							lightsEnum = LightsEnum.Off;
							break;
						case Strings.LightStateRed: /* It is red */
							lightsEnum = LightsEnum.Red;
							break;
						case Strings.LightStateYellow: /* It is yellow */
							lightsEnum = LightsEnum.Yellow;
							break;
					}

					// Change the specified traffic light
					_laneControl.ChangeTrafficLight(lightsEnum, strLight);
				}
			}

			// Read messages in the background, so the UI stays responsive
			_bwRead.RunWorkerAsync();
		}

		/// <summary>
		/// When the background work is done, enables buttons and textboxes
		/// When the user didn't interfere, a success message is displayed
		/// </summary>
		/// <param name="sender">Background worker write</param>
		/// <param name="e">Whether the user interfered as property cancelled</param>
		private void RunWorkerCompletedWriting(object sender, RunWorkerCompletedEventArgs e)
		{
			// If the user interfered
			if (e.Cancelled)
			{
				// Disable read only textboxes
				_tbAddress.ReadOnly = false;
				_tbPort.ReadOnly = false;

				// Switch the text of button Start
				SwitchTextButtonStart();
			}
			else /* Else if the user didn't interfere, display a success message */
			{
				// Disable start Button
				_btnStart.Enabled = false;

				// Display success message
				DisplayMessage(e.Result as string);

				// Wait for lane control
				while (_laneControl == null) { }

				// Create start time and multiplier messages
				string startTime =
				DynamicJson.Serialize(new object[]
				{
					new
					{
						starttime = DateTime.Now.ToShortTimeString()
					}
				});
				string multiplier =
				DynamicJson.Serialize(new object[]
				{
					new
					{
						multiplier = _multiplier
					}
				});

				// Send start time
				WriteMessage(startTime);

				// Send multiplier
				WriteMessage(multiplier);

				// Enable speed up Button
				_btnSpeedUp.Enabled = true;

				// Spawn all vehicles from the JSON input file in the background
				_bwInput.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Switch the text of button Start
		/// </summary>
		private void SwitchTextButtonStart()
		{
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

		/// <summary>
		/// Write the specified message to the host,
		/// if the TCP client is connected
		/// </summary>
		/// <param name="message"></param>
		private void WriteMessage(string message)
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
					DisplayMessage(Strings.Sent + Strings.Space + message);
				}
			}
		}

		/// <summary>
		/// Write a multiplier message to the host
		/// </summary>
		public void WriteMultiplierMessage()
		{
			// Create the multiplier message
			string message =
			DynamicJson.Serialize(new object[]
			{
				new
				{
					multiplier = _multiplier
				}
			});

			// Write the message
			WriteMessage(message);
		}
		#endregion
	}
}