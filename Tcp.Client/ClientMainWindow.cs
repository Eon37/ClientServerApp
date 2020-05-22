using System;
using System.Windows.Forms;
using SomeProject.Library.Client;
using SomeProject.Library;

namespace SomeProject.TcpClient
{
    public partial class ClientMainWindow : Form
    {
        /// <summary>
        /// Inicializes client window
        /// </summary>
        public ClientMainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sends message to server by Clicking "Send message" button
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnMsgBtnClick(object sender, EventArgs e)
        {
            Client client = new Client();
            OperationResult res;
            res = client.SendMessageToServer(textBox.Text);

            textBox.Text = "";
            labelRes.Text = res.Message;

            timer.Interval = 2000;
            timer.Start();
        }

        /// <summary>
        /// Clears the label after the time had expired
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            labelRes.Text = "";
            timer.Stop();
        }

        /// <summary>
        /// Sends file to server by Clicking "Send File" button
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void sendFileBtn_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            OperationResult res;
            string path = openFile();

            if (path != null) res = client.SendFileToServer(path);
            else return;

            labelRes.Text = res.Message;

            timer.Interval = 2000;
            timer.Start();
        }

        /// <summary>
        /// Lets user choose file path from OpenFileDialog
        /// </summary>
        /// <returns>File path</returns>
        private string openFile()
        {
            using (OpenFileDialog opFile = new OpenFileDialog())
            {
                if (opFile.ShowDialog(this) == DialogResult.OK)
                {
                    return opFile.FileName;
                }
                else
                {
                    labelRes.Text = "Could not open the file.";

                    timer.Interval = 2000;
                    timer.Start();
                    return null;
                }
            }
        }
    }
}
