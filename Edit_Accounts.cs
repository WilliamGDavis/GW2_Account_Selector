using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuildWars2_Launcher
{
    public partial class Edit_Account : Form
    {
        Form1 Form1;
        //public string acctID;
        public int dtRow;
        public string acctName;
        public string email;
        public string password;
        public string server;
        public string apiKey;
        public string prodKey;

        public Edit_Account(Form1 parent)
        {
            InitializeComponent();
            this.Form1 = parent;
        }

        private void Edit_Account_Load(object sender, EventArgs e)
        {
            //Populate the textboxes from the properties
            txt_AcctName.Text = this.acctName;
            txt_Email.Text = this.email;
            txt_Password.Text = this.password;
            txt_Server.Text = this.server;
            txt_ApiKey.Text = this.apiKey;
            txt_ProdKey.Text = this.prodKey;
        }


        private void Clear_Values(object sender, FormClosedEventArgs e)
        {
            //Clear the properties when closing out of the window
            //this.acctID = null;
            this.dtRow = 0;
            this.acctName = null;
            this.email = null;
            this.password = null;
            this.server = null;
            this.apiKey = null;
            this.prodKey = null;
        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            string acctName = txt_AcctName.Text.ToString();
            string email = txt_Email.Text.ToString();
            string password = txt_Password.Text.ToString();
            string server = txt_Server.Text.ToString();
            string prodKey = txt_ProdKey.Text.ToString();

            //Update the datatable with the input data
            //TODO: Sanitize
            this.Form1.dt.Rows[this.dtRow][0] = acctName;
            this.Form1.dt.Rows[this.dtRow][1] = email;
            this.Form1.dt.Rows[this.dtRow][2] = password;
            this.Form1.dt.Rows[this.dtRow][3] = server;
            this.Form1.dt.Rows[this.dtRow][4] = prodKey;

            //Write the changes back to GW2Data
            this.Form1.ds.AcceptChanges();
            this.Form1.ds.WriteXml(Form1.XmlDirectory);

            //Set the scroll row, Refresh the datagrid, and close the current form
            Form1.currRow = this.dtRow;
            Form1.PopulateData();
            this.Close();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            //Delete the row from the datatable
            this.Form1.dt.Rows[this.dtRow].Delete();

            //Write the changes back to GW2Data
            this.Form1.ds.AcceptChanges();
            this.Form1.ds.WriteXml(Form1.XmlDirectory);

            this.Form1.currRow = 0;

            //Refresh the datagrid and close the popup
            Form1.PopulateData();
            this.Close();
        }
        
    }
}
