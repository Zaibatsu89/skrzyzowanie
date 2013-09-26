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
	/// <date>2013</date>
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
			InitializeBackgroundWorkerSpawn();
			InitializeBackgroundWorkerWrite();

			// Initialize attributes
			_multiplier = 1;
			_tbAddress.Text = CreateAddress();
			_timeSpanSleep = new TimeSpan(100000000);
			SwitchTextButtonStart();
		}

		// Appoint private attributes
		private BackgroundWorker _bwInput;
		private BackgroundWorker _bwRead;
		private BackgroundWorker _bwSpawn;
		private BackgroundWorker _bwWrite;
		private Button _btnInput, _btnSpeedDown,
			_btnSpeedUp, _btnStart;
		private TcpClient _client;
		private IContainer _components = null;
		private string[] _json;
		private LaneControl _laneControl;
		private Label _lblAddress;
		private Label _lblPort;
		private Label _lblSpeedKey;
		private Label _lblSpeedValue;
		private int _multiplier;
		private ProgressBar _progressBarFile;
		private ProgressBar _progressBarMessages;
		private TableLayoutPanel _tableLayoutPanel1,
			_tableLayoutPanel2, _tableLayoutPanel3,
			_tableLayoutPanel4;
		private TextBox _tbAddress, _tbPort;
		private TimeSpan _timeSpanSleep;

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
			// Appoint input Background worker
			_bwInput = new BackgroundWorker();
			_bwInput.WorkerReportsProgress = true;

			// Appoint input Background worker events
			_bwInput.DoWork += new DoWorkEventHandler(DoWorkInput);
			_bwInput.ProgressChanged += new ProgressChangedEventHandler(ProgressChangedInput);
			_bwInput.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedInput);
		}

		/// <summary>
		/// Initialize backend: background worker read
		/// </summary>
		private void InitializeBackgroundWorkerRead()
		{
			// Appoint read Background worker
			_bwRead = new BackgroundWorker();

			// Appoint read Background worker events
			_bwRead.DoWork += new DoWorkEventHandler(DoWorkReading);
			_bwRead.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedReading);
		}

		/// <summary>
		/// Initialize backend: background worker spawn
		/// </summary>
		private void InitializeBackgroundWorkerSpawn()
		{
			// Appoint spawn Background worker
			_bwSpawn = new BackgroundWorker();

			// Appoint spawn Background worker events
			_bwSpawn.DoWork += new DoWorkEventHandler(DoWorkSpawning);
			_bwSpawn.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedSpawning);
		}

		/// <summary>
		/// Initialize backend: background worker write
		/// </summary>
		private void InitializeBackgroundWorkerWrite()
		{
			// Appoint write Background worker
			_bwWrite = new BackgroundWorker();
			_bwWrite.WorkerSupportsCancellation = true;

			// Appoint write Background worker events
			_bwWrite.DoWork += new DoWorkEventHandler(DoWorkWriting);
			_bwWrite.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedWriting);
		}

		/// <summary>
		/// Initialize frontend: buttons, table layout panels,
		/// textboxes and other visual candy
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommunicationForm));
			this._btnInput = new System.Windows.Forms.Button();
			this._btnSpeedDown = new System.Windows.Forms.Button();
			this._btnSpeedUp = new System.Windows.Forms.Button();
			this._btnStart = new System.Windows.Forms.Button();
			this._lblSpeedValue = new System.Windows.Forms.Label();
			this._progressBarFile = new System.Windows.Forms.ProgressBar();
			this._progressBarMessages = new System.Windows.Forms.ProgressBar();
			this._tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._lblAddress = new System.Windows.Forms.Label();
			this._tbAddress = new System.Windows.Forms.TextBox();
			this._lblPort = new System.Windows.Forms.Label();
			this._tbPort = new System.Windows.Forms.TextBox();
			this._tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this._lblSpeedKey = new System.Windows.Forms.Label();
			this._tableLayoutPanel1.SuspendLayout();
			this._tableLayoutPanel2.SuspendLayout();
			this._tableLayoutPanel3.SuspendLayout();
			this._tableLayoutPanel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// _btnInput
			// 
			this._btnInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._btnInput.Location = new System.Drawing.Point(3, 3);
			this._btnInput.Name = "_btnInput";
			this._btnInput.Size = new System.Drawing.Size(309, 21);
			this._btnInput.TabIndex = 0;
			this._btnInput.Text = "Laad invoerbestand";
			this._btnInput.Click += new System.EventHandler(this.ClickInput);
			// 
			// _btnSpeedDown
			// 
			this._btnSpeedDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSpeedDown.Enabled = false;
			this._btnSpeedDown.Location = new System.Drawing.Point(159, 3);
			this._btnSpeedDown.Name = "_btnSpeedDown";
			this._btnSpeedDown.Size = new System.Drawing.Size(72, 21);
			this._btnSpeedDown.TabIndex = 2;
			this._btnSpeedDown.Text = "<";
			this._btnSpeedDown.Click += new System.EventHandler(this.ClickSpeedDown);
			// 
			// _btnSpeedUp
			// 
			this._btnSpeedUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._btnSpeedUp.Enabled = false;
			this._btnSpeedUp.Location = new System.Drawing.Point(237, 3);
			this._btnSpeedUp.Name = "_btnSpeedUp";
			this._btnSpeedUp.Size = new System.Drawing.Size(75, 21);
			this._btnSpeedUp.TabIndex = 3;
			this._btnSpeedUp.Text = ">";
			this._btnSpeedUp.Click += new System.EventHandler(this.ClickSpeedUp);
			// 
			// _btnStart
			// 
			this._btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._btnStart.Enabled = false;
			this._btnStart.Location = new System.Drawing.Point(3, 3);
			this._btnStart.Name = "_btnStart";
			this._btnStart.Size = new System.Drawing.Size(309, 21);
			this._btnStart.TabIndex = 0;
			this._btnStart.Click += new System.EventHandler(this.ClickStart);
			// 
			// _lblSpeedValue
			// 
			this._lblSpeedValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lblSpeedValue.AutoSize = true;
			this._lblSpeedValue.Location = new System.Drawing.Point(81, 7);
			this._lblSpeedValue.Name = "_lblSpeedValue";
			this._lblSpeedValue.Size = new System.Drawing.Size(13, 13);
			this._lblSpeedValue.TabIndex = 1;
			this._lblSpeedValue.Text = "1";
			// 
			// _progressBarFile
			// 
			this._progressBarFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._progressBarFile.ForeColor = System.Drawing.Color.Black;
			this._progressBarFile.Location = new System.Drawing.Point(0, 0);
			this._progressBarFile.Name = "_progressBarFile";
			this._progressBarFile.Size = new System.Drawing.Size(100, 23);
			this._progressBarFile.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBarFile.TabIndex = 0;
			// 
			// _progressBarMessages
			// 
			this._progressBarMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._progressBarMessages.ForeColor = System.Drawing.Color.Black;
			this._progressBarMessages.Location = new System.Drawing.Point(0, 0);
			this._progressBarMessages.Name = "_progressBarMessages";
			this._progressBarMessages.Size = new System.Drawing.Size(100, 23);
			this._progressBarMessages.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBarMessages.TabIndex = 0;
			// 
			// _tableLayoutPanel1
			// 
			this._tableLayoutPanel1.ColumnCount = 4;
			this._tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this._tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this._tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutPanel1.Controls.Add(this._lblAddress, 0, 0);
			this._tableLayoutPanel1.Controls.Add(this._tbAddress, 1, 0);
			this._tableLayoutPanel1.Controls.Add(this._lblPort, 2, 0);
			this._tableLayoutPanel1.Controls.Add(this._tbPort, 3, 0);
			this._tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel1.Name = "_tableLayoutPanel1";
			this._tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanel1.Size = new System.Drawing.Size(315, 24);
			this._tableLayoutPanel1.TabIndex = 0;
			// 
			// _lblAddress
			// 
			this._lblAddress.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblAddress.AutoSize = true;
			this._lblAddress.Location = new System.Drawing.Point(8, 5);
			this._lblAddress.Name = "_lblAddress";
			this._lblAddress.Size = new System.Drawing.Size(20, 13);
			this._lblAddress.TabIndex = 0;
			this._lblAddress.Text = "IP:";
			// 
			// _tbAddress
			// 
			this._tbAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._tbAddress.Location = new System.Drawing.Point(34, 3);
			this._tbAddress.Name = "_tbAddress";
			this._tbAddress.Size = new System.Drawing.Size(120, 20);
			this._tbAddress.TabIndex = 1;
			// 
			// _lblPort
			// 
			this._lblPort.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblPort.AutoSize = true;
			this._lblPort.Location = new System.Drawing.Point(197, 5);
			this._lblPort.Name = "_lblPort";
			this._lblPort.Size = new System.Drawing.Size(35, 13);
			this._lblPort.TabIndex = 2;
			this._lblPort.Text = "Poort:";
			// 
			// _tbPort
			// 
			this._tbPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._tbPort.Location = new System.Drawing.Point(238, 3);
			this._tbPort.Name = "_tbPort";
			this._tbPort.Size = new System.Drawing.Size(74, 20);
			this._tbPort.TabIndex = 3;
			this._tbPort.Text = "1337";
			// 
			// _tableLayoutPanel2
			// 
			this._tableLayoutPanel2.ColumnCount = 1;
			this._tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutPanel2.Controls.Add(this._btnInput, 0, 0);
			this._tableLayoutPanel2.Location = new System.Drawing.Point(0, 22);
			this._tableLayoutPanel2.Name = "_tableLayoutPanel2";
			this._tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel2.Size = new System.Drawing.Size(315, 27);
			this._tableLayoutPanel2.TabIndex = 1;
			// 
			// _tableLayoutPanel3
			// 
			this._tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanel3.Controls.Add(this._btnStart, 0, 0);
			this._tableLayoutPanel3.Location = new System.Drawing.Point(0, 45);
			this._tableLayoutPanel3.Name = "_tableLayoutPanel3";
			this._tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanel3.Size = new System.Drawing.Size(315, 27);
			this._tableLayoutPanel3.TabIndex = 2;
			// 
			// _tableLayoutPanel4
			// 
			this._tableLayoutPanel4.ColumnCount = 4;
			this._tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this._tableLayoutPanel4.Controls.Add(this._lblSpeedKey, 0, 0);
			this._tableLayoutPanel4.Controls.Add(this._lblSpeedValue, 1, 0);
			this._tableLayoutPanel4.Controls.Add(this._btnSpeedDown, 2, 0);
			this._tableLayoutPanel4.Controls.Add(this._btnSpeedUp, 3, 0);
			this._tableLayoutPanel4.Location = new System.Drawing.Point(0, 68);
			this._tableLayoutPanel4.Name = "_tableLayoutPanel4";
			this._tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanel4.Size = new System.Drawing.Size(315, 27);
			this._tableLayoutPanel4.TabIndex = 3;
			// 
			// _lblSpeedKey
			// 
			this._lblSpeedKey.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._lblSpeedKey.AutoSize = true;
			this._lblSpeedKey.Location = new System.Drawing.Point(24, 7);
			this._lblSpeedKey.Name = "_lblSpeedKey";
			this._lblSpeedKey.Size = new System.Drawing.Size(51, 13);
			this._lblSpeedKey.TabIndex = 0;
			this._lblSpeedKey.Text = "Snelheid:";
			// 
			// CommunicationForm
			// 
			this.ClientSize = new System.Drawing.Size(315, 95);
			this.Controls.Add(this._tableLayoutPanel1);
			this.Controls.Add(this._tableLayoutPanel2);
			this.Controls.Add(this._tableLayoutPanel3);
			this.Controls.Add(this._tableLayoutPanel4);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = global::KruispuntGroep4.Simulator.Settings.Default.Location;
			this.MaximizeBox = false;
			this.Name = "CommunicationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Communicatie - SEN Groep 4";
			this._tableLayoutPanel1.ResumeLayout(false);
			this._tableLayoutPanel1.PerformLayout();
			this._tableLayoutPanel2.ResumeLayout(false);
			this._tableLayoutPanel3.ResumeLayout(false);
			this._tableLayoutPanel4.ResumeLayout(false);
			this._tableLayoutPanel4.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		#region Public methods
		/// <summary>
		/// Set lane control, used by a view
		/// </summary>
		/// <param name="laneControl">Lane control</param>
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
		/// Override OnFormClosed, so that all views are closed as well
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
		/// Button 'Laad invoerbestand',
		/// to read JSON input file
		/// </summary>
		/// <param name="sender">Input Button</param>
		/// <param name="e">Event args</param>
		private void ClickInput(object sender, EventArgs e)
		{
			// Create open file dialog
			FileDialog dialog = new OpenFileDialog();
			DialogResult result = dialog.ShowDialog();

			// If the dialog result is OK
			if (result.Equals(DialogResult.OK))
			{
				// Remove button Input
				_tableLayoutPanel2.Controls.Remove(_btnInput);

				// Add progress bars
				_tableLayoutPanel2.ColumnCount = 2;
				_tableLayoutPanel2.Controls.Add(_progressBarFile, 0, 0);
				_tableLayoutPanel2.Controls.Add(_progressBarMessages, 1, 0);

				// Disable button Input
				_btnInput.Enabled = false;

				// Read file in the background worker file,
				// so the UI stays responsive
				_bwInput.RunWorkerAsync(dialog.FileName);
			}
		}

		/// <summary>
		/// Button to decrease multiplier times two and send multiplier
		/// </summary>
		/// <param name="sender">Speed down Button</param>
		/// <param name="e">Event args</param>
		private void ClickSpeedDown(object sender, EventArgs e)
		{
			// Decrease the multiplier times two
			_multiplier /= 2;

			// Send multiplier to host
			WriteMultiplierMessage();

			// Create sleep TimeSpan from multiplier
			long sleep = 100000000 / _multiplier;
			_timeSpanSleep = new TimeSpan(sleep);

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
		private void ClickSpeedUp(object sender, EventArgs e)
		{
			// Increase the multiplier times two
			_multiplier *= 2;

			// Send multiplier to host
			WriteMultiplierMessage();

			// Create sleep TimeSpan from multiplier
			long sleep = 100000000 / _multiplier;
			_timeSpanSleep = new TimeSpan(sleep);

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
		private void ClickStart(object sender, EventArgs e)
		{
			// If the button reads 'Start simulatie'
			if (_btnStart.Text.Equals(Strings.ViewStart))
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
				// background worker write,
				// so the UI stays responsive
				_bwWrite.CancelAsync();
			}
		}

		/// <summary>
		/// Create an IP address
		/// </summary>
		private string CreateAddress()
		{
			// Initialize IP address and create host;
			string address = Strings.Address;
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			// For each IP address, check if it's an inter network address
			foreach (IPAddress ip in host.AddressList)
			{
				// If the address family equals 'InterNetwork'
				if (ip.AddressFamily.ToString().Equals(Strings.Network))
				{
					// Set the address
					address = ip.ToString();
				}
			}

			// Return the address
			return address;
		}

		/// <summary>
		/// Read the JSON input file
		/// </summary>
		/// <param name="sender">Background worker input</param>
		/// <param name="e">Do work event args</param>
		private void DoWorkInput(object sender, DoWorkEventArgs e)
		{
			// Read JSON from selected file
			_json = File.ReadAllLines(e.Argument.ToString());
			int count = _json.Length;

			for (int i = 0; i < count; i++)
			{
				// Create percentage from i and count
				float percentage = ((float)i / (float)_json.Length) * 100f;

				// Report the user the progress of reading a file
				_bwInput.ReportProgress((int)percentage, Strings.ProgressBarFile);
			}

			// Report the user the maximum progress of reading a file
			_bwInput.ReportProgress(_progressBarFile.Maximum, Strings.ProgressBarFile);

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

			// Report the user the maximum progress of converting a JSON array to messages
			_bwInput.ReportProgress(_progressBarMessages.Maximum, Strings.ProgressBarMessages);
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
		/// Spawn vehicles
		/// </summary>
		/// <param name="sender">Background worker spawn</param>
		/// <param name="e">Do work event args</param>
		private void DoWorkSpawning(object sender, DoWorkEventArgs e)
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
					case Strings.VehicleBicycle: /* It is a bicycle */
						vehicleType = VehicleTypeEnum.bicycle;
						break;
					case Strings.VehicleBus: /* It is a bus */
						vehicleType = VehicleTypeEnum.bus;
						break;
					case Strings.VehicleCar: /* It is a car */
						vehicleType = VehicleTypeEnum.car;
						break;
					case Strings.VehiclePedestrian: /* It is a pedestrian */
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
					// Sleep for a maximum of 10 seconds,
					// depending on multiplier value
					Thread.Sleep(_timeSpanSleep);
				}

				// The previous time is time
				previousTime = time;
			}
		}

		/// <summary>
		/// Connect to the host
		/// </summary>
		/// <param name="sender">Background worker write</param>
		/// <param name="e">Tuple with address string and port int</param>
		private void DoWorkWriting(object sender, DoWorkEventArgs e)
		{
			// Create host tuple
			Tuple<string, int> host = e.Argument as Tuple<string, int>;

			// Connect while there is no TCP client created
			while (_client == null)
			{
				// Try connecting
				try
				{
					// If the user requested cancellation,
					// set Cancel property and break
					if (_bwWrite.CancellationPending)
					{
						e.Cancel = true;
						break;
					}

					// Connecting to specified host address and port
					_client = new TcpClient(host.Item1, host.Item2);
				}
				catch (SocketException) /* When there is no connection possible */
				{
					// Sleep for a second, so the error spam is limited
					Thread.Sleep(1000);
				}
			}

			// If the cancel property is false
			if (!e.Cancel)
			{
				// Read messages in the background, so the UI stays responsive
				_bwRead.RunWorkerAsync();
			}
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

				// Create percentage from i and count
				float percentage = ((float)i / (float)count) * 100f;

				// Report the user the progress of converting a JSON array to messages
				_bwInput.ReportProgress((int)percentage, Strings.ProgressBarMessages);
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
		/// Display the progress in the specified progress bar
		/// </summary>
		/// <param name="sender">Background worker input</param>
		/// <param name="e">Specified progress bar as property user state and progress percentage</param>
		private void ProgressChangedInput(object sender, ProgressChangedEventArgs e)
		{
			// Which progress bar is it?
			switch (e.UserState.ToString())
			{
				case Strings.ProgressBarFile: /* It is the file Progress bar */
					// Display progress in the file Progress bar
					_progressBarFile.Value = e.ProgressPercentage;
					break;
				case Strings.ProgressBarMessages: /* It is the messages Progress bar */
					// Display progress in the messages Progress bar
					_progressBarMessages.Value = e.ProgressPercentage;
					break;
			}
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
				}
			}

			// Return the message
			return message;
		}

		/// <summary>
		/// Enable button Start
		/// </summary>
		/// <param name="sender">Background worker input</param>
		/// <param name="e">Run worker completed event args</param>
		private void RunWorkerCompletedInput(object sender, RunWorkerCompletedEventArgs e)
		{
			// Remove progress bars
			_tableLayoutPanel2.ColumnCount = 1;
			_tableLayoutPanel2.Controls.Remove(_progressBarFile);
			_tableLayoutPanel2.Controls.Remove(_progressBarMessages);

			// Add button Input
			_tableLayoutPanel2.Controls.Add(_btnInput);

			// Enable button Start
			_btnStart.Enabled = true;
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
			string message = e.Result.ToString();

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
		/// Display a message saying all input JSONs are sent
		/// </summary>
		/// <param name="sender">Background worker spawn</param>
		/// <param name="e">Run worker completed event args</param>
		private void RunWorkerCompletedSpawning(object sender, RunWorkerCompletedEventArgs e)
		{
			// Disable speed down/up Buttons
			_btnSpeedDown.Enabled = false;
			_btnSpeedUp.Enabled = false;
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

				// Spawn all vehicles from the JSON input file
				// in the background worker spawn,
				// so the UI stays responsive
				_bwSpawn.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Switch the text of button Start
		/// </summary>
		private void SwitchTextButtonStart()
		{
			// If the button start text isn't 'Start simulatie'
			if (!_btnStart.Text.Equals(Strings.ViewStart))
			{
				// The button start text is 'Start simulatie'
				_btnStart.Text = Strings.ViewStart;
			}
			else /* Else if the button start text is 'Start simulatie' */
			{
				// The button start text is 'Stop simulatie'
				_btnStart.Text = Strings.ViewStop;
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