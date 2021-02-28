using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualDisplayPlugin
{
	public class DisplayPixel
	{
		public int X { get; set; }
		public int Y { get; set; }
		//public int z { get; set; }
		public int Chan { get; set; }
		//public int R { get; set; }
		//public int G { get; set; }
		//public int B { get; set; }
		public string ColorOrder { get; set; }
		public byte PixelChan { get; set; }
	}
}
