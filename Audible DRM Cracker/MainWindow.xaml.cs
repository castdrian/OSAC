using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.ComponentModel;

namespace Audible_DRM_Cracker
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

        }

        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        System.Windows.Threading.DispatcherTimer scrolltimer = new System.Windows.Threading.DispatcherTimer();

        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Audible DRM Cracker" + "\nVersion: 1.0" + "\nDeveloped by: adrifcastr" +
                "\n" + "\nThanks to:" + "\nEvan#8119" + "\nSylveon#8666");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/adrifcastr");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            // Displays an OpenFileDialog so the user can select a File.  
            
            openFileDialog1.Filter = "Audible Audio Files|*.aax";
            openFileDialog1.Title = "Select an Audible Audio File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .AAX file was selected, open it.  
            if (openFileDialog1.ShowDialog() == true)
               inputdisplay.Text = openFileDialog1.FileName;

            hashbutton.IsEnabled = true;

        }

        private void inputdisplay_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            
            saveFileDialog1.Filter = "LAME MP3 File|*.mp3";
            saveFileDialog1.Title = "Choose Output File";

            if (saveFileDialog1.ShowDialog() == true)
                outputdisplay.Text = saveFileDialog1.FileName;

        }

        public async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string resdir = AppDomain.CurrentDomain.BaseDirectory + "\\res";
            Directory.CreateDirectory(resdir);
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "ffprobe.exe");

            string ffdir = AppDomain.CurrentDomain.BaseDirectory + "\\res\\ffprobe.exe";
            string arg = inputdisplay.Text;

            Process ffp = new Process();
            ffp.StartInfo.FileName = ffdir;
            ffp.StartInfo.Arguments = arg;
            ffp.StartInfo.CreateNoWindow = true;
            ffp.StartInfo.RedirectStandardOutput = true;
            ffp.StartInfo.RedirectStandardError = true;
            ffp.StartInfo.UseShellExecute = false;
            ffp.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            ffp.Start();

            string r = ffp.StandardError.ReadToEnd();
            txtConsole.AppendText(r);

            await Task.Run(() => ffp.WaitForExit());
            txtConsole.ScrollToEnd();

            ffp.Close();

            var regex = new Regex(@"[A-z0-9]{40}");
            string checksum = regex.Match(txtConsole.Text).Value;

            hashbox.AppendText(checksum);

            bytebutton.IsEnabled = true;
        }

        public async void bytebutton_Click(object sender, RoutedEventArgs e)
        {
            hashbutton.IsEnabled = false;
            MessageBox.Show("This may take a while!");

            string resdir = AppDomain.CurrentDomain.BaseDirectory + "\\res";
            Directory.CreateDirectory(resdir);

            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "rcrack.exe");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "alglib1.dll");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_0_10000x789935_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_1_10000x791425_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_2_10000x790991_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_3_10000x792120_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_4_10000x790743_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_5_10000x790568_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_6_10000x791458_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_7_10000x791707_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_8_10000x790202_0.rtc");
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "audible_byte#4-4_9_10000x791022_0.rtc");

            string rcdir = AppDomain.CurrentDomain.BaseDirectory + "\\res\\rcrack.exe";

            string arghash = hashbox.Text;

            Process rcr = new Process();
            rcr.StartInfo.FileName = rcdir;
            rcr.StartInfo.Arguments = @". -h " + arghash;
            rcr.StartInfo.CreateNoWindow = true;
            rcr.StartInfo.RedirectStandardOutput = true;
            rcr.StartInfo.UseShellExecute = false;
            rcr.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            rcr.Start();

            string o = rcr.StandardOutput.ReadToEnd();
            txtConsole.AppendText(o);

            await Task.Run(() => rcr.WaitForExit());
            txtConsole.ScrollToEnd();
            rcr.Close();

            var regex = new Regex(@"hex:([A-z0-9]+)");

            Match match = regex.Match(txtConsole.Text);
            if (match.Success)
            {
                string abytes = match.Groups[1].Value;
                bytebox.Text = abytes;
            }
            
            convertbutton.IsEnabled = true;
        }
        
        public class TextBoxWriter : TextWriter
        {
            TextBox _output = null;

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

        public async void convertbutton_Click(object sender, RoutedEventArgs e)
        {

            string resdir = AppDomain.CurrentDomain.BaseDirectory + "\\res";
            Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "ffmpeg.exe");

            string ffdir = AppDomain.CurrentDomain.BaseDirectory + "\\res\\ffmpeg.exe";
            string arg = @"-y -activation_bytes ";
            string arg1 = @" -i ";
            string arg2 = @" -ab 80k -vn ";
            string abytes = bytebox.Text;
            string arguments = arg + abytes + arg1 + openFileDialog1.FileName + arg2 + saveFileDialog1.FileName;
           
            Process ffm = new Process();
            ffm.StartInfo.FileName = ffdir;
            ffm.StartInfo.Arguments = arguments;
            ffm.StartInfo.CreateNoWindow = true;
            ffm.StartInfo.RedirectStandardError = true;
            ffm.StartInfo.UseShellExecute = false;
            ffm.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            ffm.EnableRaisingEvents = true;
            Console.SetOut(new TextBoxWriter(txtConsole));
            ffm.ErrorDataReceived += (s, ea) => { Console.WriteLine($"{ea.Data}"); };

            scrolltimer.Tick += new EventHandler(scrolltimer_Tick);
            scrolltimer.Interval = new TimeSpan(0,0,1);

            ffm.Start();
            ffm.BeginErrorReadLine();
            scrolltimer.Start();

            await Task.Run(() => ffm.WaitForExit());

            ffm.Close();

            MessageBox.Show("Conversion Complete!");

            Directory.Delete(resdir, true);
        }

        private void scrolltimer_Tick(object sender, EventArgs e)
        {
            // code goes here 
            txtConsole.ScrollToEnd();
        }
    }
}
