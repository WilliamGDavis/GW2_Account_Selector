using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace GuildWars2_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataSet ds = new DataSet();
        DataTable dt;
        string XmlDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\My Documents\\GW2Data.xml";

        private void Form1_Load(object sender, EventArgs e)
        {
            //Create a new XML Reader and fill a dataset
            XmlReader xmlreader = XmlReader.Create(XmlDirectory, new XmlReaderSettings());
            this.ds.ReadXml(xmlreader);
            this.dt = ds.Tables[0]; //Retrieve the data table from the dataset

            //Create the columns for the datagrid
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Number";
            dataGridView1.Columns[1].Name = "Account Info";
            dataGridView1.Columns[2].Name = "Last Login";

            //Wrap the columns
            dataGridView1.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            //Format the columns
            dataGridView1.Columns[0].Width = 50;

            //Loop through the data rows in the data table
            foreach (DataRow dr in this.dt.Rows)
            {
                //Capture the data from the datatable
                string acctNumber = dr["Number"].ToString();
                string acctName = dr["Name"].ToString();
                string acctEmail = dr["Email"].ToString();
                string acctPassword = dr["Password"].ToString();
                string server = dr["Server"].ToString();
                string prodKey = dr["Key"].ToString();
                string lastLogin = dr["LastLogin"].ToString();

                //Format the datagrid rows and add them to the datagrid
                string column1 = acctName + Environment.NewLine + server + Environment.NewLine + acctEmail + Environment.NewLine + prodKey;
                string[] row = new string[] { acctNumber, column1, lastLogin };
                dataGridView1.Rows.Add(row);
            }

            //Clean and dispose of the xmlreader
            if (xmlreader != null)
            {
                xmlreader.Close();
                xmlreader.Dispose();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Retrieve data from the datatable, not the datagrid.  It allows us to get the 'Hidden' values
            int currentIndex = dataGridView1.SelectedRows[0].Index;
            string email = this.dt.Rows[currentIndex][2].ToString();
            string password = this.dt.Rows[currentIndex][3].ToString();
            string lastLogin = this.dt.Rows[currentIndex][6].ToString();
            string updatedDate = DateTime.Now.ToString();
            
            //Assign the CMD prompt command and launch
            string loginCommand = "/C start \"\" \"C:\\Program Files (x86)\\Guild Wars 2\\Gw2.exe\" -email \"" + email + "\" -password \"" + password + "\" -nopatchui";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\CMD.exe", loginCommand);

            //Update and write the 'LastLogin' time to the XML file
            this.dt.Rows[currentIndex][6] = updatedDate.ToString();
            this.ds.AcceptChanges();
            this.ds.WriteXml(XmlDirectory);
            
            //Reload the XML data so you don't have to close and reopen the application to see the changes
            RefreshData();
        }

        private void RefreshData()
        {
            //Get the currently selected row for later recall
            int currRow = dataGridView1.SelectedRows[0].Index;
            //Clear out the data table and data grid
            this.ds.Tables[0].Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();

            //Re-Populate the dataset and data table
            XmlReader xmlreader = XmlReader.Create(this.XmlDirectory, new XmlReaderSettings());
            this.ds.ReadXml(xmlreader);
            this.dt = ds.Tables[0];

            //Create the columns for the datagrid
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Number";
            dataGridView1.Columns[1].Name = "Account Info";
            dataGridView1.Columns[2].Name = "Last Login";

            //Wrap the columns
            dataGridView1.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            //Format the columns
            dataGridView1.Columns[0].Width = 50;

            //Loop through the data rows in the data table
            foreach (DataRow dr in this.dt.Rows)
            {
                //Capture the data from the datatable
                string acctNumber = dr["Number"].ToString();
                string acctName = dr["Name"].ToString();
                string acctEmail = dr["Email"].ToString();
                string acctPassword = dr["Password"].ToString();
                string server = dr["Server"].ToString();
                string prodKey = dr["Key"].ToString();
                string lastLogin = dr["LastLogin"].ToString();

                //Format the datagrid rows and add them to the datagrid
                string column1 = acctName + Environment.NewLine + server + Environment.NewLine + acctEmail + Environment.NewLine + prodKey;
                string[] row = new string[] { acctNumber, column1, lastLogin };
                dataGridView1.Rows.Add(row);
            }

            //Re-Select the row that was selected before the refresh command
            dataGridView1.Rows[currRow].Selected = true;

            //Clean and dispose of the xmlreader
            if (xmlreader != null)
            {
                xmlreader.Close();
                xmlreader.Dispose();
            }
        }
    }
}

