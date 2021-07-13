using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SQLQueryEditor
{
    public partial class MainWin : Form
    {
        private DBManager db = DBManager.GetIstance();
        public MainWin()
        {

            InitializeComponent();

        }


        private void exec_query_Click(object sender, EventArgs e)
        {
            info_label.ForeColor = Color.Black;
            info_label.Text = "Executing ...";
            Task.Run(() =>
            {
                try
                {
                    Tuple<List<string>, List<string[]>> col_data = db.ExecuteQuery(query_box.Text);

                    List<string> col = col_data.Item1;
                    List<string[]> res = col_data.Item2;

                    visualizzatore_risultati.Invoke((MethodInvoker)delegate
                    {
                        visualizzatore_risultati.Clear();

                        foreach (string c in col)
                            visualizzatore_risultati.Columns.Add(c);

                        foreach (string[] r in res)
                        {
                            ListViewItem item = new ListViewItem(r);
                            visualizzatore_risultati.Items.Add(item);
                        }
                    });


                    info_label.Invoke((MethodInvoker)delegate
                    {
                        info_label.Text = "Query executed.";
                        info_label.ForeColor = Color.Black;
                    });

                }
                catch (Exception exc)
                {
                    if (exc is MySqlException || exc is ArgumentNullException
                        || exc is InvalidOperationException || exc is NullReferenceException)
                    {
                        info_label.Invoke((MethodInvoker) delegate
                        {
                            info_label.Text = exc.Message;
                            info_label.ForeColor = Color.Red;
                        });

                    }
                }
            });
        }

        private void new_connection_Click(object sender, EventArgs e)
        {
            SQLQueryEditor conn = new SQLQueryEditor();
            conn.Visible = true;
        }

        private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.CloseConnection();
        }

        private void close_and_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
