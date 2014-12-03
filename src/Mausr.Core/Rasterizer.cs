using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mausr.Core {
	public class Rasterizer {

		private static Pen[] pointsColors = new Pen[] { Pens.Red, Pens.Lime, Pens.Yellow };

		public Bitmap Rasterize(RawDrawing drawing, int imageSize, float penSizePerc, bool extraMargin, bool drawPoints) {
			var img = new Bitmap(imageSize, imageSize, PixelFormat.Format24bppRgb);

			float penSize = imageSize * penSizePerc;
			var pen = new Pen(Color.Black, penSize) {
				StartCap = LineCap.Round,
				EndCap = LineCap.Round,
				LineJoin = LineJoin.Round,
			};

			float drawingSize = imageSize - 1;
			float offset = 0;

			if (extraMargin) {
				// Make sure to fill all available space
				drawingSize += 2;
				offset = (float)Math.Ceiling(penSize / 2);
				drawingSize -= 2 * offset;
				offset -= 1;
			}

			using (var g = Graphics.FromImage(img)) {
				g.Clear(Color.White);
				g.SmoothingMode = SmoothingMode.HighQuality;

				foreach (var line in drawing.Lines) {
					if (line.Length == 0) {
						continue;
					}

					var pts = new PointF[line.Length];

					for (int i = 0; i < line.Length; i++) {
						pts[i].X = offset + line[i].X * drawingSize;
						pts[i].Y = offset + line[i].Y * drawingSize;
					}
					g.DrawLines(pen, pts);
				}

				if (drawPoints) {
					int lineIndex = 0;
					foreach (var line in drawing.Lines) {
						if (line.Length == 0) {
							continue;
						}

						var ptPen = pointsColors[lineIndex % pointsColors.Length];
						for (int i = 0; i < line.Length; i++) {
							float x = (float)Math.Round(offset + line[i].X * drawingSize);
							float y = (float)Math.Round(offset + line[i].Y * drawingSize);
							g.DrawLine(ptPen, x, y, x + 0.2f, y);
							g.DrawLine(ptPen, x, y, x - 0.2f, y);
						}

						lineIndex += 1;
					}
				}
			}

			return img;
		}

	}
}
