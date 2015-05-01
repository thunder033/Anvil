using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CollisisionEditor2
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
		public bool settingsSaved { get; private set; }

        string _projectDirectory;
        public string sProjectDirectory {
            get {
                return _projectDirectory;
            }
            set
            {
                _projectDirectory = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("sProjectDirectory"));
                }
            }
        }

        string _textureDir;
		public string sTextureDir {
            get {
                return _textureDir;
            }
            set
            {
                _textureDir = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("sTextureDir"));
                }
            }
        }

        string _texturesFile;
		public string sTexturesFile {
            get {
                return _texturesFile;
            }
            set
            {
                _texturesFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("sProjectDirectory"));
                }
            }
        }
		public string sAnimationDefsDir;
		public string sAnimationSheetDir;
		public string sAnimationsIndex;
		public string sAnimationExt;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsWindow()
        {
            InitializeComponent();

			settingsSaved = false;

			if (CollisisionEditor2.Settings.Load())
			{
				projectDirectory.Text = CollisisionEditor2.Settings.projectDirectory;
				texturesDirectory.Text = CollisisionEditor2.Settings.textureDir;
				texturesIndex.Text = CollisisionEditor2.Settings.texturesFile;
				animationDefsDir.Text = CollisisionEditor2.Settings.animationDefsDir;
				animationSheetDir.Text = CollisisionEditor2.Settings.animationSheetDir;
				animationIndex.Text = CollisisionEditor2.Settings.animationsIndex;
				animationFileExt.Text = CollisisionEditor2.Settings.animationExt;
				customFieldsCSV.Text = CollisisionEditor2.Settings.customFieldsCSV;
			}
			else
			{
				this.Close();
			}
        }

		private void SaveSettings(object sender, RoutedEventArgs e)
		{
            StreamWriter outStream = null;
            try
            {
                outStream = new StreamWriter(CollisisionEditor2.Settings.settingsFile);

                outStream.WriteLine(projectDirectory.Text);
                outStream.WriteLine(texturesDirectory.Text);
                outStream.WriteLine(texturesIndex.Text);
                outStream.WriteLine(animationDefsDir.Text);
                outStream.WriteLine(animationSheetDir.Text);
                outStream.WriteLine(animationIndex.Text);
                outStream.WriteLine(animationFileExt.Text);
				outStream.WriteLine(customFieldsCSV.Text);

				CollisisionEditor2.Settings.customFields = CollisisionEditor2.Settings.customFieldsCSV.Split(',');
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                System.Windows.MessageBox.Show("Error saving settings file on line " + frame.GetFileLineNumber() + " : " + ex.Message);
            }
            finally
            {
                if (outStream != null)
                {
                    outStream.Close();
                }
            }

            settingsSaved = true;
            this.Close();
		}

		private void browseProjectDirectory(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Browse for the directory that the C# MonoGame project is located in";
			dialog.ShowDialog();

			projectDirectory.Text = dialog.SelectedPath;
		}
    }
}
