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
    class Modem_Controller
    {
        static bool debug = false;
        public static string modem_port = "";
        public static bool init_modem()
        {
            bool success = false;
            for (int i = 0; i < 10; i++)
            {
                if (find_AT_SupportedPort() == true)
                {
                    success = true;
                    break;
                }
            }
            if (success)
            {
                if(debug)MessageBox.Show(modem_port);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static int signal_strength()
        {
            SerialPort serialPort = new SerialPort(modem_port);
            if (serialPort.IsOpen) return -1;
            serialPort.Open();

            serialPort.WriteLine("at+csq" + Environment.NewLine);
            Thread.Sleep(10);
            if (serialPort.BytesToRead != 0)
            {
                serialPort.ReadLine();
            }
            string msg = serialPort.ReadLine(); //+csq: 18,99
            serialPort.Close();
            msg = msg.Split(' ')[1];
            msg = msg.Split(',')[0];
            return int.Parse(msg);
        }
        public static string get_modem_info()
        {
            string text = "";
            SerialPort serialPort = new SerialPort(modem_port);
			serialPort.Open();

            serialPort.WriteLine("ati" + Environment.NewLine);
			Thread.Sleep(10);
			if (serialPort.BytesToRead != 0)
			{
				serialPort.ReadLine();
			}
			if (serialPort.BytesToRead != 0)
			{
				text = serialPort.ReadLine();
			}
            if (serialPort.BytesToRead != 0)
            {
                text += serialPort.ReadLine();
            }
            if (serialPort.BytesToRead != 0)
            {
                text += serialPort.ReadLine();
            }
            if (serialPort.BytesToRead != 0)
            {
                text += serialPort.ReadLine();
            }
            while (serialPort.BytesToRead != 0)
            {
                serialPort.ReadLine();
            }
            serialPort.WriteLine("at+zdon?" + Environment.NewLine);
            Thread.Sleep(10);
            if (serialPort.BytesToRead != 0)
            {
                serialPort.ReadLine();
            }
            if (serialPort.BytesToRead != 0)
            {
                text += serialPort.ReadLine();
            }
            while (serialPort.BytesToRead != 0)
            {
                serialPort.ReadLine();
            }
            serialPort.Close();
            return text;
        }
        private static bool find_AT_SupportedPort()
        {
            string[] portNames = SerialPort.GetPortNames();
            string[] array = portNames;

            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                SerialPort serialPort = new SerialPort(text);
                
                try
                {
                    serialPort.Open();
                    serialPort.WriteLine("at" + Environment.NewLine);
                    Thread.Sleep(10);
                    if (serialPort.BytesToRead != 0)
                    {
                        serialPort.ReadLine();
                        if (serialPort.BytesToRead != 0)
                        {
                            string text2 = serialPort.ReadLine();
                            if (text2.Contains("OK"))
                            {
                                
                                modem_port = text;
                                serialPort.WriteLine("at+zoprt=5" + Environment.NewLine);//set online mode
                                Thread.Sleep(10);
                                while (serialPort.BytesToRead != 0)
                                {
                                    serialPort.ReadLine();
                                }
                                serialPort.Close();
                                return true;
                            }
                        }
                        serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    serialPort.Close();
                    if(debug)MessageBox.Show(ex.ToString());
                }
            }
            return false;
        }
        public static string send_ussd(string com)
        {
            SerialPort serialPort = new SerialPort(modem_port);
            serialPort.Open();
            serialPort.WriteLine("at+cusd=1,\"" + com + "\",15" + Environment.NewLine);
            Thread.Sleep(10);
            Thread.Sleep(10);
            while (serialPort.BytesToRead != 0)
            {
                serialPort.ReadLine();
            }
            for (int i = 0; i < 28; i++)
            {
                Thread.Sleep(250);
                if (serialPort.BytesToRead != 0)
                {
                    break;
                }
            }
            if (serialPort.BytesToRead != 0)
            {
                serialPort.ReadLine();
            }
            if (serialPort.BytesToRead != 0)
            {
                string encoded_message = serialPort.ReadLine();
                serialPort.Close();
                
                return encoded_message;
            }
            else
            {
                serialPort.Close();
                return "";
            }
        }
        public static string decode_ussd_message(string message)
        {
            string[] array = message.Split(new char[]{'"'});
            char[] array2 = array[1].ToCharArray();
            string text = "";
            int num = array2.Length;
            for (int i = 0; i < num; i++)
            {
                if (array2[i] > '9')
                {
                    char[] expr_3F_cp_0 = array2;
                    int expr_3F_cp_1 = i;
                    expr_3F_cp_0[expr_3F_cp_1] -= '7';
                }
                else
                {
                    char[] expr_58_cp_0 = array2;
                    int expr_58_cp_1 = i;
                    expr_58_cp_0[expr_58_cp_1] -= '0';
                }
            }
            for (int j = 0; j < num; j += 4)
            {
                int num2 = (int)(array2[j] * '\u0010' + array2[j + 1]);
                num2 *= 256;
                int num3 = (int)(array2[j + 2] * '\u0010' + array2[j + 3]);
                text += (char)(num2 + num3);
            }
            return text;
        }
    }
}
