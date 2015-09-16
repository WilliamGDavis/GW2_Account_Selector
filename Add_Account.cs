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
    public partial class Add_Account : Form
    {
        Form1 Form1;

        public Add_Account(Form1 parent)
        {
            InitializeComponent();
            this.Form1 = parent;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string acctName = txt_AcctName.Text.ToString();
            string email = txt_Email.Text.ToString();
            string password = txt_Password.Text.ToString();
            string server = txt_Server.Text.ToString();
            string prodKey = txt_ProdKey.Text.ToString();
            string LastLogin = null;

            Form1.dt.Rows.Add(acctName, email, password, server, prodKey, LastLogin);

            //Write the changes back to GW2Data
            this.Form1.ds.AcceptChanges();
            this.Form1.ds.WriteXml(Form1.XmlDirectory);

            //Refresh the datagrid and close the popup
            Form1.PopulateData();
            this.Close();
        }
    }
}
