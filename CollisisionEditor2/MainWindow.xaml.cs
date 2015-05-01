using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

namespace CollisisionEditor2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        //allows the canvas to be accessed in other classes
        public static Canvas textureCanvas;

		//list of animations
		public static ObservableCollection<String> animations;
        
        //mapping of texture names and file locations
		public static Dictionary<string, string> textures;

        //contains data about the currently loaded animation
		internal static Dictionary<string, string> _AnimationDefinition {get;private set;}

        //allows the animation definition to be bound to a DataList
        public static ObservableCollection<AnimationProperty> AnimationDefinition {
            get {
                //creat a new observable collection and load it with the animation definition data
                ObservableCollection<AnimationProperty> animDef = new ObservableCollection<AnimationProperty>();
                foreach(KeyValuePair<string,string> kvPair in _AnimationDefinition){
                    animDef.Add(new AnimationProperty() { Property = kvPair.Key, Value = kvPair.Value });
                }
                //return the animation definition
                return animDef; 
            }
        }

        //quick access to current frame count
        int frameCount;
        //temporary storage for frame number; used in currentFrame.changeText event handler
		int previousFrame;
		BitmapImage currentTexture;

		public ObservableCollection<Polygon> activeCollisionPolygon { get; set; }
        //stores the rectangles for the current animation
        List<PolygonData>[] collisionPolygons;
        //List<Rectangle> activeCollisionPolygon;

		//updates the active collision polygon collection
        DispatcherTimer updateTimer;

        //used when outputting the current animation
		Thread animationSave;
		Thread animIndexSave;

        public static ObservableCollection<AnimationProperty> customFields { get; set; }

		//Returns possible PolygonType values for binding
		public IEnumerable<PolygonType> PolygonTypeValues
		{
			get
			{
				return Enum.GetValues(typeof(PolygonType)).Cast<PolygonType>();
			}
		}

		public MainWindow()
		{
			InitializeComponent();

            this.WindowState = WindowState.Maximized;

            if (!Settings.Load())
            {
                this.Close();
            }
            else
            {
                if (!Settings.CheckConfiguration())
                {
                    MessageBox.Show("Invalid configuration, application exiting");
                    this.Close();
                }
            }
            
            //allows access to texture canvas
            textureCanvas = TextureWrapper;

            //Setup an empty animation defintion to display
            _AnimationDefinition = new Dictionary<string, string>();
            _AnimationDefinition.Add("Name","");
            _AnimationDefinition.Add("Texture","");
            _AnimationDefinition.Add("Frames","");
            _AnimationDefinition.Add("Frame Dur.","");
            _AnimationDefinition.Add("Frame Width","");
            _AnimationDefinition.Add("Frame Height","");
            _AnimationDefinition.Add("Offset X","");
            _AnimationDefinition.Add("Offset Y","");
            _AnimationDefinition.Add("Obj Width","");
            _AnimationDefinition.Add("Obj Height", "");

            //bind the animation definiton to the display (this has to be re-bound each time _AnimationDefinition is modified)
            AnimationInfo.ItemsSource = AnimationDefinition;
			//bind the collision poylgons
            CollisionPolygonList.ItemsSource = activeCollisionPolygon;

            customFields = new ObservableCollection<AnimationProperty>();

            updateTimer = new DispatcherTimer();
            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Interval = new TimeSpan(0,0,0,0,250);
            updateTimer.Start();

            //MessageBox.Show(System.IO.Directory.GetCurrentDirectory());
            //MessageBox.Show(File.Exists("../../../../RaptorRun/Binary/animations.txt").ToString());

            //setup variables
            collisionPolygons = null;
			textures = new Dictionary<string, string>();
			previousFrame = 0;
			animationSave = null;
            currentTexture = null;

			animations = new ObservableCollection<string>();
			AnimationList.ItemsSource = animations;

            //watch the frame textBox for changes
			CurrentFrame.TextChanged += CurrentFrame_TextChanged;

			//load in animations and textures from the index file
			LoadContent();

            //set test default for frame count
            frameCount = 5;

            //saves when the window is closed
			this.Closed += MainWindow_Closed;
		}

        void updateTimer_Tick(object sender, EventArgs e)
        {
            updateActiveCollisionPolygon();
        }

		void MainWindow_Closed(object sender, EventArgs e)
		{
            if (_AnimationDefinition["Name"] != "")
            {
                //save the current displayed polygon
                LoadCollisionPolygon(int.Parse(CurrentFrame.Text), int.Parse(CurrentFrame.Text));
                //save the animation
                SaveAnimation(new Object());
				SaveAnimationsIndex(new Object());
            }
		}

		/// <summary>
		/// Loads animations and textures indexes, and opens the first animation in the index
		/// </summary>
		void LoadContent()
		{
			//Load in texture definitions & animations
			StreamReader inStream = null;
			try
			{

				//Load in the animation names from the index file
				inStream = new StreamReader(System.IO.Path.Combine(Settings.projectDirectory, Settings.animationDefsDir, Settings.animationsIndex));

				string animation;
				while ((animation = inStream.ReadLine()) != null)
				{
					animations.Add(animation);
				}

				//load in the texture names from the texture index file
				string texturesPath = System.IO.Path.Combine(Settings.projectDirectory, Settings.textureDir, Settings.texturesFile);
				inStream = new StreamReader(texturesPath);

				string texture;
				while ((texture = inStream.ReadLine()) != null)
				{
					string[] textureHash = texture.Split(',');
					textures.Add(textureHash[0], textureHash[1]);
				}

				//Load the first animation
				LoadAnimation(animations.First<string>());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				if(inStream != null)
				{
					inStream.Close();
				}
			}
		}

		void SaveAnimationsIndex(object obj)
		{
			StreamWriter outStream = null;
			try
			{
				//open the index file for writing
				outStream = new StreamWriter(System.IO.Path.Combine(Settings.projectDirectory, Settings.animationDefsDir, Settings.animationsIndex));

				foreach(String key in animations)
				{
					outStream.WriteLine(key);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Animation Index Save Error"+ex.Message);
			}
			finally
			{
				if(outStream != null)
				{
					outStream.Close();
				}
			}

		}

        //FRAME NAVIGATION
        #region Frame Navigation Controls
        private void AdvanceFrame_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame.Text = (int.Parse(CurrentFrame.Text) + 1).ToString();
        }

        private void BackFrame_Click(object sender, RoutedEventArgs e)
        {
            CurrentFrame.Text = (int.Parse(CurrentFrame.Text) - 1).ToString();
        }

        //Track frame in textbox
        void CurrentFrame_TextChanged(object sender, EventArgs e)
        {
            TextBox input = sender as TextBox;
            int intVal;
            //make sure the value of the input is valid
            if (int.TryParse(input.Text, out intVal))
            {
                if (intVal >= 0 && intVal < frameCount && currentTexture != null)
                {
                    //load the new collision polygon and saves the current one
                    LoadCollisionPolygon(intVal, previousFrame);

                    //Display the correct frame of animation

                    //get the properties of the animation from the definition
                    int fWidth = int.Parse(_AnimationDefinition["Frame Width"]);
                    int fHeight = int.Parse(_AnimationDefinition["Frame Height"]);
                    int aOffsetX = int.Parse(_AnimationDefinition["Offset X"]);
                    int aOffsetY = int.Parse(_AnimationDefinition["Offset Y"]);

                    //get the max number of rows and columns of frames per sheet for this animation
                    int maxRowFrames = 2048 / fWidth;
                    int maxColumns = 2048 / fHeight;

                    //Determines what row to point to based on the current frame
                    //and the maximum frames in a row
                    int row = intVal / maxRowFrames;

                    //Determines column to point to based on currentframe and max frames
                    int column = intVal % maxRowFrames;

                    int spriteSheet = intVal / (maxRowFrames * maxColumns);

                    //calculate the frame offsets
                    int frameOffsetX = column * fWidth + aOffsetX;
                    int frameOffsetY = (row - (spriteSheet * maxColumns)) * fHeight + aOffsetY;

                    //get the file name of the current texture
                    string textureName = textures[_AnimationDefinition["Texture"]].Split('/','\\').Last() + ((spriteSheet > 0) ? spriteSheet.ToString() : "");

                    //Load in a png fo the current texture
                    currentTexture = new BitmapImage();
                    currentTexture.BeginInit();
                    string uri = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Settings.projectDirectory, Settings.animationSheetDir, textureName + ".png");
                    currentTexture.UriSource = new Uri(uri);
                    currentTexture.EndInit();

                    //unlike xna, wpf will crash if it tries to access any portion of the image beyond its dimensions, so try to ensure the source box is inside the image
                    if (frameOffsetX + fWidth > currentTexture.PixelWidth) fWidth = (int)currentTexture.PixelWidth - frameOffsetX;
                    if (frameOffsetY + fHeight > currentTexture.PixelHeight) fHeight = (int)currentTexture.PixelHeight - frameOffsetY;

                    //create the new frame
                    CroppedBitmap frame = new CroppedBitmap();
                    frame.BeginInit();
                    frame.Source = currentTexture;
                    frame.SourceRect = new Int32Rect(frameOffsetX, frameOffsetY, fWidth, fHeight);
                    frame.EndInit();

                    //setup the image to render the frame
					Texture.Stretch = Stretch.Fill;
                    Texture.Source = frame;
					Texture.Height = fHeight;
                    Texture.Width = fWidth;
					
                    TextureBorder.Width = fWidth;
                    TextureBorder.Height = fHeight;

                    previousFrame = intVal;
                }
                else
                {
                    //if (intVal < 0) intVal = 0;
                    //if (intVal >= frameCount) intVal = frameCount - 1;
                    intVal = previousFrame;
                }
                input.Text = intVal.ToString();
            }
            else
            {
                CurrentFrame.Text = previousFrame.ToString();
            }
        }
        #endregion


        //Load in animation from list
        void LoadSelectedAnimation(object sender, MouseButtonEventArgs e)
        {
			Label item = sender as Label;

			LoadAnimation(item.Content.ToString());
        }

		void LoadAnimation(string key)
		{
			#region Saving Current Animation
			//If there's animation open, we save it
			if (_AnimationDefinition["Name"] != "")
			{
				//ensure the current collsion polygon is saved
				LoadCollisionPolygon(int.Parse(CurrentFrame.Text), int.Parse(CurrentFrame.Text));

				/* Writing is done in a new thread because using try/catch automatically creates a new thread, 
				 * which would allow the new animation to be loaded before the current one is saved
				 */
				animationSave = new Thread(new ParameterizedThreadStart(SaveAnimation));
				animationSave.Start();

				animIndexSave = new Thread(new ParameterizedThreadStart(SaveAnimationsIndex));
				animIndexSave.Start();

				//wait for the animation to be save before continuing
				animationSave.Join();
				animIndexSave.Join();
			}
			#endregion

			#region Load Selected Animation

			StreamReader inStream = null;
			try
			{
				//open the animation definition
				inStream = new StreamReader(System.IO.Path.Combine(Settings.projectDirectory, Settings.animationDefsDir, key + "." + Settings.animationExt));

				//Load in the information about the animation
				_AnimationDefinition.Clear();
				_AnimationDefinition.Add("Name", inStream.ReadLine());
				_AnimationDefinition.Add("Texture", inStream.ReadLine());
				_AnimationDefinition.Add("Frames", inStream.ReadLine());
				_AnimationDefinition.Add("Frame Dur.", inStream.ReadLine());
				_AnimationDefinition.Add("Frame Width", inStream.ReadLine());
				_AnimationDefinition.Add("Frame Height", inStream.ReadLine());
				_AnimationDefinition.Add("Offset X", inStream.ReadLine());
				_AnimationDefinition.Add("Offset Y", inStream.ReadLine());
				_AnimationDefinition.Add("Obj Width", inStream.ReadLine());
				_AnimationDefinition.Add("Obj Height", inStream.ReadLine());

				//re-bind the definition to the display
				AnimationInfo.ItemsSource = AnimationDefinition;

				//reset the current frame
				CurrentFrame.Text = "0";
				//set the quick-access frame count
				frameCount = int.Parse(_AnimationDefinition["Frames"]);
				//reset the collision polygon array (there might not be any for a given animation)
				collisionPolygons = new List<PolygonData>[int.Parse(_AnimationDefinition["Frames"])];

				//instantiate each list in the collision polygon array
				for (int i = 0; i < frameCount; i++) collisionPolygons[i] = new List<PolygonData>();

				//if a collision polygon is defined, load it
				string collisionRectsString;
				if ((collisionRectsString = inStream.ReadLine()) != null)
				{
					//split the string into frames (delimted by ;), and then into individual polygon properties (denoted by ,)
					string[][] collisionInfo = collisionRectsString.Split(';').Select(s => s.Split(',')).ToArray();

					//process the polygon data for each frame
					for (int i = 0; i < frameCount; i++)
					{
						//the definition won't necessarily end nicely, so its easier just to wait for an error than try to prevent it
						try
						{
							//create a rectangle from each set of properties
							//format in file is left,top,width,height
							for (int p = 0; p < collisionInfo[i].Length; p += 4)
							{
								//again, things won't always come out nicely
								try
								{
									string[] typePcs = collisionInfo[i][p].Split(':');

									int type = 0;
									double left;
									if (typePcs.Length > 1)
									{
										type = int.Parse(typePcs[0]);
										left = double.Parse(typePcs[1]);
									}
									else
									{
										left = double.Parse(collisionInfo[i][p]);
									}

									double top = double.Parse(collisionInfo[i][p + 1]);
									double width = double.Parse(collisionInfo[i][p + 2]);
									double height = double.Parse(collisionInfo[i][p + 3]);

									collisionPolygons[i].Add(new PolygonData(left, top, width, height, (PolygonType)type));
								}
								catch (IndexOutOfRangeException)
								{
									Console.WriteLine("Reached end of polygon def");
								}
								catch (Exception ex)
								{
									Console.Write(ex.Message);
								}
							}
						}
						catch (IndexOutOfRangeException)
						{
							Console.WriteLine("No more frames");
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}

				customFields.Clear();
				for (int f = 0; f < Settings.customFields.Length; f++)
				{
					customFields.Add(new AnimationProperty(Settings.customFields[f], inStream.ReadLine() + ""));
				}

				//get the file name of the current texture
				string textureName = textures[_AnimationDefinition["Texture"]].Split('/', '\\').Last();

				//Load in a png fo the current texture
				currentTexture = new BitmapImage();
				try
				{
					currentTexture.BeginInit();
					string uri = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Settings.projectDirectory, Settings.animationSheetDir, textureName + ".png");
					currentTexture.UriSource = new Uri(uri);
				}
				catch (FileNotFoundException e)
				{
					MessageBox.Show("Source preview image for animation '" + key + "' not found at " + e.FileName);
				}
				finally
				{
					currentTexture.EndInit();
				}
				

				//get the draw properties of the current animation
				int fWidth = int.Parse(_AnimationDefinition["Frame Width"]);
				int fHeight = int.Parse(_AnimationDefinition["Frame Height"]);
				int aOffsetX = int.Parse(_AnimationDefinition["Offset X"]);
				int aOffsetY = int.Parse(_AnimationDefinition["Offset Y"]);

				//draw a specific portion of the texture (the first frame)
				CroppedBitmap frame = new CroppedBitmap();
				frame.BeginInit();
				frame.Source = currentTexture;
				frame.SourceRect = new Int32Rect(aOffsetX, aOffsetY, fWidth, fHeight);
				frame.EndInit();

				//apply the texture & draw rectangle to the Image control
				Texture.Stretch = Stretch.Fill;
				Texture.Source = frame;
				Texture.Width = fWidth;
				Texture.Height = fHeight;
				TextureBorder.Width = fWidth;
				TextureBorder.Height = fHeight;

				Console.WriteLine(Texture.ActualWidth + " " + Texture.ActualHeight);

				//load the collision polygon for the first frame
				LoadCollisionPolygon(0);
			}
			catch (FileNotFoundException e)
			{
				MessageBox.Show("Animation definition file for '" + key + "' does not exist at " + e.FileName);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Loading Animation: " + ex.Message);
			}
			finally
			{
				if (inStream != null)
				{
					inStream.Close();
				}
			}
			#endregion
		}
        
        public void updateActiveCollisionPolygon()
        {
            ContentControl[] currentPolygon = TextureWrapper.Children.OfType<ContentControl>().ToArray();

            ObservableCollection<Polygon> tempRects = new ObservableCollection<Polygon>();

            for (int c = 0; c < currentPolygon.Length; c++)
            {
                //only attempt to save if an index to save to was given
                double x = Canvas.GetLeft(currentPolygon[c]);
                double y = Canvas.GetTop(currentPolygon[c]);
                double width = currentPolygon[c].ActualWidth;
                double height = currentPolygon[c].ActualHeight;

				Polygon polygon = new Polygon(new Rect(x, y, width, height), currentPolygon[c], activeCollisionPolygon);
				polygon.selected = Selector.GetIsSelected(currentPolygon[c]);

				//We need to copy the active list to reference it, because it could change during the update function
				List<Polygon> copiedPolygon = activeCollisionPolygon.ToList();

				foreach(Polygon prevPoly in copiedPolygon)
				{
					if(prevPoly.controller.GetHashCode() == currentPolygon[c].GetHashCode())
					{
						polygon.type = prevPoly.type;
					}
				}

				tempRects.Add(polygon);
            }
            activeCollisionPolygon = tempRects;
			CollisionPolygonList.ItemsSource = activeCollisionPolygon;
        }

        //Displays a collision polygon from the current animation's array of polygons
        void LoadCollisionPolygon(int frameIndex, int? curFrameIndex = null)
        {
            //if there's a set of stored polygons
			if (collisionPolygons != null)
            {
                #region Save & Clear Current Polygon
				updateActiveCollisionPolygon();

                //collect the rects in the currently displayed polygon
                ContentControl[] currentPolygon = TextureWrapper.Children.OfType<ContentControl>().ToArray();

                //if there's polygon stored in the current index, clear it out
				if (curFrameIndex != null) collisionPolygons[(int)curFrameIndex].Clear();
                
                //process each content control/rectangle to get its cooridinates, then store them, and then clear the control
				for (int c = 0; c < activeCollisionPolygon.Count; c++)
				{
					Polygon polygon = activeCollisionPolygon[c];

                    //only attempt to save if an index to save to was given
					if(curFrameIndex != null)
					{
						double x = Canvas.GetLeft(polygon.controller);
						double y = Canvas.GetTop(polygon.controller);
						double width = polygon.controller.ActualWidth;
						double height = polygon.controller.ActualHeight;

						collisionPolygons[(int) curFrameIndex].Add(new PolygonData(x, y, width, height, polygon.type));
					}
				    
                    //remove the content control
					TextureWrapper.Children.Remove(polygon.controller);
				}
                //activeCollisionPolygon.Clear();
                #endregion

                #region Load Stored Polygon
                if (frameIndex < collisionPolygons.Length)
                {
					activeCollisionPolygon = new ObservableCollection<Polygon>();
                    //Load in the new polygon
                    for (int i = 0; i < collisionPolygons[frameIndex].Count(); i++)
                    {
						Rect rect = collisionPolygons[frameIndex][i].rect;
                        CreateControlRectangle(rect.X, rect.Y, rect.Width, rect.Height,collisionPolygons[frameIndex][i].type);
                    }
                    
                }
                updateActiveCollisionPolygon();
                #endregion
            }
        }

        //Add a rectangle to the canvas
		private void AddRectangle(object sender, RoutedEventArgs e)
		{
            CreateControlRectangle(100, 100, 100, 100);
            updateActiveCollisionPolygon();

            #region deprecated code
            /*
            Button button = sender as Button;
			switch(button.Content.ToString())
			{
				case "Open File":
					OpenFileDialog fileDialog = new OpenFileDialog();
					fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
					fileDialog.FileOk += fileDialog_FileOk;
					fileDialog.ShowDialog();
					break;
				case "Add Rectangle":
                    CreateControlRectangle(100, 100, 100, 100);
					break;
            }*/
            #endregion
        }

        //Deleting selected rectangle
        private void RectDelete_Click(object sender, RoutedEventArgs e)
        {
            ContentControl selectedControl = null;
            foreach (ContentControl control in TextureWrapper.Children.OfType<ContentControl>())
            {
                if (Selector.GetIsSelected(control))
                {
                    selectedControl = control;
                }
            }
            if (selectedControl != null)
            {
                TextureWrapper.Children.Remove(selectedControl);
                updateActiveCollisionPolygon();
            }
        }

        public void CreateControlRectangle(double left, double top, double width, double height, PolygonType type = 0)
        {
            //Create a new content control (allows the rectangle to be moved/resized)
            ContentControl newCC = new ContentControl();
            newCC.Width = width;
            newCC.Height = height;
            newCC.Style = Resources["DesignerItemStyle"] as Style;
			newCC.Background = new SolidColorBrush((type == PolygonType.Base) ? Color.FromArgb(140, 190, 205, 230) : Color.FromArgb(0, 100, 100, 100));
			newCC.MouseDown += SelectContentControl;
			newCC.MouseDoubleClick += SelectContentControl;

            Canvas.SetTop(newCC, top);
            Canvas.SetLeft(newCC, left);

            //create the rectangle
            Rectangle newRect = new Rectangle();
            newRect.Stroke = new SolidColorBrush(Colors.Red);
			newRect.Fill = new SolidColorBrush(Colors.Transparent);
            newRect.MouseDown += SelectContentControl;

            //display the new content control
            newCC.Content = newRect;
            TextureWrapper.Children.Add(newCC);

			Polygon polygon = new Polygon(new Rect(left, top, width, height), newCC, activeCollisionPolygon);
			polygon.type = type;
			activeCollisionPolygon.Add(polygon);
        }


        //Select ContentControl
        void SelectContentControl(object sender, MouseButtonEventArgs e)
        {
			Rectangle selectedControl;
			ContentControl contentControl = sender as ContentControl;
			if(contentControl != null)
			{
				selectedControl = contentControl.Content as Rectangle;
			}
			else
			{
				selectedControl = sender as Rectangle;
			}

			
			
            foreach (ContentControl control in TextureWrapper.Children.OfType<ContentControl>())
            {
                Selector.SetIsSelected(control, false);
				((Rectangle)control.Content).Fill = new SolidColorBrush(Colors.Transparent);

				if(selectedControl != null)
				{ 
					if(selectedControl.IsDescendantOf(control))
					{
						Selector.SetIsSelected(control, true);
						((Rectangle)control.Content).Fill = null;

						foreach (Polygon polygon in activeCollisionPolygon)
						{
							polygon.selected = false;
							if (polygon.controller == control)
							{
								polygon.selected = true;
							}
						}
					}
				}
            }

			try
			{
				if (selectedControl == null)
			{
				Label listElem = sender as Label;
				if (listElem != null)
				{
					Selector.SetIsSelected(((Polygon)listElem.DataContext).controller, true);
				}
			}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
        }

        #region Old image loading
        //Select an image to open
		void fileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				OpenFileDialog fileDialog = sender as OpenFileDialog;

				BitmapImage newTexture = new BitmapImage();
				newTexture.BeginInit();
				newTexture.UriSource = new Uri(fileDialog.FileName);
				newTexture.EndInit();

				Texture.Source = newTexture;

				ToolbarBtn1.Content = "Add Rectangle";

			}
			catch (Exception)
			{
				throw;
			}
		}
        #endregion

		/// <summary>
		/// Writes the current animation to a file
		/// </summary>
		/// <param name="obj">Empty object to allow this function to be called from a parameterized thread</param>
        public void SaveAnimation(object obj)
        {
			StreamWriter outStream = null;

            try
            {
                outStream = new StreamWriter(System.IO.Path.Combine(Settings.projectDirectory, Settings.animationDefsDir, _AnimationDefinition["Name"] + "." + Settings.animationExt));

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

				string polygons = "";
				for (int f = 0; f < collisionPolygons.Length; f++)
				{
					foreach(PolygonData polygon in collisionPolygons[f])
					{
						polygons += String.Format("{4}:{0},{1},{2},{3},", polygon.rect.X, polygon.rect.Y, polygon.rect.Width, polygon.rect.Height,(int)polygon.type);
					}
					//remove last comma
					polygons = polygons.TrimEnd(',');

					//add a semicolon to denote the end of the frame
					polygons += ";";
				}
				//remove last semicolon
				if(polygons.Length > 0)
					polygons = polygons.TrimEnd(';');

				outStream.WriteLine(polygons);

                for (int f = 0; f < customFields.Count; f++)
                {
                    outStream.WriteLine(customFields[f].Value);
                }

            }
            catch (Exception ex)
            {
				MessageBox.Show("Animation Save Error: " + ex.Message);
                //throw;
            }
			finally
			{
				if (outStream != null)
				{
					outStream.Close();
				}
			}
        }

        private void CreateAnimation(object sender, RoutedEventArgs e)
        {
            //open a new animation window
            Window newAnimationWin = new AnimPropertiesWindow();
            newAnimationWin.Show();
        }

        private void EditAnimation(object sender, RoutedEventArgs e)
        {
            //Setup the edit window
            AnimPropertiesWindow editAnimationWin = new AnimPropertiesWindow(_AnimationDefinition);
            editAnimationWin.Owner = this;
            editAnimationWin.Title = "Edit Animation";
            //Open it and wait until its closed
            editAnimationWin.ShowDialog();
			
			//read data from edit dialog and store it in the animation definition
			foreach(KeyValuePair<string, string> kvp in editAnimationWin._AnimationDefinition)
			{
                //don't allow extra information to be added to the definition
                if (_AnimationDefinition.ContainsKey(kvp.Key))
                {
                    _AnimationDefinition[kvp.Key] = kvp.Value;
                }
			}

            //update the animatiion properties display
            AnimationInfo.ItemsSource = AnimationDefinition;
            CurrentFrame_TextChanged(CurrentFrame, new EventArgs());
        }

		void FreezeUpdate()
		{
			updateTimer.Stop();
		}

		void ResumeUpdate()
		{
			updateTimer.Start();
		}

		private void PolygonTypeSelection_MouseEnter(object sender, MouseEventArgs e)
		{
			FreezeUpdate();
		}

		private void PolygonTypeSelection_MouseLeave(object sender, MouseEventArgs e)
		{
			ResumeUpdate();
		}

		private void PolygonTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			return;
			ComboBox comboBox = sender as ComboBox;
			for (int i = 0; i < CollisionPolygonList.Items.Count; i++)
			{
				ListBoxItem item = (ListBoxItem)CollisionPolygonList.Items[i];
				if(comboBox.IsDescendantOf(item))
				{
					Polygon polygon = (Polygon)item.DataContext;
					polygon.type = (PolygonType)comboBox.SelectedIndex;
				}
			}
		}

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWin = new SettingsWindow();
            settingsWin.ShowDialog();

            if (settingsWin.settingsSaved)
            {
                if (Settings.Load())
                {
                    Settings.CheckConfiguration();
                }
                
            }
        }

        private void EditCustomFields(object sender, RoutedEventArgs e)
        {
            Custom_Fields dialog = new Custom_Fields();
            dialog.ShowDialog();

            string test = customFields[0].Value;
        }
	}
}