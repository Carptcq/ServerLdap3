using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading;

namespace ServerLdap3
{
    public partial class ServerLdap : Form
    {
        public ServerLdap()
        {
            InitializeComponent();
        }

        private void ServerLdap_Load(object sender, EventArgs e)
        {
            //button1_Click(sender, e);
            Ejercicio();
            GetDefaultGateway();
            ShowConnectedID();
            GetMACAddress();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Welcome");

            DialogResult dr = MessageBox.Show("Do you want to close this window?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(dr == DialogResult.Yes)
            {
                this.Close();
            }

        }
        private void Ejercicio()
        {
            //IP Address
            string _hostName = Dns.GetHostName();
            IPAddress[] _ipList = Dns.GetHostAddresses(_hostName)/*.AddressList*/;

            foreach (IPAddress  ip in _ipList)
            {
                label1.Text = string.Format("IP Host: {0}\n", ip);
            }
            
            //Gateway IP
            label2.Text = string.Format("IP Gateway: {0}\n", GetDefaultGateway());
            
            //Username
            string username = System.Windows.Forms.SystemInformation.UserName;
            label4.Text = "Username: " + username;
            
            //HostName
            string hostname = System.Windows.Forms.SystemInformation.ComputerName;
            label5.Text = "Hostname: " + hostname;

            
                //Console.WriteLine("Pingping" + address);
                Ping pingClass = new Ping();
                PingReply pingReply = pingClass.Send("1.1.1.1", 1000);
                int contadorPing = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (pingReply.Status == IPStatus.Success)
                    {
                        contadorPing++;

                        if (contadorPing <= 0)
                        {
                            label9.Text = "Cloudflare Connection: Disconnected";
                        }
                        else if(contadorPing > 0 && contadorPing <= 3)
                        {
                            label9.Text = "Cloudflare Connection: Unstable";
                        }
                        else if(contadorPing >= 4)
                        {
                            label9.Text = "Cloudflare Connection: Connected";
                        }
                    }

                }
            
        }

        public static IPAddress GetDefaultGateway()
        {
            var gateway_address = NetworkInterface.GetAllNetworkInterfaces().Where(e => e.OperationalStatus == OperationalStatus.Up)
                .SelectMany(e => e.GetIPProperties().GatewayAddresses).FirstOrDefault();
            if(gateway_address == null)
            {
                return null;
            }
            return gateway_address.Address;
        }

        private void ShowConnectedID()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string s = p.StandardOutput.ReadToEnd();
            string s1 = s.Substring(s.IndexOf("SSID"));
            s1 = s1.Substring(s1.IndexOf(":"));
            s1 = s1.Substring(2, s1.IndexOf("\n")).Trim();

            string estado = "";
            bool RedActiva = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (RedActiva)
            {
                estado = "Conectado";
            }
            else
            {
                estado = "No Conectado";
            }
            //SSID Name
            label6.Text = "SSID: " + s1;
            //SSID Status
            label7.Text = "SSID Status: " + estado;
            p.WaitForExit();
        }

        public void GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            //MAC
            label8.Text = "MAC Address: " + sMacAddress;
        }

        public static long PingAddress(string address, int timeout)
        {
            try
            {
                //Console.WriteLine("Pingping" + address);
                Ping pingClass = new Ping();
                PingReply pingReply = pingClass.Send(address, timeout);
                int contadorPing = 0;
                for (int i = 0; i < 4; i++)
                {
                    if(pingReply.Status == IPStatus.Success)
                    {
                        contadorPing++;

                        if(contadorPing <= 0)
                        {
                            
                        }
                    }
                    
                }
                return pingReply.RoundtripTime;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                throw;
            }
            return -1;
        }
    }
}
