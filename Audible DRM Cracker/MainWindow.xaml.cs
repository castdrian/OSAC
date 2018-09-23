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
using System.Timers;
using System.ComponentModel;
using System.Windows.Controls;

namespace Audible_DRM_Cracker
{
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    System.Windows.Threading.DispatcherTimer scrolltimer = new System.Windows.Threading.DispatcherTimer();
    System.Windows.Threading.DispatcherTimer progresstimer = new System.Windows.Threading.DispatcherTimer();
    public string abytes;
    public string checksum;
    public string resdir = AppDomain.CurrentDomain.BaseDirectory + "\\res";
    public string ftype;

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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("Audible DRM Cracker" + "\nVersion: 2.0" + "\nDeveloped by: adrifcastr" +
          "\n" + "\nThanks to:" + "\nEvan#8119" + "\nSylveon#8666");
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
    [Obsolete("method is no longer needed", true)]
    public void checkspaces()
    {
      Regex Check1 = new Regex(@"\s");
      string inputfile = inputdisplay.Text;

      Regex Check2 = new Regex(@"\s");
      string outputfile = outputdisplay.Text;

      if (Check1.IsMatch(inputfile) || Check2.IsMatch(outputfile))
      {
        MessageBox.Show("One or more filenames contain whitespaces." + "\nThe Conversion will probably fail.");
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

      Directory.CreateDirectory(resdir);
      Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "ffprobe.exe");

      string ffdir = AppDomain.CurrentDomain.BaseDirectory + "\\res\\ffprobe.exe";
      string arg = string.Format("\"{0}\"", inputdisplay.Text);

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
      checksum = regex.Match(txtConsole.Text).Value;

      crackbytes();
    }

    public async void crackbytes()
    {
      statuslbl.Content = "Cracking Activation Bytes...";

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


      Process rcr = new Process();
      rcr.StartInfo.FileName = rcdir;
      rcr.StartInfo.Arguments = @". -h " + checksum;
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
        abytes = match.Groups[1].Value;
      } 
      carmdrm();
    }

    public async void carmdrm()
    {
      statuslbl.Content = "Converting File...";

      Extract("Audible_DRM_Cracker", AppDomain.CurrentDomain.BaseDirectory + "\\res", "res", "ffmpeg.exe");

      string ffdir = AppDomain.CurrentDomain.BaseDirectory + "\\res\\ffmpeg.exe";
      string arg = @"-y -activation_bytes ";
      string arg1 = @" -i ";
      string arg2 = @" -ab ";
      string arg3 = @"k -vn ";
      string fileout = Path.Combine(outputdisplay.Text, Path.GetFileNameWithoutExtension(inputdisplay.Text) + GetOutExtension());
      string arguments = string.Format("{0}{1}{2}\"{3}\"{4}{5}{6}\"{7}\"", arg, abytes, arg1, inputdisplay.Text, arg2, qlabel.Content, arg3, fileout);

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

      statuslbl.Content = "Conversion Complete!";

      stopbar();

      Directory.Delete(resdir, true);
    }

    private string GetOutExtension()
    {
      if (rmp3.IsChecked.Value) return ".mp3";
      else if (raac.IsChecked.Value) return ".raac";
      else if (rflac.IsChecked.Value) return ".rflac";
      else //default
      {
        rmp3.IsChecked = true; 
        return ".mp3";
      }
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
      ftype = "LAME MP3 Audio File|*.mp3";
    }

    private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
    {
      enbslst();
      outpbutton.IsEnabled = true;
      ftype = "AAC M4A Audio File|*.m4a";
    }

    private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
    {
      dsslst();
      outpbutton.IsEnabled = true;
      ftype = "FLAC Audio File|*.flac";
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