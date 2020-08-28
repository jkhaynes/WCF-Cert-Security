using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleWinFormsClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        

            var client = new RestClient("https://localhost:44374");
            var request = new RestRequest("/api/cert", Method.GET);
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new Exception(response.ErrorMessage);
            }

            var rawData = JsonConvert.DeserializeObject<byte[]>(response.Content);

            var cert = new X509Certificate2(rawData);

            ServiceReference1.Service1Client proxy = new ServiceReference1.Service1Client();
            proxy.ClientCredentials.ClientCertificate.Certificate = cert;
            this.label1.Text = proxy.GetHelloWorld();
        }
    }
}
