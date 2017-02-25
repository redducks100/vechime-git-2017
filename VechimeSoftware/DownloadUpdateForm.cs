using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace VechimeSoftware
{
    public partial class DownloadUpdateForm : Form
    {
        private LoginForm parent;
        private VersionData newVersion;
        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;

        public DownloadUpdateForm(LoginForm _parent, VersionData _newVersion)
        {
            InitializeComponent();
            newVersion = _newVersion;

            parent = _parent;

            tempFile = Path.Combine(Path.GetTempPath(), "Setup.msi");

            versionLabel.Text = newVersion.Version;
            infoTextBox.Text = newVersion.Info;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;

            try
            {
                webClient.DownloadFileAsync(new Uri(newVersion.DownloadURL), tempFile);
            }
            catch
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if(e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else
            {
                progressLabel.Text = "Verifying download...";
                downloadBar.Style = ProgressBarStyle.Marquee;

                bgWorker.RunWorkerAsync(new string[] { tempFile, newVersion.MD5 });
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadBar.Value = Convert.ToInt32((double)(e.BytesReceived)/(double)newVersion.UpdateSize * 100.0);
            progressLabel.Text = String.Format("{0} / {1}",FormatBytes(e.BytesReceived,1),FormatBytes(newVersion.UpdateSize, 1));
        }

        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType = true)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            if(newBytes > 1024 && newBytes <= 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if(newBytes > 1048576 && newBytes <= 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }

            if(decimalPlaces > 0)
            {
                formatString += ":0.";
            }

            for(int i=0;i<decimalPlaces;i++)
            {
                formatString += "0";
            }

            formatString += "}";

            if(showByteType)
            {
                formatString += byteType;
            }

            return string.Format(formatString, newBytes);
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            string updateMD5 = ((string[])e.Argument)[1];
            if (Utils.HashFile(file,HashType.MD5) != updateMD5) //check hash
            {
                e.Result = DialogResult.No;
            }
            else
            {
                if (MessageBox.Show("Doriti sa instalati versiunea noua?", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    e.Result = DialogResult.OK;
                    parent.updateFilePath = tempFile;
                }
                else
                {
                    e.Result = DialogResult.No;
                }
            }
        }

        private void DownloadUpdateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

            if(bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }
    }
}
