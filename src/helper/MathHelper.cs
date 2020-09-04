using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    class MathHelper
    {
        public static void LinearFit(double[] x, double[] y, out double a, out double b)
        {
            double xsum = 0;
            double ysum = 0;
            double xysum = 0;
            double x2sum = 0;
            int m = x.Length;
            for(int i=0; i<m; i++)
            {
                xsum = xsum + x[i];
                ysum = ysum + y[i];
                xysum = xysum + x[i] * y[i];
                x2sum = x2sum + x[i] * x[i];
            }
            a = (m * xysum - xsum * ysum) / (m * x2sum - xsum * xsum + 1e-10);
            b = (ysum - a * xsum) / m;
            return;
        }

        public static void LinearVal(double[] x, double a, double b, double[] y)
        {
            for(int i=0; i<x.Length; i++)
            {
                y[i] = a * x[i] + b;
            }
        }

        public static double Corrcoef(double[] d1, double[] d2)
        {
            double xy = 0, x = 0, y = 0, xsum = 0, ysum = 0;
            double corrc;
            int m = d1.Length > d2.Length?d2.Length:d1.Length;
            for(int i=0;i<m;i++)
            {
                xsum += d1[i];
                ysum += d2[i];
            }
            for(int i=0;i<m;i++)
            {
                x = x + (m * d1[i] - xsum) * (m * d1[i] - xsum);
                y = y + (m * d2[i] - ysum) * (m * d2[i] - ysum);
                xy = xy + (m * d1[i] - xsum) * (m * d2[i] - ysum);
            }
            corrc = Math.Abs(xy) / (Math.Sqrt(x) * Math.Sqrt(y));
            return corrc;
        }
    }
}
