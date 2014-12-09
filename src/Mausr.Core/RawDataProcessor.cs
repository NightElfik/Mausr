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


		public RawDrawing Normalize(RawDrawing drawing) {
			var newDrawing = new RawDrawing(drawing);
			NormalizeInPlace(newDrawing);
			return newDrawing;
		}

		/// <summary>
		/// Centers and expands the drawing to [0, 1] rectanlge.
		/// </summary>
		/// <param name="drawing"></param>
		public void NormalizeInPlace(RawDrawing drawing) {

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


		public RawDrawing Rotate(RawDrawing drawing, double angleDegrees) {
			var newDrawing = new RawDrawing(drawing);
			RotateInPlace(drawing, angleDegrees);
			return newDrawing;
		}

		public void RotateInPlace(RawDrawing drawing, double angleDegrees) {

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
			
			float sin = (float)Math.Sin(angleDegrees / 180 * Math.PI);
			float cos = (float)Math.Cos(angleDegrees / 180 * Math.PI);

			foreach (var line in drawing.Lines) {
				for (int i = 0; i < line.Length; ++i) {
					float dx = line[i].X - ox;
					float dy = line[i].Y - oy;
					line[i].X = cos * dx - sin * dy + ox;
					line[i].Y = sin * dx + cos * dy + oy;
				}
			}
		}

	}
}