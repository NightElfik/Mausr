using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mausr.Core {
	public class Rasterizer {

		private static Pen[] pointsColors = new Pen[] { Pens.Red, Pens.Lime, Pens.Yellow };

		public int ImageSize { get; set; }
		public float PenSizePerc { get; set; }
		public bool ExtraMargin { get; set; }
		public bool DrawPoints { get; set; }

		

		public Bitmap Rasterize(RawDrawing drawing) {
			return Rasterize(drawing, ImageSize, PenSizePerc, ExtraMargin, DrawPoints);
		}

		public void Rasterize(Bitmap img, RawDrawing drawing) {
			Contract.Requires(img.Width == img.Height);

			Rasterize(img, drawing, PenSizePerc, ExtraMargin, DrawPoints);
		}


		public Bitmap Rasterize(RawDrawing drawing, int imageSize, float penSizePerc, bool extraMargin, bool drawPoints) {
			Bitmap img = new Bitmap(imageSize, imageSize, PixelFormat.Format24bppRgb);
			Rasterize(img, drawing, penSizePerc, extraMargin, drawPoints);
			return img;
		}

		public void Rasterize(Bitmap img, RawDrawing drawing, float penSizePerc, bool extraMargin, bool drawPoints) {
			Contract.Requires(img.Width == img.Height);

			int imageSize = img.Width;
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
		}

	}
}
