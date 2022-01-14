using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace GameServerKillers
{
    public partial class FormLogin : Form
    {
        public string PlayerName = "";
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            PlayerName = tbName.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbName_KeyUp(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbName.Text)
                && ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return)))
            {
                PlayerName = tbName.Text;
                Close();

            }
        }
    }
}
