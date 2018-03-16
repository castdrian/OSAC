using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            string arg = inputdisplay.Text;

            Process ffp = new Process();
            ffp.StartInfo.FileName = "ffprobe.exe";
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
            string arghash = inputdisplay.Text;

            Process rcr = new Process();
            rcr.StartInfo.FileName = "rcrack.exe";
            rcr.StartInfo.Arguments = arghash;
            rcr.StartInfo.CreateNoWindow = true;
            rcr.StartInfo.RedirectStandardOutput = true;
            rcr.StartInfo.RedirectStandardError = true;
            rcr.StartInfo.UseShellExecute = false;
            rcr.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            rcr.Start();

            string r = rcr.StandardError.ReadToEnd();
            txtConsole.Text = r;

            rcr.WaitForExit();
            rcr.Close();
        }
    }
}
