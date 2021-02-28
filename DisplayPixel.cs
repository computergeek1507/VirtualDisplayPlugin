using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDisplayPlugin
{
	public class DisplayPixel
	{
		public DisplayPixel(int x, int y, int chan, int pixChan, string colorOrder)
		{
			X = x;
			Y = y;
			Chan = chan;
			PixelChan = pixChan;
			ColorOrder = colorOrder;
			SetPixelOreder();
		}

		void SetPixelOreder() {
			if (PixelChan == 3 || PixelChan == 4)
			{
				if (ColorOrder == "RGB" || ColorOrder == "RGBW")
				{
					R_Off = 0;
					G_Off = 1;
					B_Off = 2;
				}
				else if (ColorOrder == "RBG")
				{
					R_Off = 0;
					G_Off = 2;
					B_Off = 1;
				}
				else if (ColorOrder == "GRB")
				{
					R_Off = 1;
					G_Off = 0;
					B_Off = 2;
				}
				else if (ColorOrder == "GBR")
				{
					R_Off = 2;
					G_Off = 0;
					B_Off = 1;
				}
				else if (ColorOrder == "BRG")
				{
					R_Off = 1;
					G_Off = 2;
					B_Off = 0;
				}
				else if (ColorOrder == "BGR")
				{
					R_Off = 2;
					G_Off = 1;
					B_Off = 0;
				}
			}
			else if (PixelChan == 1)
			{
				{
					R_Off = 0;
					G_Off = 0;
					B_Off = 0;
				}
			}
		}


		public int X { get; set; }
		public int Y { get; set; }
		//public int z { get; set; }
		public int Chan { get; set; }
		public int R_Off { get; set; }
		public int G_Off { get; set; }
		public int B_Off { get; set; }

		public string ColorOrder { get; set; }
		public int PixelChan { get; set; }
	}
}
