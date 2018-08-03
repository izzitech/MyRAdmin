using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

namespace MyRAdmin {
	public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

		Data data;

		string _IP;
        int _Port;
        string _Filter;

        DataView dv;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (CheckThings.RAdminIsOk() != 0)
            {
                MessageBox.Show("Error with RAdmin files...");
                return;
            }

			data = new Data();

            dv = new DataView(data._hosts);
            dataGridView1.DataSource = dv;


			foreach (DataGridViewRow row in dataGridView1.Rows) {
				try {
					PingIt(row.Cells["IP"].Value.ToString(), (int)row.Cells["Radmin Port"].Value);
				}
				catch (Exception ex) {
					dataGridView1.Rows[row.Index].Cells["Details"].Value = $"Error: {ex.Message}";
					dataGridView1.Rows[row.Index].Cells["Details"].Style.BackColor = Color.LightPink;
				}
			}
			
			System.Windows.Forms.Timer recolorDataGridView1 = new System.Windows.Forms.Timer();
			recolorDataGridView1.Interval = 1000;
			recolorDataGridView1.Tick += RecolorDataGridView1_Tick;
			recolorDataGridView1.Start();
		}

		private void RecolorDataGridView1_Tick(object sender, EventArgs e)
		{
				DataGridViewCheckBoxCell IPcell;
				DataGridViewCheckBoxCell PortCell;
				foreach (DataGridViewRow row in dataGridView1.Rows) {
					IPcell = row.Cells["IPIsUp"] as DataGridViewCheckBoxCell;
					PortCell = row.Cells["PortIsUp"] as DataGridViewCheckBoxCell;

					IPcell.FalseValue = Boolean.FalseString;
					IPcell.TrueValue = Boolean.TrueString;

					PortCell.FalseValue = Boolean.FalseString;
					PortCell.TrueValue = Boolean.TrueString;

				try {
					if (IPcell != null && IPcell.Value != null) {
						if (IPcell.Value.ToString() == Boolean.TrueString) {
							dataGridView1.Rows[row.Index].Cells["IP"].Style.BackColor = Color.LightGreen;
						} else {
							dataGridView1.Rows[row.Index].Cells["IP"].Style.BackColor = Color.LightPink;
						}
					}

					if (PortCell != null && PortCell.Value != null) {
						if (PortCell.Value.ToString() == Boolean.TrueString) {
							dataGridView1.Rows[row.Index].Cells["Radmin Port"].Style.BackColor = Color.LightGreen;
						} else {
							dataGridView1.Rows[row.Index].Cells["Radmin Port"].Style.BackColor = Color.LightPink;
						}
					}
				}
				catch { }
				}
		}

		private void PingIt(string ip, int port)
		{
			Action action = () => {
				Utils utils = new Utils();
				while (true) {
					bool isUp = false;
					isUp = utils.IsUp(ip);
					bool isPortUp = false;
					isPortUp = utils.IsPortUp(ip, port);

					data.SetIpStatus(ip, isUp);
					data.SetPortStatus(ip, port, isPortUp);
					Thread.Sleep(500);
				}
			};

			Task task = new Task(action);
			task.Start();
		}

		private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;

            _Filter = "";
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            Process.Start("Externals\\Radmin.exe", String.Format("/connect:{0}:{1}", _IP, _Port));

            Process[] process = Process.GetProcessesByName(String.Format("Radmin security: {0}", _IP));

            if (process.Length > 0)
            {
                process = Process.GetProcessesByName(String.Format("Radmin security: {0}", _IP));

                SetForegroundWindow(process[0].MainWindowHandle);
                SendKeys.Send("s");

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) SetData();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) SetData();
        }

        private void SetData()
        {
			try {
				_IP = dataGridView1.SelectedRows[0].Cells["IP"].Value.ToString();
				_Port = (int)dataGridView1.SelectedRows[0].Cells["Radmin Port"].Value;
				}
			catch (Exception ex) {
				dataGridView1.SelectedRows[0].Cells["Details"].Value = $"Error: {ex.Message}";
				dataGridView1.SelectedRows[0].Cells["Details"].Style.BackColor = Color.LightPink;
			}
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = true;

            _Filter = String.Format("Tipo LIKE '%{0}%'", textBox6.Text.Trim());
            dv.RowFilter = _Filter;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = true;

            _Filter = String.Format("Name LIKE '%{0}%'", textBox1.Text.Trim());
            dv.RowFilter = _Filter;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            radioButton3.Checked = true;

            _Filter = String.Format("IP LIKE '%{0}%'", textBox2.Text.Trim());
            dv.RowFilter = _Filter;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            radioButton4.Checked = true;

            _Filter = String.Format("ID LIKE '%{0}%'", textBox3.Text.Trim());
            dv.RowFilter = _Filter;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            radioButton5.Checked = true;

            _Filter = String.Format("Sector LIKE '%{0}%'", textBox4.Text.Trim());
            dv.RowFilter = _Filter;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            radioButton6.Checked = true;

            _Filter = String.Format("Details LIKE '%{0}%'", textBox5.Text.Trim());
            dv.RowFilter = _Filter;
        }

		private void button7_Click(object sender, EventArgs e)
		{
			data.WriteRaHostsXML();
		}

		private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{

		}
	}

	static class CheckThings
    {
        public static int RAdminIsOk()
        {
            if (File.Exists("Externals\\Radmin.exe") && File.Exists("Externals\\WinLpcDl.dll"))
            {
                return 0;
            }

            return -1;
        }
    }

	class Data
    {
		public DataTable _hosts = new DataTable("Hosts");

		public void SetIpStatus(string ip, bool status )
		{
			// Try-catch because it throws an exception of "out of index" but debug says otherwise.
			try {
				DataRow[] HRow = _hosts.Select($"IP = '{ip}'");
				if (HRow.Length < 1) return;
				HRow[0]["IPIsUp"] = status;
			}
			catch { }
		}

		public void SetPortStatus(string ip, int port, bool status)
		{
			// Try-catch because it throws an exception of "out of index" but debug says otherwise.
			try {
				DataRow[] HRow = _hosts.Select($"IP = '{ip}'");
				if (HRow.Length < 1) return;
				HRow[0]["PortIsUp"] = status;
			}
			catch { }
		}

		public void ReadRaHostsXML()
		{
			var xmlSerializer = new XmlSerializer(typeof(DataTable));
			var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

			try {
				var file = File.OpenRead(path);
				_hosts = xmlSerializer.Deserialize(file) as DataTable;
				file.Close();
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine($"Error al leer xml: {ex.Message}");
			}
		}

		public void WriteRaHostsXML()
		{
			
			var xmlSerializer = new XmlSerializer(typeof(DataTable));
			var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";


			try {
				var file = File.Create(path);

				xmlSerializer.Serialize(file, _hosts);
				file.Close();
			}
			catch { }
			
		}

		public Data()
		{
			ReadRaHostsXML();

			if (_hosts.Rows.Count > 0) return;

			// If _hosts is null, fill it with mock data.
			DataColumn column;

			column = new DataColumn();
			column.ColumnName = "Tipo";
			column.DataType = System.Type.GetType("System.String");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "ID";
			column.DataType = System.Type.GetType("System.String");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "Name";
			column.DataType = System.Type.GetType("System.String");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "IP";
			column.DataType = System.Type.GetType("System.String");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "IPIsUp";
			column.DataType = System.Type.GetType("System.Boolean");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "Radmin Port";
			column.DataType = System.Type.GetType("System.Int32");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "PortIsUP";
			column.DataType = System.Type.GetType("System.Boolean");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "Sector";
			column.DataType = System.Type.GetType("System.String");
			_hosts.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "Details";
			column.DataType = System.Type.GetType("System.String");
			_hosts.Columns.Add(column);

			var row = _hosts.NewRow();
			row["Tipo"] = "PC";
			row["ID"] = "PC27";
			row["Name"] = "PIT";
			row["IP"] = "172.100.1.206";
			row["Radmin Port"] = "6206";
			row["Sector"] = "IT";
			row["Details"] = "";
			_hosts.Rows.Add(row);

			row = _hosts.NewRow();
			row["Tipo"] = "Server";
			row["ID"] = "MAIN";
			row["Name"] = "Server Mon";
			row["IP"] = "172.100.1.200";
			row["Radmin Port"] = "6200";
			row["Sector"] = "Datacenter";
			row["Details"] = "";
			_hosts.Rows.Add(row);
		}
	}
}
