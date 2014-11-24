using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;

namespace Mausr.Core.Plot {
	public class FunctionPlotter {

		public Bitmap ContourPlot(IFunctionWithDerivative funcition, Vector<double> origin, int xDimIndex, int yDimIndex,
				int imgWidth, int imgHeight, double stepPerPixel, int contoursCount, double scalePower,
				List<Tuple<Color, List<Vector<double>>>> pointGroups) {

			var point = new DenseVector(origin.Count);
			origin.CopyTo(point);

			double xOrigin = origin[xDimIndex] - stepPerPixel * imgWidth / 2;
			double yOrigin = origin[yDimIndex] - stepPerPixel * imgHeight / 2;

			double min, max;
			min = max = funcition.Evaluate(origin);

			double[,] values = new double[imgHeight, imgWidth];
			for (int y = 0; y < imgHeight; ++y) {
				point[yDimIndex] = yOrigin + (imgHeight - y - 1) * stepPerPixel;

				for (int x = 0; x < imgWidth; ++x) {
					point[xDimIndex] = xOrigin + x * stepPerPixel;

					double value = funcition.Evaluate(point);
					values[y, x] = value;

					if (value < min) {
						min = value;
					}
					else if (value > max) {
						max = value;
					}
				}
			}

			double range = max - min;

			var img = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
			var imgData = img.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

			int[,] contours = new int[imgHeight, imgWidth];

			unsafe {
				byte* imgPtr = (byte*)imgData.Scan0;
				int imgStride = imgData.Stride;

				// Image colors.
				for (int y = 0; y < imgHeight; ++y) {
					int stride = y * imgStride;

					for (int x = 0; x < imgWidth; ++x) {
						int contour = (int)(contoursCount * Math.Pow((values[y, x] - min) / range, scalePower));
						if (contour >= contoursCount) {
							contour = contoursCount - 1;  // Fox for values that are equa to max.
						}

						Contract.Assert(contour >= 0 && contour < contoursCount);
						contours[y, x] = contour;
						byte color = (byte)(55 + 200 * contour / (contoursCount - 1));

						int baseI = stride + x * 3;
						imgPtr[baseI] = color;
						imgPtr[baseI + 1] = color;
						imgPtr[baseI + 2] = color;
					}
				}

				// Contours in x direction.
				for (int y = 0; y < imgHeight; ++y) {
					int stride = y * imgStride;

					for (int x = 1; x < imgWidth; ++x) {
						if (contours[y, x] == contours[y, x - 1]) {
							continue;
						}

						int baseI = stride + x * 3;
						imgPtr[baseI] = 0;
						imgPtr[baseI + 1] = 0;
						imgPtr[baseI + 2] = 0;
					}
				}

				// Contours in y direction.
				for (int x = 0; x < imgWidth; ++x) {
					int offset = x * 3;

					for (int y = 1; y < imgHeight; ++y) {
						if (contours[y, x] == contours[y - 1, x]) {
							continue;
						}

						int baseI = y * imgStride + offset;
						imgPtr[baseI] = 0;
						imgPtr[baseI + 1] = 0;
						imgPtr[baseI + 2] = 0;
					}
				}
			}

			img.UnlockBits(imgData);

			// Points.
			if (pointGroups != null) {
				using (var g = Graphics.FromImage(img)) {
					g.SmoothingMode = SmoothingMode.HighQuality;
					int labelY = 10;
					int labelYSpace = 20;
					g.FillRectangle(new SolidBrush(Color.FromArgb(192, Color.White)), 8, labelY - 4, 40, labelYSpace * pointGroups.Count);

					foreach (var tuple in pointGroups) {
						var points = tuple.Item2;
						if (points.Count < 2) {
							continue;
						}

						Pen arrowPen = new Pen(tuple.Item1, 1);
						arrowPen.EndCap = LineCap.ArrowAnchor;

						PointF previousPt = new PointF(
							(float)((points[0][xDimIndex] - xOrigin) / stepPerPixel),
							imgHeight - (float)((points[0][yDimIndex] - yOrigin) / stepPerPixel) - 1);

						const int margin = 1024;

						// Arrows.
						for (int i = 1; i < points.Count; ++i) {
							PointF pt = new PointF(
								(float)((points[i][xDimIndex] - xOrigin) / stepPerPixel),
								imgHeight - (float)((points[i][yDimIndex] - yOrigin) / stepPerPixel) - 1);
							if (pt.X < -margin || pt.Y < -margin || pt.X >= imgHeight + margin || pt.Y >= imgHeight + margin) {
								continue;
							}

							g.DrawLine(arrowPen, previousPt, pt);
							previousPt = pt;
						}

						string steps = points.Count.ToString();
						g.DrawString(steps, SystemFonts.DefaultFont, new SolidBrush(tuple.Item1), 10, labelY);
						labelY += labelYSpace;
					}
				}

			}

			return img;
		}

	}
}
