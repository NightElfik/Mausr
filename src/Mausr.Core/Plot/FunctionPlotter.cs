using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using MathNet.Numerics.LinearAlgebra.Double;
using Mausr.Core.Optimization;

namespace Mausr.Core.Plot {
	public class FunctionPlotter {

		public Bitmap ContourPlot(IFunctionWithDerivative funcition, DenseVector origin, int xDimIndex, int yDimIndex,
				int imgWidth, int imgHeight, double stepPerPixel, int contoursCount, double scalePower) {

			DenseVector point = new DenseVector(origin.Count);
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
			return img;
		}

	}
}
