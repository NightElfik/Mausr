using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mausr.Core {
	[StructLayout(LayoutKind.Sequential)]
	public struct RawPoint {

		public float X;
		public float Y;
		public float T;

	}
}
