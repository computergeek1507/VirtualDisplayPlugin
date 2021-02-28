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

		private Bitmap _displayPicture;

		public StatusForm(VirtualDisplayPlugin plugin)
		{
			_plugin = plugin;
			InitializeComponent();
		}

		public void SetDisplayImage(Bitmap picture)
		{
			_displayPicture = picture;
		}

		private void StatusForm_Load(object sender, EventArgs e)
		{

		}

		private void StatusForm_Shown(object sender, EventArgs e)
		{
			
		}
				
		private void checkBoxTest_CheckedChanged(object sender, EventArgs e)
		{

		}

        private void StatusForm_Paint(object sender, PaintEventArgs e)
        {
			e.Graphics.DrawImage(_displayPicture, 0, 0, _displayPicture.Width,
				_displayPicture.Height);
		}
    }
}
