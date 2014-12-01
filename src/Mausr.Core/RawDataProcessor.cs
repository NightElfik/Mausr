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



		public RawDrawing Rotate(RawDrawing drawing, double angleDegrees, bool normalize) {

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

			float ox = (minX + maxX) / 2;
			float oy = (minY + maxY) / 2;

			var newDrawing = new RawDrawing(drawing);

			float sin = (float)Math.Sin(angleDegrees / 180 * Math.PI);
			float cos = (float)Math.Cos(angleDegrees / 180 * Math.PI);

			foreach (var line in newDrawing.Lines) {
				for (int i = 0; i < line.Length; ++i) {
					float dx = line[i].X - ox;
					float dy = line[i].Y - oy;
					line[i].X = cos * dx - sin * dy + ox;
					line[i].Y = sin * dx + cos * dy + oy;
				}
			}

			if (normalize) {
				Normalize(newDrawing);
			}
			return newDrawing;
		}

	}
}