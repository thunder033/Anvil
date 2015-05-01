using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CollisisionEditor2
{
    public static class Settings
	{
		//Define paths to resources
		public static string projectDirectory = "";
		//These now serve as default values in the case the settings file does not exist
		public static string textureDir = @"Binary\Content\Textures\";
		public static string texturesFile = "textures.txt";
		public static string animationDefsDir = @"Binary\AnimationsDefinitions\";
		public static string animationSheetDir = @"Assets\SpriteSheets\";
		public static string animationsIndex = "animations.txt";
		public static string animationExt = "txt";

		public static string customFieldsCSV = "Sound Keys,Sound Frames";
        public static string[] customFields = new string[2] {"Sound Keys","Sound Frames"};

		public static string settingsFile = "settings.settings";

		public static bool Load()
		{
			if (File.Exists(settingsFile))
			{
				StreamReader inStream = null;
				try
				{
					inStream = new StreamReader(settingsFile);

					projectDirectory = inStream.ReadLine() + "";
                    textureDir = inStream.ReadLine() + "";
                    texturesFile = inStream.ReadLine() + "";
                    animationDefsDir = inStream.ReadLine() + "";
                    animationSheetDir = inStream.ReadLine() + "";
                    animationsIndex = inStream.ReadLine() + "";
                    animationExt = inStream.ReadLine() + "";
					customFieldsCSV = inStream.ReadLine() + "";

					customFields = customFieldsCSV.Split(',');

					return true;

				}
				catch (Exception ex)
				{
					MessageBox.Show("Error Loading Settings: " + ex.Message);
					return false;
				}
				finally
				{
					if (inStream != null)
					{
						inStream.Close();
					}
				}
			}
			else
			{
				StreamWriter outStream = null;
				try
				{
					outStream = new StreamWriter(settingsFile);

					outStream.WriteLine(projectDirectory);
					outStream.WriteLine(textureDir);
					outStream.WriteLine(texturesFile);
					outStream.WriteLine(animationDefsDir);
					outStream.WriteLine(animationSheetDir);
					outStream.WriteLine(animationsIndex);
					outStream.WriteLine(animationExt);
					outStream.WriteLine(customFieldsCSV);

					customFields = customFieldsCSV.Split(',');
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error Creating Settings File: " + ex.Message);
				}
				finally
				{
					if (outStream != null)
					{
						outStream.Close();
					}
				}

				MessageBox.Show("Settings file missing! A default settings file created.");
				return Settings.Load();
			}
		}

		public static bool CheckConfiguration()
		{
			List<string> errors = new List<string>();
			if (!Directory.Exists(projectDirectory))
				errors.Add("Project directory not found: " + projectDirectory);
			else
			{
				if (!Directory.Exists(Path.Combine(projectDirectory, textureDir)))
					errors.Add("Textures directory not found: " + Path.Combine(projectDirectory, textureDir));
				else if (!File.Exists(Path.Combine(projectDirectory, textureDir, texturesFile)))
					errors.Add("Textures index file not found: " + Path.Combine(projectDirectory, textureDir, texturesFile));

				if (!Directory.Exists(Path.Combine(projectDirectory, animationDefsDir)))
					errors.Add("Animation definitions directory not found: " + Path.Combine(projectDirectory, animationDefsDir));
				else if (!File.Exists(Path.Combine(projectDirectory, animationDefsDir, animationsIndex)))
					errors.Add("Animation definitions index file not found: " + Path.Combine(projectDirectory, animationDefsDir, animationsIndex));
				else if (!Directory.Exists(Path.Combine(projectDirectory, animationSheetDir)))
					errors.Add("Animation preview directory not found: " + Path.Combine(projectDirectory, animationSheetDir));

				if (String.IsNullOrEmpty(animationExt))
					errors.Add("File type for animation definitions is not given.");

			}

			if (errors.Count != 0)
			{
				string errorString = "Configuration Error(s): \n";
				errorString += String.Join("\n", errors.ToArray<string>());
				MessageBox.Show(errorString);

				SettingsWindow settingsWin = new SettingsWindow();
				settingsWin.ShowDialog();

				if (settingsWin.settingsSaved)
				{
                    if (Load())
                    {
                        return Settings.CheckConfiguration();
                    }
                    else
                    {
                        return false;
                    }
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}
	}
}
