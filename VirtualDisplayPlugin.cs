using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xScheduleWrapper;
using System.Drawing;
using System.Windows.Forms;

namespace VirtualDisplayPlugin
{
	public class VirtualDisplayPlugin
	{
		public event EventHandler<string> SendError;
		public event EventHandler SendReloadDimensions;
	
		StatusForm _form;

		bool _outputing = false;

		public int _panelWidth = 0;
		public int _panelHeight = 0;

		public string _showDir = "";

		void OnSendError(string errorString) => SendError.Invoke(this, errorString);

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

			SendError += _form.StatusFormMeasage;
			SendReloadDimensions += _form.ReloadStatusBox;
			_form.ReloadSettings += Reload_Setting;

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
		/// read the setting XML file from the show directory
		/// </summary>
		private bool ReadSetting()
		{
			PluginSettings setting = new PluginSettings(_showDir);
			_selectedOutput = setting.EthernetOutput;
			_selectedMatrix = setting.MatrixName;
			_brightness = setting.Brightness;

			if (_brightness == 0)
			{
				_brightness = 100;
			}

			for (int i = 0; i != _allDevices.Count; ++i)
			{
				LivePacketDevice device = _allDevices[i];
				if (device.Name == _selectedOutput)
				{
					_intSelectOutput = i;
					break;
				}
			}

			if (_intSelectOutput == -1)
			{
				OnSendError("Ethernet Ouput not found");
				return false;
			}

			string result;
			xSchedule_Action("GetMatrix", _selectedMatrix, "", out result);

			Matrix settings = JsonConvert.DeserializeObject<Matrix>(result);
			if (!string.IsNullOrEmpty(settings.name))
			{
				_panelWidth = int.Parse(settings.width);
				_panelHeight = int.Parse(settings.height);
				_startChannel = int.Parse(settings.startchannel);
				OnReloadDimensions();
			}
			else
			{
				OnSendError(_selectedMatrix + " Matrix not found");
				return false;
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
				if (_intSelectOutput == -1)
				{
					OnSendError("No Ethernet Output Setup, Skipping Output");
					return;
				}

				if (_startChannel == -1)
				{
					OnSendError("No Matrix Set, Skipping Output");
					return;
				}
				PacketDevice selectedDevice = _allDevices[_intSelectOutput];
				using (PacketCommunicator communicator = selectedDevice.Open(100, // name of the device
																		 PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
																		 100)) // read timeout
				{
					MacAddress source = new MacAddress("22:22:33:44:55:66");

					// set mac destination to 02:02:02:02:02:02
					MacAddress destination = new MacAddress("11:22:33:44:55:66");

					// Ethernet Layer
					int pixelWidth = _panelWidth;
					int pixelHeight = _panelHeight;
					int startChannel = _startChannel;

					communicator.SendPacket(BuildFirstPacket(source, destination));
					communicator.SendPacket(BuildSecondPacket(source, destination));
					for (int i = 0; i < pixelHeight; i++)
					{
						int offset = pixelWidth * i;
						communicator.SendPacket(BuildPixelPacket(source, destination, i, pixelWidth, buffer, startChannel, offset));
					}
				}
			}
			catch (Exception ex)
			{
				OnSendError(ex.Message);
			}
			_outputing = false;
		}
	}
}