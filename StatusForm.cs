using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualDisplayPlugin
{
	public partial class StatusForm : Form
	{
		private VirtualDisplayPlugin _plugin;

		public StatusForm(VirtualDisplayPlugin plugin)
		{
			_plugin = plugin;
			InitializeComponent();
		}

		public void SetDisplayImage(Bitmap picture)
		{
			pictureBox1.Image = picture;
			Application.DoEvents();
		}

		private void StatusForm_Load(object sender, EventArgs e)
		{

		}

		private void StatusForm_Shown(object sender, EventArgs e)
		{
			
		}
    }
}
