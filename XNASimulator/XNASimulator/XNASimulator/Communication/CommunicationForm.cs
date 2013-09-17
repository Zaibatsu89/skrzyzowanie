#region Usings
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using KruispuntGroep4.Globals;
using KruispuntGroep4.Simulator.Main;
using KruispuntGroep4.Simulator.ObjectControllers;
#endregion

namespace KruispuntGroep4.Simulator.Communication
{
	class CommunicationForm : Form
	{
		#region Constructor, private attributes, dispose and initializing methods
		public CommunicationForm()
		{
			InitializeComponent();
		}

		private Button _btnSend, _btnStart;
		private IContainer _components = null;
		private LaneControl _laneControl;
		private TableLayoutPanel _tableLayoutPanel1,
			_tableLayoutPanel2, _tableLayoutPanel3,
			_tableLayoutPanel4;
		private TextBox _tbConsole, _tbIp,
			_tbMessage, _tbPort;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			_btnSend = new Button();
			_btnStart = new Button();
			_tableLayoutPanel1 = new TableLayoutPanel();
			_tableLayoutPanel1.SuspendLayout();
			_tableLayoutPanel2 = new TableLayoutPanel();
			_tableLayoutPanel2.SuspendLayout();
			_tableLayoutPanel3 = new TableLayoutPanel();
			_tableLayoutPanel3.SuspendLayout();
			_tableLayoutPanel4 = new TableLayoutPanel();
			_tableLayoutPanel4.SuspendLayout();
			_tbConsole = new TextBox();
			_tbIp = new TextBox();
			_tbMessage = new TextBox();
			_tbPort = new TextBox();

			SuspendLayout();

			//  
			// lblIp
			//  
			Label lblIp = new Label();
			lblIp.Anchor = AnchorStyles.Right;
			lblIp.AutoSize = true;
			lblIp.Text = Strings.AddressKey;
			//  
			// lblPort
			//  
			Label lblPort = new Label();
			lblPort.Anchor = AnchorStyles.Right;
			lblPort.AutoSize = true;
			lblPort.Text = Strings.PortKey;
			//  
			// _tableLayoutPanel1 
			//  
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
			_tableLayoutPanel1.Controls.Add(lblIp, 0, 0);
			_tableLayoutPanel1.Controls.Add(_tbIp, 1, 0);
			_tableLayoutPanel1.Controls.Add(lblPort, 2, 0);
			_tableLayoutPanel1.Controls.Add(_tbPort, 3, 0);
			_tableLayoutPanel1.Location = new Point(0, 0);
			_tableLayoutPanel1.RowCount = 1;
			_tableLayoutPanel1.Size = new Size(315, 23);
			//  
			// _tbIp
			//  
			_tbIp.Anchor =
				(AnchorStyles)((AnchorStyles.Left |
				AnchorStyles.Right));
			_tbIp.TabIndex = 0;
			_tbIp.Text = Strings.AddressValue;
			//  
			// _tbPort
			//  
			_tbPort.Anchor =
				(AnchorStyles)((AnchorStyles.Left |
				AnchorStyles.Right));
			_tbPort.TabIndex = 1;
			_tbPort.Text = Strings.PortValue;
			//  
			// _tableLayoutPanel2 
			//  
			_tableLayoutPanel2.Anchor =
				((AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right)));
			_tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_tableLayoutPanel2.ColumnCount = 1;
			_tableLayoutPanel2.Controls.Add(_btnStart, 0, 0);
			_tableLayoutPanel2.Location = new Point(0, 23);
			_tableLayoutPanel2.RowCount = 1;
			_tableLayoutPanel2.Size = new Size(315, 30);
			//  
			// _btnStart 
			//  
			_btnStart.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnStart.Click += new EventHandler(_btnStart_Click);
			_btnStart.TabIndex = 2;
			_btnStart.Text = Strings.StartView;
			//  
			// _tableLayoutPanel3 
			//  
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
			_tableLayoutPanel3.RowCount = 1;
			_tableLayoutPanel3.Size = new Size(315, 30);
			//  
			// _tbMessage
			//  
			_tbMessage.Anchor =
				(AnchorStyles)(((AnchorStyles.Bottom |
				AnchorStyles.Left) |
				AnchorStyles.Right));
			_tbMessage.TabIndex = 3;
			//  
			// _btnSend 
			//  
			_btnSend.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_btnSend.Click += new EventHandler(_btnSend_Click);
			_btnSend.TabIndex = 4;
			_btnSend.Text = Strings.Send;
			//  
			// _tableLayoutPanel4 
			//  
			_tableLayoutPanel4.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tableLayoutPanel4.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_tableLayoutPanel4.ColumnCount = 1;
			_tableLayoutPanel4.Controls.Add(_tbConsole, 0, 0);
			_tableLayoutPanel4.Location = new Point(0, 83);
			_tableLayoutPanel4.RowCount = 1;
			_tableLayoutPanel4.Size = new Size(315, 635);
			//  
			// _tbConsole
			//  
			_tbConsole.Anchor =
				(AnchorStyles)((((AnchorStyles.Top |
				AnchorStyles.Bottom) | AnchorStyles.Left) |
				AnchorStyles.Right));
			_tbConsole.Multiline = true;
			_tbConsole.ScrollBars = ScrollBars.Vertical;
			_tbConsole.TabIndex = 5;
			// 
			// CommunicationForm
			// 
			AutoScaleDimensions = new SizeF(120F, 120F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(new Point(315, 732));
			ComponentResourceManager resources = new ComponentResourceManager(typeof(CommunicationForm));
			Controls.Add(_tableLayoutPanel4);
			Controls.Add(_tableLayoutPanel3);
			Controls.Add(_tableLayoutPanel2);
			Controls.Add(_tableLayoutPanel1);
			FormBorderStyle = FormBorderStyle.Fixed3D;
			Icon = (Icon)(resources.GetObject(Strings.Icon));
			MaximizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			_tableLayoutPanel1.ResumeLayout(false);
			_tableLayoutPanel1.PerformLayout();
			_tableLayoutPanel2.ResumeLayout(false);
			_tableLayoutPanel2.PerformLayout();
			_tableLayoutPanel3.ResumeLayout(false);
			_tableLayoutPanel3.PerformLayout();
			_tableLayoutPanel4.ResumeLayout(false);
			Text = Strings.TitleCommunication;

			ResumeLayout(false);
		}
		#endregion

		#region Events
		private void _btnSend_Click(object sender, EventArgs e)
		{
			if (_tbMessage.Text.Length > 0)
			{
				SetText(Strings.Sent + _tbMessage.Text);
			}
		}

		private void _btnStart_Click(object sender, EventArgs e)
		{
			Thread thread = new Thread(() =>
			{
				new MainGame(this).Run();
			});

			thread.Start();

			//TODO: connect to simulator at given address and port
		}
		#endregion

		#region Public methods
		public void SetLaneControl(LaneControl laneControl)
		{
			_laneControl = laneControl;
		}
		#endregion

		#region Private methods
		private void ChangeLight()
        {
            _laneControl.ChangeTrafficLight(Strings.TrafficLightValue, Strings.SpawnLaneID);
        }

		private void SetText(string text)
		{
			DateTime date = DateTime.Now;
			string time = date.ToString(Strings.Time);

			_tbConsole.AppendText(
				time +
				Strings.Space +
				text +
				Environment.NewLine
			);
		}

        private void SpawnVehicle()
        {
            _laneControl.SpawnVehicle(Strings.VehicleType, Strings.SpawnLaneID, Strings.DestinationLaneID);
		}
		#endregion
	}
}