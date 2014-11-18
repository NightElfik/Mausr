using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mausr.Core {
	public class RawDrawing {

		public RawPoint[][] Lines { get; set; }

		public int LinesCount { get { return Lines == null ? -1 : Lines.Length; } }



		public byte[] Serialize() {
			Contract.Requires(Lines != null);
			Contract.Requires(Contract.ForAll(Lines, l => l != null));

			const int sizeOfInt = 4;
			int sizeOfPoint = Marshal.SizeOf<RawPoint>();

			int dataLength = sizeOfPoint * Lines.Sum(l => l.Length)  // Data length
				+ sizeOfInt * (Lines.Length + 1);  // Counts

			int dataIndex = 0;
			byte[] data = new byte[dataLength];

			SerializationHelper.SerializeInt(Lines.Length, data, ref dataIndex);

			for (int l = 0; l < Lines.Length; ++l) {
				var line = Lines[l];
				SerializationHelper.SerializeInt(line.Length, data, ref dataIndex);
				SerializationHelper.SerializeArray(line, data, ref dataIndex);
			}

			return data;
		}

		public static RawDrawing Deserialize(byte[] data) {
			int dataIndex = 0;
			var resultDrawing = new RawDrawing();

			int linesCount = SerializationHelper.DeserializeInt(data, ref dataIndex);

			resultDrawing.Lines = new RawPoint[linesCount][];

			for (int l = 0; l < linesCount; ++l) {
				int lineLength = SerializationHelper.DeserializeInt(data, ref dataIndex);
				resultDrawing.Lines[l] = new RawPoint[lineLength];
				SerializationHelper.DeserializeArray(resultDrawing.Lines[l], data, ref dataIndex);
			}

			return resultDrawing;
		}
	}
}
