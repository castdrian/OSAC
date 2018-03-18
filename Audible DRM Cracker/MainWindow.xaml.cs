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
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
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
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "LAME MP3 File|*.mp3";
            saveFileDialog1.Title = "Choose Output File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.ShowDialog() == true)
                outputdisplay.Text = saveFileDialog1.FileName;

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
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
            txtConsole.Text = r;

            ffp.WaitForExit();
            ffp.Close();

            var regex = new Regex(@"[A-z0-9]{40}");
            string checksum = regex.Match(txtConsole.Text).Value;

            hashbox.Text = checksum;

            bytebutton.IsEnabled = true;
        }

        private void bytebutton_Click(object sender, RoutedEventArgs e)
        {
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

            var regex = new Regex(@"[A-z0-9]{40}");
            string checksum = regex.Match(txtConsole.Text).Value;

            string arghash = checksum;

            Process rcr = new Process();
            rcr.StartInfo.FileName = rcdir;
            rcr.StartInfo.Arguments = @"-h" + arghash;
            rcr.StartInfo.CreateNoWindow = true;
            rcr.StartInfo.RedirectStandardOutput = true;
            rcr.StartInfo.RedirectStandardError = true;
            rcr.StartInfo.UseShellExecute = false;
            rcr.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            rcr.Start();

            string o = rcr.StandardOutput.ReadToEnd();
            txtConsole.Text = o;

            rcr.WaitForExit();
            rcr.Close();

            Directory.Delete(resdir, true);

        }
    }
}
