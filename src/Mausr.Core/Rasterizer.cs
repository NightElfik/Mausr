using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mausr.Core {
	public class Rasterizer {

		public Bitmap Rasterize(RawDrawing drawing, int imageSize, float penSize) {
			var img = new Bitmap(imageSize, imageSize, PixelFormat.Format24bppRgb);

			var pen = new Pen(Color.Black, penSize) {
				StartCap = LineCap.Round,
				EndCap = LineCap.Round,
				LineJoin = LineJoin.Round,
			};

			using (var g = Graphics.FromImage(img)) {
				g.Clear(Color.White);
				g.SmoothingMode = SmoothingMode.HighQuality;

				foreach (var line in drawing.Lines) {
					if (line.Length == 0) {
						continue;
					}

					var pts = new PointF[line.Length];

					for (int i = 0; i < line.Length; i++) {
						pts[i].X = line[i].X * imageSize;
						pts[i].Y = line[i].Y * imageSize;
					}
					g.DrawLines(pen, pts);

					for (int i = 0; i < line.Length; i++) {
						int x = (int)Math.Round(pts[i].X);
						int y = (int)Math.Round(pts[i].Y);
						g.DrawLine(Pens.Red, x, y, x + 0.01f, y);
					}

				}
			}

			return img;
		}

	}
}
