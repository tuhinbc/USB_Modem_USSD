using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace USSD
{
    public partial class Form1 : Form
    {
        string cmds = "";
        bool processing_ongoing = false;
        BackgroundWorker bw;
        public Form1()
        {
            InitializeComponent();
            if (Modem_Controller.init_modem() == false)
            {
                MessageBox.Show("Failed to find any modem . Please try again.");
                Application.Exit();
            }
            //MessageBox.Show(Modem_Controller.get_modem_info());
            richTextBox1.Text = "Device Info :\n\n"+Modem_Controller.get_modem_info();
            load_ussd_commands();

            //Modem_Controller.signal_strength();
            richTextBox2.Text = " Abdullah Al Mamun\n tuhinbc@gmail.com\n 01812133471";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (processing_ongoing)
            {
                button1.Text = "Send";
                processing_ongoing = false;
                return;
            }
            if (textBox1.TextLength == 0)
            {
                richTextBox1.Text = "Please Enter a command.";
                return;
            }
            else richTextBox1.Clear();
            processing_ongoing = true;
            button1.Text = "Cancel";
            
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync(textBox1.Text);
            
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (processing_ongoing == false) return;
            string msg="";
            msg = (string)e.Result;
            msg = Modem_Controller.decode_ussd_message(msg);

            richTextBox1.Text = msg;
            store_new_command();
            textBox1.Clear();

            load_ussd_commands();
            processing_ongoing = false;
            button1.Text = "Send";
        }
        void store_new_command()
        {
            Properties.Settings settings = Properties.Settings.Default;
            string tmp = settings.UCommands;
            if (tmp.Length > 120)
            {
                tmp = tmp.Split(new char[] { '♣' }, 4)[3];
            }
            settings.UCommands = tmp + "♣" + textBox1.Text;
            Properties.Settings.Default.Save();
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Modem_Controller.send_ussd((string)e.Argument);
        }
        private void load_ussd_commands()
        {
            
            string cmds = Properties.Settings.Default.UCommands;
            //MessageBox.Show(cmds);
            string[] array = cmds.Split(new char[]
			{
				'♣'
			});
            this.listBox1.Items.Clear();
            string[] array2 = array;
   
            for (int i = array2.Length-1 ; i >=0; i--)
            {
                string item = array2[i];
                this.listBox1.Items.Add(item);
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') button1.PerformClick();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex<0) return;
            textBox1.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (processing_ongoing) return;
            int val=0;
            try
            {
                val = Modem_Controller.signal_strength();
            }
            catch (Exception c)
            {
                return;
            }
            if (val == -1) return;
            if (val > 31) val = 31;
            if (val < 0) val = 0;
            progressBar1.Value = val;
            label3.Text = (-113 + (val * 2)).ToString() + "dBm";
        }


    }
}
