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
		

		public string _showDir = "";

		void OnReloadDimensions() => SendReloadDimensions.Invoke(this, null);

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

		public bool Start(string showDir)
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
			var path = _showDir + "//virtualdisplaymap";
			if (!System.IO.File.Exists(path))
				return false;
			_nodes.Clear();
			using (var rd = new StreamReader(path))
			{
				while (!rd.EndOfStream)
				{
					var splits = rd.ReadLine().Split(',');
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
							_nodes.Append(new DisplayPixel
							{
								X = x,
								Y = y,
								Chan = Convert.ToInt32(splits[2]),
								PixelChan = Convert.ToByte(splits[3]),
								ColorOrder = splits[4]
							});
						}
						catch (Exception e)
						{ }
					}
				}
			}

			return true;
		}

		/// <summary>
		/// This function is called for each frame
		/// </summary>
		public void ManipulateBuffer(PixelBuffer buffer)
		{
			OutputToPanel(buffer);
		}

		public void FireEvent(string type, string parameters)
		{
			//MessageBox.Show(parameters);
		}

		/// <summary>
		/// This Outputs the PixelBuffer data to the panel.
		/// </summary>
		async Task OutputToPanel(PixelBuffer buffer)
		{
			if (_outputing)
				return;

			_outputing = true;
			try
			{
				if (_pictureWidth == 0 || _pictureHeight == 0)
				{
					_outputing = false;
					return;
				}
				
				Bitmap display = new Bitmap(_pictureWidth, _pictureHeight);
				foreach (var node in _nodes)
				{
					Color color = getPixelColor(node, buffer);
					display.SetPixel(node.X, node.Y, color);
				}

				_form.SetDisplayImage(display);
				//OnSendError("No Ethernet Output Setup, Skipping Output");

			}
			catch (Exception ex)
			{
				//OnSendError(ex.Message);
			}
			_outputing = false;
		}

		Color getPixelColor( DisplayPixel pix, PixelBuffer buffer)
		{
			int r = 0;
			int g = 0;
			int b = 0;


			if ((pix.PixelChan == 3) ||
			 ((pix.PixelChan == 4) ))
			{
				if (pix.ColorOrder == "RGB" || pix.ColorOrder == "RGBW")
				{
					r = buffer[pix.Chan];
					g = buffer[pix.Chan + 1];
					b = buffer[pix.Chan + 2];
				}
				else if (pix.ColorOrder == "RBG")
				{
					r = buffer[pix.Chan];
					g = buffer[pix.Chan + 2];
					b = buffer[pix.Chan + 1];
				}
				else if (pix.ColorOrder == "GRB")
				{
					r = buffer[pix.Chan + 1];
					g = buffer[pix.Chan];
					b = buffer[pix.Chan + 2];
				}
				else if (pix.ColorOrder == "GBR")
				{
					r = buffer[pix.Chan + 2];
					g = buffer[pix.Chan];
					b = buffer[pix.Chan + 1];
				}
				else if (pix.ColorOrder == "BRG")
				{
					r = buffer[pix.Chan + 1];
					g = buffer[pix.Chan + 2];
					b = buffer[pix.Chan];
				}
				else if (pix.ColorOrder == "BGR")
				{
					r = buffer[pix.Chan + 2];
					g = buffer[pix.Chan + 1];
					b = buffer[pix.Chan];
				}
			}
			else if (pix.PixelChan == 1)
			{
				if (pix.ColorOrder == "Red")
				{
					r = buffer[pix.Chan];
					g = 0;
					b = 0;
				}
				else if (pix.ColorOrder == "Green")
				{
					r = 0;
					g = buffer[pix.Chan];
					b = 0;
				}
				else if (pix.ColorOrder == "Blue")
				{
					r = 0;
					g = 0;
					b = buffer[pix.Chan];
				}
				else if (pix.ColorOrder == "White")
				{
					r = buffer[pix.Chan];
					g = buffer[pix.Chan];
					b = buffer[pix.Chan];
				}
			}

			return Color.FromArgb(r, g, b);

			return Color.Black;
		}
	}
}