using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Mausr.Core {
	public class RawDataProcessor {

		/// <summary>
		/// Removes lines with less than two points.
		/// </summary>
		public void CleanData(RawDrawing drawing) {
			drawing.Lines = drawing.Lines
				.Where(l => l.Length >= 2)
				.ToArray();
		}

		/// <summary>
		/// Centers and expands the drawing to [0, 1] rectanlge.
		/// </summary>
		/// <param name="drawing"></param>
		public void Normalize(RawDrawing drawing) {

			float minX = float.PositiveInfinity,
				minY = float.PositiveInfinity,
				maxX = float.NegativeInfinity,
				maxY = float.NegativeInfinity;

			foreach (var line in drawing.Lines) {
				foreach (var rawPoint in line) {
					minX = Math.Min(minX, rawPoint.X);
					minY = Math.Min(minY, rawPoint.Y);
					maxX = Math.Max(maxX, rawPoint.X);
					maxY = Math.Max(maxY, rawPoint.Y);
				}
			}

			float wid = maxX - minX;
			float hei = maxY - minY;
			float scale = Math.Max(wid, hei);
			float xOffset = (1 - wid / scale) / 2;
			float yOffset = (1 - hei / scale) / 2;
			Contract.Assert(xOffset == 0 || yOffset == 0);
			
			foreach (var line in drawing.Lines) {
				for (int i = 0; i < line.Length; ++i) {
					line[i].X = xOffset + (line[i].X - minX) / scale;
					line[i].Y = yOffset + (line[i].Y - minY) / scale;
				}
			}
		}

	}
}