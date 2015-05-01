using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using XNAContentCompiler;
using System.Threading;
using System.Text.RegularExpressions;

namespace CollisisionEditor2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AnimPropertiesWindow : Window
    {
        BitmapImage sourceTexture;
        String sourcePath;
        bool textureIdentified;
		int textureSheetCount;
		int _textureSheetIndex;
		public int textureSheetIndex
		{
			get
			{
				return _textureSheetIndex;
			}
			set
			{
				if(goToTextureSheet(value))
					_textureSheetIndex = value;
			}
		}

		public ObservableDictionary<string, string> _AnimationDefinition;

        Thread tProcessBuildOutput;
        string[] outputFiles;
		ComboItem[] compiledTextures;

		public enum ActionType
		{
			Edit,
			Create
		}

		ActionType action;

        public AnimPropertiesWindow()
        {
            InitializeComponent();

			_AnimationDefinition = new ObservableDictionary<string, string>();
			_AnimationDefinition.Add("Name", "");
			_AnimationDefinition.Add("Texture", "");
			_AnimationDefinition.Add("Frames", "");
			_AnimationDefinition.Add("Frame Dur.", "");
			_AnimationDefinition.Add("Frame Width", "");
			_AnimationDefinition.Add("Frame Height", "");
			_AnimationDefinition.Add("Offset X", "");
			_AnimationDefinition.Add("Offset Y", "");
			_AnimationDefinition.Add("Obj Width", "");
			_AnimationDefinition.Add("Obj Height", "");

            //additional fields not part of the definition but included to simply architecture
            _AnimationDefinition.Add("Texture Path", "");

			DataContext = _AnimationDefinition;

            sourcePath = "";
            sourceTexture = null;
            tProcessBuildOutput = null;
            outputFiles = null;
            textureIdentified = false;
			textureSheetIndex = 0;

			Texture.SizeChanged += Texture_SizeChanged;
			action = ActionType.Create;

			Closed += AnimPropertiesWindow_Closed;
        }

		void AnimPropertiesWindow_Closed(object sender, EventArgs e)
		{
			saveTexture();
		}

        public AnimPropertiesWindow(Dictionary<string,string> aAnimationDefintion) : this()
        {
			action = ActionType.Edit;
            foreach (KeyValuePair<string, string> kvp in aAnimationDefintion)
            {
				_AnimationDefinition[kvp.Key] = kvp.Value;
                switch (kvp.Key)
                {
                    case "Texture":
                        textureName.Text = kvp.Value;
                        break;
                    case "Name":
                        animationName.Text = kvp.Value;
                        break;
                    case "Frames":
                        totalFrames.Text = kvp.Value;
                        break;
                    case "Frame Dur.":
                        frameDuration.Text = kvp.Value;
                        break;
                    case "Frame Width":
                        frameWidth.Text = kvp.Value;
                        break;
                    case "Frame Height":
                        frameHeight.Text = kvp.Value;
                        break;
                    case "Offset X":
                        animOffsetX.Text = kvp.Value;
                        break;
                    case "Offset Y":
                        animOffsetY.Text = kvp.Value;
                        break;
                    case "Obj Width":
                        worldWidth.Text = kvp.Value;
                        break;
                    case "Obj Height":
                        worldHeight.Text = kvp.Value;
                        break;
                }
            }

            Loaded += Window1_Loaded;

			SaveButton.Visibility = Visibility.Hidden;
        }

        void Texture_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkTextureName(textureName, new EventArgs());
            updateSlicePreview(new Object(), new EventArgs());
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            checkTextureName(textureName.Text, true);
            updateSlicePreview(new Object(), new EventArgs());
        }

		private void BrowseNewAnimation(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
			fileDialog.FileOk += fileDialog_FileOk;
			fileDialog.ShowDialog();
		}

		void fileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				OpenFileDialog fileDialog = sender as OpenFileDialog;
                sourcePath = fileDialog.FileName;

				sourceTexture = new BitmapImage();
                sourceTexture.BeginInit();
                sourceTexture.UriSource = new Uri(sourcePath);
                sourceTexture.EndInit();

                Texture.Source = sourceTexture;
                Texture.Visibility = Visibility.Visible;

                TextureSelect.Visibility = Visibility.Collapsed;

			}
			catch (Exception ex)
			{
				MessageBox.Show("Error opening file: " + ex.Message);
			}
		}

		private void checkTextureName(object sender, EventArgs e)
		{
			TextBox input = sender as TextBox;

			checkTextureName(input.Text);
		}

		private void checkTextureName(string textureKey, bool overrideTextureCheck = false)
		{
			TextureInstr.Text = "Select existing texture, or Double Click to Browse or Drag PNG Here to create a new one";

			if (MainWindow.textures.ContainsKey(textureKey))
			{
				//if a new texture not loaded, don't perform check
				if (textureKey != _AnimationDefinition["Texture"] || String.IsNullOrEmpty(sourcePath) || overrideTextureCheck)
				{
					textureIdentified = true;

					textureName.FontWeight = FontWeights.Bold;
					textureName.Background = new SolidColorBrush(Color.FromRgb(220, 230, 255));
					newTextureAlert.Visibility = Visibility.Hidden;
					existTextureAlert.Visibility = Visibility.Visible;

					textureInfo.Content = "";
					prevSheet.IsEnabled = false;
					nextSheet.IsEnabled = false;
					textureSheetCount = 0;

					_AnimationDefinition["Texture Path"] = "";
					_AnimationDefinition["Texture Path"] = MainWindow.textures[textureKey].Split(',').Last();
					_AnimationDefinition["Texture"] = textureKey;

					string textureFile = MainWindow.textures[textureKey].Split('/','\\').Last();
					string sourceFile = System.IO.Path.Combine(Settings.projectDirectory, Settings.animationSheetDir, textureFile + ".png");

					if (File.Exists(sourceFile))
					{
						int count = 0;
						string multiSheetPath;

						do
						{
							multiSheetPath = System.IO.Path.Combine(Settings.projectDirectory, Settings.animationSheetDir, textureFile + (++count).ToString() + ".png");
						} while (File.Exists(multiSheetPath));

						textureSheetCount = count;

						if (textureSheetCount > 0)
						{
							nextSheet.IsEnabled = true;
						}

						textureSheetIndex = 0;
					}
					else
					{
						TextureInstr.Text = "No preview available for this texture, no PNG is present.";

						Texture.Source = null;
						Texture.Visibility = Visibility.Collapsed;

						TextureSelect.Visibility = Visibility.Visible;
					}
				}

			}
			else
			{
                if (textureIdentified)
                {
                    textureIdentified = false;

                    Texture.Source = null;
                    Texture.Visibility = Visibility.Collapsed;

                    TextureSelect.Visibility = Visibility.Visible;

                    texturePath.Text = "";
                }

                newTextureAlert.Visibility = (String.IsNullOrEmpty(textureName.Text)) ? Visibility.Hidden : Visibility.Visible;
                existTextureAlert.Visibility = Visibility.Hidden;

                textureName.FontWeight = FontWeights.Normal;
                textureName.Background = new SolidColorBrush(Colors.White);
                
			}
		}

        private void updateSlicePreview(object sender, EventArgs e)
        {
            //clear the current preview
            slicePreview.RowDefinitions.Clear();
            slicePreview.ColumnDefinitions.Clear();
            slicePreview.Children.Clear();
            slicePreview.Margin = new Thickness(0);

            //define ouputs
            int iFrameWidth = 0;
            int iFrameHeight = 0;
            int iFrameCount = 0;
            int iAnimOffsetX = 0;
            int iAnimOffsetY = 0;
			float fWorldWidth = 0;
			float fWorldHeight = 0;
			
            //parse input values
            bool hasWidth = int.TryParse(frameWidth.Text, out iFrameWidth);
            bool hasHeight = int.TryParse(frameHeight.Text, out iFrameHeight);
            bool hasCount = int.TryParse(totalFrames.Text, out iFrameCount);
            bool hasOffsetX = int.TryParse(animOffsetX.Text, out iAnimOffsetX);
            bool hasOffsetY = int.TryParse(animOffsetY.Text, out iAnimOffsetY);
			bool hasWorldWidth = float.TryParse(worldWidth.Text, out fWorldWidth);
			bool hasWorldHeight = float.TryParse(worldHeight.Text, out fWorldHeight);

            //reset error states for inputs
            frameWidth.Background = new SolidColorBrush(Colors.White);
            frameHeight.Background = new SolidColorBrush(Colors.White);

            //minimum fields required to create preview
            if (hasWidth && hasHeight && hasCount && sourceTexture != null && iFrameWidth > 0 && iFrameHeight > 0)
            {
                //the number of rows and colums that the frames will comprise
                int columns = sourceTexture.PixelWidth / iFrameWidth;
                int rows = sourceTexture.PixelHeight / iFrameHeight;

                //the dimensions of the frames for the preview image
                int columnWidth = (int)(((float)iFrameWidth / (float)sourceTexture.PixelWidth) * Texture.ActualWidth);
                int rowHeight = (int)(((float)iFrameHeight / (float)sourceTexture.PixelHeight) * Texture.ActualHeight);

                //prevent invalid preview
                if(rows > 0 && columns > 0)
                { 
                    //create the column definitions
                    for (int i = 0; i < columns; i++)
                    {
                        ColumnDefinition newCol = new ColumnDefinition();
                        newCol.Width = new GridLength(columnWidth);

                        slicePreview.ColumnDefinitions.Add(newCol);
                    }

                    //create row definitions
                    for (int i = 0; i < rows; i++)
                    {
                        RowDefinition newRow = new RowDefinition();
                        newRow.Height = new GridLength(rowHeight);

                        slicePreview.RowDefinitions.Add(newRow);
                    }

                    //create the rectangles (frames) that overlay the texture
                    for (int i = 0; i < iFrameCount; i++)
                    {
                        Border newDiv = new Border();
                        newDiv.BorderThickness = new Thickness(1);
                        newDiv.BorderBrush = new SolidColorBrush(Colors.Black);
                        newDiv.SetValue(Grid.RowProperty, (int)(i / columns));
                        newDiv.SetValue(Grid.ColumnProperty, (int)(i % columns));

                        slicePreview.Children.Add(newDiv);
                    }

                    //the animation offset from the top-left of the spritesheet
				    int iRelOffsetX = (int)(Texture.ActualWidth * ((float)iAnimOffsetX / (float)sourceTexture.PixelWidth));
				    int iRelOffsetY = (int)( Texture.ActualHeight * ((float)iAnimOffsetY / (float)sourceTexture.PixelHeight));
				    slicePreview.Margin = new Thickness(iRelOffsetX,iRelOffsetY, 0, 0);

                    //check if we have enough information to create a preview of the object in the game world
				    if(hasWorldWidth && hasWorldHeight && iAnimOffsetX + iFrameWidth <= sourceTexture.PixelWidth && iAnimOffsetY + iFrameHeight <= sourceTexture.PixelHeight)
				    {
                        //pull the first frame to use for the preview
					    CroppedBitmap firstFrame = new CroppedBitmap();
					    firstFrame.BeginInit();
					    firstFrame.Source = sourceTexture;
					    firstFrame.SourceRect = new Int32Rect(iAnimOffsetX, iAnimOffsetY, iFrameWidth, iFrameHeight);
					    firstFrame.EndInit();

                        //apply the source
					    worldPreview.Source = firstFrame;
					    worldPreview.Stretch = Stretch.Fill;

                        //resize the image to match the given object dimensions and aspect ratio
                        string[] aspectRatioTerms = PreviewAspectRatio.SelectedValue.ToString().Split(':');

                        //calculate the aspect ratio as a float
                        int iAspectWidth = int.Parse(aspectRatioTerms[1]);
                        int iAspectHeight = int.Parse(aspectRatioTerms[2]);
                        float fAspectRatio = (float)iAspectWidth / (float)iAspectHeight;

                        //apply the aspect ratio to the frame's wdith
                        fWorldWidth *= fAspectRatio;

                        //determine wether to stretch the image horizontally or vertically so it can be as large as possible in the preview box
                        float worldDimensionRatio = fWorldWidth / fWorldHeight;
                        float wrapperDimensionRatio = (float)(worldPreviewWrapper.ActualWidth / worldPreviewWrapper.ActualHeight);

                        //if the image is stretched further horizontally
					    if(worldDimensionRatio > wrapperDimensionRatio)
					    {
						    worldPreview.Width = Math.Abs(worldPreviewWrapper.ActualWidth - 2);
						    worldPreview.Height = Math.Abs((1 / worldDimensionRatio) * (worldPreviewWrapper.ActualWidth - 2));
					    }
                        //if the image is strectched further vertically
					    else 
					    {
						    worldPreview.Width = Math.Abs((worldDimensionRatio) * (worldPreviewWrapper.ActualHeight - 2));
						    worldPreview.Height = Math.Abs(worldPreviewWrapper.ActualHeight - 2);
					    }
				    }
                }
                //indicate the given animation properties are invalid
                else 
                {
                    frameWidth.Background = new SolidColorBrush(Colors.Orange);
                    frameHeight.Background = new SolidColorBrush(Colors.Orange);
                }
            }
        }

		private void createAnimation(object sender, RoutedEventArgs e)
		{
			//titties titties titties
			saveTexture();

			if(checkAnimationName())
			{
				StreamWriter outStream = null;
				try
				{
					string filePath = System.IO.Path.Combine(Settings.projectDirectory,Settings.animationDefsDir,_AnimationDefinition["Name"]+"."+Settings.animationExt);
					outStream = new StreamWriter(filePath);

					outStream.WriteLine(_AnimationDefinition["Name"]);
					outStream.WriteLine(_AnimationDefinition["Texture"]);
					outStream.WriteLine(_AnimationDefinition["Frames"]);
					outStream.WriteLine(_AnimationDefinition["Frame Dur."]);
					outStream.WriteLine(_AnimationDefinition["Frame Width"]);
					outStream.WriteLine(_AnimationDefinition["Frame Height"]);
					outStream.WriteLine(_AnimationDefinition["Offset X"]);
					outStream.WriteLine(_AnimationDefinition["Offset Y"]);
					outStream.WriteLine(_AnimationDefinition["Obj Width"]);
					outStream.WriteLine(_AnimationDefinition["Obj Height"]);
				}
				catch(Exception ex)
				{
					MessageBox.Show("Error saving animation: " + ex.Message);
				}
				finally
				{
					if (outStream != null)
					{
						outStream.Close();
					}
				}
			}
		}
		public void saveTexture()
		{
			//   try
			//   {
			if (!String.IsNullOrEmpty(sourcePath) && !String.IsNullOrEmpty(_AnimationDefinition["Texture"]))
			{
				ContentBuilder contentBuilder = new ContentBuilder();

				//create a combo item for the texture
				compiledTextures = new ComboItem[textureSheetCount];

				if(textureSheetCount > 0)
				{
					for (int i = 0; i < textureSheetCount; i++)
					{
						string indexKey = (i == 0) ? "" : i.ToString();
						string multiSheetPath = System.IO.Path.Combine(Settings.projectDirectory, Settings.animationSheetDir, _AnimationDefinition["Texture Path"].Split('/','\\').Last() + indexKey + ".png");

						//create the list items
						compiledTextures[i] = new ComboItem(_AnimationDefinition["Texture"] + indexKey, multiSheetPath);
						contentBuilder.Add(compiledTextures[i]);
					}
				}
				else
				{
					compiledTextures = new ComboItem[1];
					compiledTextures[0] = new ComboItem(_AnimationDefinition["Texture"], sourcePath);
					contentBuilder.Add(compiledTextures[0]);
				}

				//Compile
				String error = contentBuilder.Build();

				if (!String.IsNullOrEmpty(error))
				{
					MessageBox.Show(error);
					return;
				}
				

				//Copy files to the output directory
				string tempPath = contentBuilder.OutputDirectory;
				outputFiles = Directory.GetFiles(tempPath, "*.xnb");

				tProcessBuildOutput = new Thread(new ParameterizedThreadStart(processBuildOutput));
				tProcessBuildOutput.Start();

				tProcessBuildOutput.Join();

				StreamWriter outStream = null;
				try
				{
					outStream = new StreamWriter(System.IO.Path.Combine(Settings.projectDirectory, Settings.textureDir, Settings.texturesFile));

					foreach (KeyValuePair<string, string> kvp in MainWindow.textures)
					{
						outStream.WriteLine(kvp.Key + "," + kvp.Value);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					if (outStream != null)
					{
						outStream.Close();
					}
				}

				checkTextureName(textureName.Text);
			}
			//    }
			//     catch (Exception ex)
			//     {
			//         MessageBox.Show(ex.Message);
			//     }
		}

        public void processBuildOutput(object obj)
        {
			//string fullOutPath = System.IO.Path.Combine( _AnimationDefinition["Texture Path"]);
			string[] textureDirs = _AnimationDefinition["Texture Path"].Split('\\', '/');
            string outputDir = System.IO.Path.Combine(textureDirs.Take(textureDirs.Length - 1).ToArray());
			outputDir = System.IO.Path.Combine(Settings.projectDirectory, Settings.textureDir, outputDir);

            //If the output directory doesn't exist, create it
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            foreach (string file in outputFiles)
            {

				string fileName = System.IO.Path.GetFileName(file);
				string textureKey = System.IO.Path.GetFileNameWithoutExtension(file);

				string capturedNum = Regex.Match(textureKey, @"\d+").Value;

				int index;
				string indexKey = "";
				if(int.TryParse(capturedNum, out index))
				{
					indexKey = (index == 0) ? "" : index.ToString();
				}
				
				string outputFileName = textureDirs.Last() + indexKey + ".xnb";
				
                string outputFile = System.IO.Path.Combine(outputDir, outputFileName);

                //if the destination file exists, delete it so we don't get an error
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                File.Copy(file, outputFile);

				if(textureSheetCount == 0)
				{ 
					//Copy the source image into the preview's directory
					string outputImage = System.IO.Path.Combine(Settings.projectDirectory, Settings.animationSheetDir, fileName + ".png");

					if(File.Exists(outputImage))
					{
						File.Delete(outputImage);
					}
					File.Copy(sourcePath, outputImage);
				}

				string[] texturePathPcs = _AnimationDefinition["Texture Path"].Split('\\', '/');
                string xnbPath = System.IO.Path.Combine(System.IO.Path.Combine(texturePathPcs.Take(texturePathPcs.Length - 1).ToArray()), System.IO.Path.GetFileNameWithoutExtension(outputFileName));
                //Add a key of the texture to the texture map
                if (MainWindow.textures.ContainsKey(textureKey))
                {
                    MainWindow.textures[textureKey] = xnbPath;
                }
                else
                {
                    MainWindow.textures.Add(textureKey, xnbPath);
                }

            }
        }

		private void checkAnimationName(object sender, TextChangedEventArgs e)
		{
			checkAnimationName();
		}

		private bool checkAnimationName()
		{
			animationName.FontWeight = FontWeights.Normal;
			animationName.Background = new SolidColorBrush(Colors.White);

			if (MainWindow.animations.Contains(animationName.Text))
			{
				if (action == ActionType.Edit && animationName.Text == MainWindow._AnimationDefinition["Name"])
				{
					animationName.FontWeight = FontWeights.Bold;
					animationName.Background = new SolidColorBrush(Color.FromRgb(220, 230, 255));
				}
				else
				{
					animationName.Background = new SolidColorBrush(Colors.Orange);
					return false;
				}
			}

			return true;
		}

        private void checkTexturePath(object sender, RoutedEventArgs e)
        {
            TextBox input = sender as TextBox;
            input.Background = new SolidColorBrush(Colors.White);
            if (!Directory.Exists(System.IO.Path.Combine(Settings.projectDirectory, Settings.textureDir, input.Text)))
            {
                input.Background = new SolidColorBrush(Colors.Orange);
            }
        }

		private void advanceTextureSheet(object sender, RoutedEventArgs e)
		{
			textureSheetIndex++;
		}
		private void goBackFrame(object sender, RoutedEventArgs e)
		{
			textureSheetIndex--;
		}

		private bool goToTextureSheet(int index)
		{
			nextSheet.IsEnabled = false;
			prevSheet.IsEnabled = false;
			textureSheetsInfo.Content = "";
			textureInfo.Content = "";

			string indexKey = (index == 0) ? "" : index.ToString();

			string multiSheetPath = System.IO.Path.Combine(Settings.projectDirectory, Settings.animationSheetDir, _AnimationDefinition["Texture Path"].Split('/','\\').Last() + indexKey + ".png");

			if (File.Exists(multiSheetPath))
			{
				sourcePath = multiSheetPath;

				sourceTexture = new BitmapImage();
				sourceTexture.BeginInit();

				sourceTexture.UriSource = new Uri(sourcePath);
				sourceTexture.EndInit();

				Texture.Source = sourceTexture;
				Texture.Visibility = Visibility.Visible;
				Texture.Width = textureContainer.ActualWidth;

				TextureSelect.Visibility = Visibility.Collapsed;

				textureInfo.Content = String.Format("{0}.png - {1:0} x {2:0} px", _AnimationDefinition["Texture Path"] + index, sourceTexture.PixelWidth, sourceTexture.PixelHeight);
				textureSheetsInfo.Content = String.Format("Sheet {0} of {1}", index + 1, textureSheetCount);

				if (index > 0) prevSheet.IsEnabled = true;
				if (index + 1 < textureSheetCount) nextSheet.IsEnabled = true;

				return true;

				
			}

			return false;
		}

		
    }
}
