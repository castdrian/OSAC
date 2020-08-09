using AutoUpdaterDotNET;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Aax.Activation.ApiClient;

namespace OSAC
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer scrolltimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer progresstimer = new System.Windows.Threading.DispatcherTimer();
        public string abytes;
        public string checksum;
        public string resdir = AppDomain.CurrentDomain.BaseDirectory + "\\res";

        public MainWindow()
        {
            InitializeComponent();
            AutoUpdater.Start("https://raw.githubusercontent.com/adrifcastr/OSAC/master/OSAC/autoupdate.xml");
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.ReportErrors = false;
            AutoUpdater.RunUpdateAsAdmin = false;
        }

        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }

        public class TextBoxWriter : TextWriter
        {
            private TextBox _output = null;

            public TextBoxWriter(TextBox output)
            {
                _output = output;
            }

            public override void Write(char value)
            {
                base.Write(value);
                _output.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _output.AppendText(value.ToString());
                }));
            }

            public override Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("OSAC" + "\nVersion: 3.0" + "\nDeveloped by: adrifcastr" +
                "\n" + "\nThanks to:" + "\nEvan#8119" + "\nSylveon#8666" + "\njbodan");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.me/adrifcastr");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (var ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Filter = "Audible Audio Files|*.aax";
                ofd.Title = "Select an Audible Audio File";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    inputdisplay.Text = ofd.FileName;
                    outputdisplay.Text = Path.GetDirectoryName(ofd.FileName);
                    convertbutton.IsEnabled = true;
                }
            }
            statuslbl.Content = "";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    outputdisplay.Text = fbd.SelectedPath;
                    convertbutton.IsEnabled = true;
                }
            }
        }

        public void startbar()
        {
            pbar.IsIndeterminate = true;
        }

        public void stopbar()
        {
            pbar.IsIndeterminate = false;
        }

        public async void gethash()
        {
            startbar();

            statuslbl.Content = "Getting File Hash...";

            inpbutton.IsEnabled = false;
            outpbutton.IsEnabled = false;

            convertbutton.IsEnabled = false;

            dsrb();
            dsslst();


            checksum = ActivationByteHashExtractor.GetActivationChecksum(inputdisplay.Text);
            await crackbytes();
        }



        public async Task crackbytes()
        {
            statuslbl.Content = "Cracking Activation Bytes...";


            abytes = await AaxActivationClient.Instance.ResolveActivationBytes(checksum);

            carmdrm();
        }


        public async void carmdrm()
        {
            statuslbl.Content = "Converting File...";

            Extract("OSAC", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "ffmpeg.exe");

            string ffdir = AppDomain.CurrentDomain.BaseDirectory + "\\res\\ffmpeg.exe";
            string arg = @"-y -activation_bytes ";
            string arg1 = @" -i ";
            string arg2 = @" -ab ";
            string arg3 = @"k -map_metadata 0 -id3v2_version 3 -vn ";
            string fileout = Path.Combine(outputdisplay.Text, Path.GetFileNameWithoutExtension(inputdisplay.Text) + GetOutExtension());
            string arguments = arg + abytes + arg1 + inputdisplay.Text + arg2 + qlabel.Content + arg3 + fileout;

            Process ffm = new Process();
            ffm.StartInfo.FileName = ffdir;
            ffm.StartInfo.Arguments = arguments;
            ffm.StartInfo.CreateNoWindow = true;
            ffm.StartInfo.RedirectStandardError = true;
            ffm.StartInfo.UseShellExecute = false;
            ffm.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            ffm.EnableRaisingEvents = true;
            Console.SetOut(new TextBoxWriter(txtConsole));
            ffm.ErrorDataReceived += (s, ea) =>
            {
                Console.WriteLine($"{ea.Data}");
            };

            scrolltimer.Tick += new EventHandler(scrolltimer_Tick);
            scrolltimer.Interval = new TimeSpan(0, 0, 1);

            ffm.Start();

            ffm.BeginErrorReadLine();
            scrolltimer.Start();

            await Task.Run(() => ffm.WaitForExit());

            ffm.Close();

            inpbutton.IsEnabled = true;

            convertbutton.IsEnabled = true;

            scrolltimer.Stop();
            scrolltimer.Tick -= new EventHandler(scrolltimer_Tick);

            enrb();
            enbslst();

            statuslbl.Content = "Conversion Complete!";

            stopbar();

            Directory.Delete(resdir, true);
        }

        private string GetOutExtension()
        {
            if (rmp3.IsChecked.Value) return ".mp3";
            else if (raac.IsChecked.Value) return ".m4b";
            else if (rflac.IsChecked.Value) return ".flac";

            rmp3.IsChecked = true;
            return ".mp3";
        }

        public void convertbutton_Click(object sender, RoutedEventArgs e)
        {
            gethash();
        }

        private void scrolltimer_Tick(object sender, EventArgs e)
        {
            txtConsole.ScrollToEnd();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            enbslst();
            outpbutton.IsEnabled = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            enbslst();
            outpbutton.IsEnabled = true;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            dsslst();
            outpbutton.IsEnabled = true;
        }

        public void enbslst()
        {
            sqlbl.IsEnabled = true;
            curqlbl.IsEnabled = true;
            qlabel.IsEnabled = true;
            qslider.IsEnabled = true;
        }

        public void dsslst()
        {
            sqlbl.IsEnabled = false;
            curqlbl.IsEnabled = false;
            qlabel.IsEnabled = false;
            qslider.IsEnabled = false;
        }

        public void enrb()
        {
            rmp3.IsEnabled = true;
            raac.IsEnabled = true;
            rflac.IsEnabled = true;
        }

        public void dsrb()
        {
            rmp3.IsEnabled = false;
            raac.IsEnabled = false;
            rflac.IsEnabled = false;
        }
    }
}