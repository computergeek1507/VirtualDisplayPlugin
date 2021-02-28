using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xScheduleWrapper;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VirtualDisplayPlugin
{
	public class VirtualDisplayPlugin
	{
		public event EventHandler<string> SendError;
		public event EventHandler SendReloadDimensions;
	
		StatusForm _form;

		bool _outputing = false;

		int _pictureWidth = 0;
		int _pictureHeight = 0;
		List<DisplayPixel> _nodes = new List<DisplayPixel>();
		int _frame = 0;

		public string _showDir = "";

		public string GetMenuString()
		{
			return "VirtualDisplay Plugin";
		}
		public string GetWebFolder()
		{
			return "";
		}

		public bool xSchedule_Action(string command, string parameters, string data, out string buffer)
		{
			return xScheduleWrapper.xScheduleWrapper.Do_xSchedule_Action(command, parameters, data, out buffer);
		}

		public bool Load(string showDir)
		{
			_showDir = showDir;
			return true;
		}

		public void Unload()
		{
		}

		public bool HandleWeb(string command, string parameters, string data, string reference, out string response)
		{
			response = "";
			return false;
		}

		public bool Start(string showDir, string url)
		{
			_showDir = showDir;

			if (_form != null) return true;

			_form = new StatusForm(this);

			ReadSetting();

			_form.Show();

			return true;
		}

		public void Stop()
		{
			if (_form == null) return;

			_form.Close();
			_form = null;
		}

		public void WipeSettings()
		{

		}

		public void NotifyStatus(string status)
		{
		}

		/// <summary>
		/// readload settings on event from form window
		/// </summary>
		private void Reload_Setting(object sender, EventArgs e)
		{
			ReadSetting();
		}

		/// <summary>
		/// read the virtualdisplaymap file from the show directory
		/// </summary>
		private bool ReadSetting()
		{
			try
			{
				var path = _showDir + "//virtualdisplaymap";
				if (!System.IO.File.Exists(path))
				{
					MessageBox.Show("No File Found " + path);
					return false;
				}
				_nodes.Clear();
				using (var rd = new StreamReader(path))
				{
					while (!rd.EndOfStream)
					{
						var line = rd.ReadLine();
						if (line.StartsWith("#"))
						{
							continue;
						}
						var splits = line.Split(',');
						if (splits.Length == 5)
						{
							try
							{
								int x = Convert.ToInt32(splits[0]);
								int y = Convert.ToInt32(splits[1]);

								if (x > _pictureWidth)
								{
									_pictureWidth = x + 1;
								}

								if (y > _pictureHeight)
								{
									_pictureHeight = y + 1;
								}

								_nodes.Add(new DisplayPixel(
									x,
									y,
									Convert.ToInt32(splits[2]),
									Convert.ToInt32(splits[3]),
									splits[4]
								));
							}
							catch (Exception e)
							{
							}
						}
					}
				}
				if (_nodes.Count==0)
				{
					MessageBox.Show("No Nodes Found");
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// This function is called for each frame
		/// </summary>
		public void ManipulateBuffer(PixelBuffer buffer)
		{
			OutputToPanel(buffer);
		}

		public bool FireEvent(string type, string parameters)
		{
			//MessageBox.Show(parameters);
			return true;
		}

		public bool SendCommand(string command, string parameters, out string msg)
		{
			msg = "Demo plugin does not support commands.";
			return false;
		}

		/// <summary>
		/// This Outputs the PixelBuffer data to the panel.
		/// </summary>
		async Task OutputToPanel(PixelBuffer buffer)
		{
			if (_outputing)
				return;
			_frame++;
			if (_frame < 20)
			{
				return;
			}
			_frame = 0;
			_outputing = true;
			try
			{
				if (_pictureWidth == 0 || _pictureHeight == 0)
				{
					_outputing = false;
					return;
				}
				
				Bitmap display = new Bitmap(_pictureWidth, _pictureHeight);

				int w = display.Width;
				int h = display.Height;

				for (int x = 0; x < w; x++)
				{
					for (int y = 0; y < h; y++)
					{
						display.SetPixel(x, y, Color.Black);
					}
				}

				foreach (var node in _nodes)
				{
					Color color = getPixelColor(node, buffer);
					display.SetPixel(node.X, _pictureHeight - node.Y, color);
				}
				//display.Save(_showDir + "//virtualdisplay.jpg");
				_form.SetDisplayImage(display);
			}
			catch (Exception )
			{

			}
			_outputing = false;
		}

		Color getPixelColor( DisplayPixel pix, PixelBuffer buffer)
		{
			int r = buffer[pix.Chan + pix.R_Off];
			int g = buffer[pix.Chan + pix.G_Off];
			int b = buffer[pix.Chan + pix.B_Off];

			return Color.FromArgb(r, g, b);
		}
	}
}