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
        public DataSet ds;
        public DataTable dt;
        public string XmlDirectory;
        public int currRow;

        public Form1()
        {
            InitializeComponent();
            this.ds = new DataSet();
            this.dt = null;
            this.XmlDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\My Documents\\GW2Data.xml";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Populate the datagridview
            PopulateData();
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            //Login with the selected user account
            Login();

            //Reload the XML data so you don't have to close and reopen the application to see the changes
            PopulateData();
        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            int currentRowIndex;

            //Get the currently selected row for later recall
            if (dgv_Accounts.SelectedRows.Count > 0) { currentRowIndex = dgv_Accounts.SelectedRows[0].Index; } else { return; }

            //Instantiate a new "Edit_Account" object and pass in the variables and parent form
            Edit_Account edit_account = new Edit_Account(this);
            edit_account.dtRow = currentRowIndex;
            edit_account.acctName = this.dt.Rows[currentRowIndex][0].ToString();
            edit_account.email = this.dt.Rows[currentRowIndex][1].ToString();
            edit_account.password = this.dt.Rows[currentRowIndex][2].ToString();
            edit_account.server = this.dt.Rows[currentRowIndex][3].ToString();
            edit_account.prodKey = this.dt.Rows[currentRowIndex][4].ToString();
            edit_account.apiKey = null;
            edit_account.ShowDialog();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            //Create the Add_Account object and pass in the parent form (this)
            Add_Account Add_Account = new Add_Account(this);
            Add_Account.Show();
        }

        public void PopulateData()
        {
            XmlReader xmlreader;

            //Get the currently selected row for later recall
            if (dgv_Accounts.SelectedRows.Count > 0 && this.currRow != 0) { currRow = dgv_Accounts.SelectedRows[0].Index; }

            //Clear out the data table and data grid
            if (this.ds.Tables.Count > 0 && this.ds.Tables[0] != null) { this.ds.Tables[0].Clear(); }
            if (dgv_Accounts.DataSource != null) { dgv_Accounts.DataSource = null; }
            if (dgv_Accounts.Rows.Count > 0) { dgv_Accounts.Rows.Clear(); }

            //Re-Populate the dataset from GW2Data.xml
            if (File.Exists(this.XmlDirectory))
            {
                xmlreader = XmlReader.Create(this.XmlDirectory, new XmlReaderSettings());
                this.ds.ReadXml(xmlreader);
            }
            else { xmlreader = null; return; }

            //Populate a datatable for use in the datagridview
            if (xmlreader != null) { this.dt = ds.Tables[0]; }
            else { this.ds = null; this.dt = null; return; }

            //Format and populate the datagrid
            buildDataGridView(dgv_Accounts);

            //Re-Select the row that was selected before the refresh command
            if (currRow != 0) { dgv_Accounts.Rows[currRow].Selected = true; }

            //Auto-Scroll to the selected row in the datagrid (If there is a scrollbar)
            dgv_Accounts.FirstDisplayedScrollingRowIndex = currRow;

            //Clean and dispose of the xmlreader
            destroyXmlReader(xmlreader);
        }

        private void destroyXmlReader(XmlReader xmlreader)
        {
            if (xmlreader != null)
            { xmlreader.Close(); xmlreader.Dispose(); }
        }

        private void buildDataGridView(DataGridView dgv_Accounts)
        {
            if (dgv_Accounts.DataSource == null && this.dt != null)
            {
                //Create the columns for the datagrid
                dgv_Accounts.ColumnCount = 3;
                dgv_Accounts.Columns[0].Name = "Number";
                dgv_Accounts.Columns[1].Name = "Account Info";
                dgv_Accounts.Columns[2].Name = "Last Login";

                //Wrap the columns
                dgv_Accounts.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                //Format the columns
                dgv_Accounts.Columns[0].Width = 50;

                //Loop through the data rows in the data table and populate the datagridview
                foreach (DataRow dr in this.dt.Rows)
                {
                    //Capture the data from the datatable
                    int acctNumber = (this.dt.Rows.IndexOf(dr) + 1); //Generate an account number
                    string acctName = dr["Name"].ToString();
                    string acctEmail = dr["Email"].ToString();
                    string acctPassword = dr["Password"].ToString();
                    string server = dr["Server"].ToString();
                    string prodKey = dr["Key"].ToString();
                    string lastLogin = dr["LastLogin"].ToString();

                    //Format the datagrid rows and add them to the datagrid
                    string column1 = acctName + Environment.NewLine + server + Environment.NewLine + acctEmail + Environment.NewLine + prodKey;
                    string[] row = new string[] { acctNumber.ToString(), column1, lastLogin };
                    dgv_Accounts.Rows.Add(row);
                }
            }
            else { return; }
        }

        private void Login()
        {
            int currentRowIndex;

            //Get the currently selected row for later recall
            if (dgv_Accounts.SelectedRows.Count > 0) { currentRowIndex = dgv_Accounts.SelectedRows[0].Index; } else { return; }

            //Retrieve data from the datatable, not the datagrid.  It allows us to get the 'Hidden' values
            string email = this.dt.Rows[currentRowIndex][1].ToString();
            string password = this.dt.Rows[currentRowIndex][2].ToString();
            string lastLogin = this.dt.Rows[currentRowIndex][5].ToString();
            string updatedDate = DateTime.Now.ToString();

            //Assign the CMD prompt command and launch
            string loginCommand = "/C start \"\" \"C:\\Program Files (x86)\\Guild Wars 2\\Gw2.exe\" -email \"" + email + "\" -password \"" + password + "\" -nopatchui";
            System.Diagnostics.Process.Start(@"C:\Windows\System32\CMD.exe", loginCommand);

            //Update and write the 'LastLogin' time to the XML file
            this.dt.Rows[currentRowIndex][5] = updatedDate.ToString();
            this.ds.AcceptChanges();
            this.ds.WriteXml(this.XmlDirectory);

            //Set the current row to reload to
            this.currRow = currentRowIndex;
        }
    }
}
