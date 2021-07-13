using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLQueryEditor
{
    public partial class SQLQueryEditor : Form
    {
        public bool isDone { get; private set; } //Var for handling the second form opening

        public SQLQueryEditor()
        {
            isDone = false;
            InitializeComponent();
        }

        
        //EventHandler of Delete button. It deletes all data inserted into the TextBoxes 
        private void Delete_Click(object sender, EventArgs e)
        {
            this.servername_box.Text = "";
            this.username_box.Text = "";
            this.password_box.Text = "";
            this.dbname_box.Text = "";

        }

        /*
         * HeventHandler for the Connect button. It uses DBManager for establishing a connection with the
         * params specified
         */
        private void Connect_Click(object sender, EventArgs e)
        {
            Delete.Enabled = false;
            Connect.Enabled = false;
            Conn_label.Visible = true;

            string server_name = this.servername_box.Text;
            string user_name = this.username_box.Text;
            string password = this.password_box.Text;
            string db_name = this.dbname_box.Text;

            DBManager db = DBManager.GetIstance();

            Task.Run(() =>
            {
                if (db.Connect(db_name, server_name, user_name, password))
                {
                    isDone = true;
                    this.Invoke((MethodInvoker)delegate
                   {
                       Close();
                   });

                }
                else
                    this.Invoke((MethodInvoker)delegate
                   {
                       MessageBox.Show(
                           "Unable to connect.\nPLease, check the params and your connection!",
                           "Error",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error
                       );

                       Delete.Enabled = true;
                       Connect.Enabled = true;
                       Conn_label.Visible = false;
                   });
            });
        }
    }
}
