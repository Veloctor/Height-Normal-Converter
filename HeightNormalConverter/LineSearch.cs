using System;

namespace Net.Kniaz.Optimization.QuasiNewton
{
	/// <summary>
	/// Summary description for LineSearch.
	/// </summary>
	public class LineSearch
	{

		
		public LineSearch()
		{
			//
			// TODO: Add constructor logic here
			//
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="f"></param>
		/// <param name="x0"></param>
		/// <param name="step"></param>
		/// <param name="maxIter"></param>
		/// <param name="xfin"></param>
		/// <returns></returns>
		public int FindMinInterval(IOneDFunction f, float x0, float step, int maxIter, ref float[]xfin)
		{
			float x1,ax, bx, cx;
			float fa, fb, fc;

			int i;
			int counter;
			counter=0;
			
			//by convention x1>x0
			x1 = x0+ Math.Abs(step);

			fa = f.GetVal(x0);
			fb = f.GetVal(x1);

			if (fa>fb)
			{
				ax = x0;
				bx = x1;
			}
			else
			{
				ax = x1;
				bx = x0;
				fb = fa;
			}

			float []x = new float[Constants.DIV+1];
			float []fx = new float[Constants.DIV+1];

			//searching inside the xo + step interval
			for (i = 0; i <= Constants.DIV; i++) 
			{
				x[i] = ax + i * (bx - ax) / ((float) Constants.DIV);
				fx[i] = f.GetVal(x[i]);
			}

			for (i = 1; i < Constants.DIV; i++)
				if (fx[i] < fx[i - 1] && fx[i] < fx[i + 1]) 
				{	/* the minimum has been found in the interval */
					xfin[0] = x[i - 1];
					xfin[1] = x[i];
					xfin[2] = x[i + 1];
					return counter;
				}

			//since no minimum has been found in the interval,
			//use Golden Section value to search for interval
			cx = bx + Constants.GOLDEN_SECTION_VAL * (bx - ax);
			fc = f.GetVal(cx);
			while (fb >= fc && counter<maxIter) 
			{
				ax = bx;
				bx = cx;
				cx = bx + Constants.GOLDEN_SECTION_VAL * (bx - ax);
				fb = fc;
				fc = f.GetVal(cx);
				counter++;
			}

			xfin[0] = ax;
			xfin[1] = bx;
			xfin[2] = cx;

			return counter;
		}

		/// <summary>
		/// Finds minimum via gold section search. x1 must be between x0 and x2
		/// </summary>
		/// <param name="f"></param>
		/// <param name="x0"></param>
		/// <param name="x1"></param>
		/// <param name="x2"></param>
		/// <param name="maxIter"></param>
		/// <param name="eps"></param>
		/// <param name="xmin"></param>
		/// <returns></returns>
		public int FindMinimumViaGoldenSection(IOneDFunction f, float x0, float x1, float x2, 
			int maxIter, float eps, ref float xmin)
		{
			float x3, f1,f2;

			x3 = x2;

			if (System.Math.Abs(x3 - x1) > Math.Abs(x1 - x0)) 
				//x0-x1 is the smaller segment
			{
				x2 = x1 + Constants.CGOLDEN_SECTION_VAL * (x3 - x1);
			} 
			else 
			{
				x2 = x1;
				x1 = x2 - Constants.CGOLDEN_SECTION_VAL * (x2 - x0);
			}

			f1 = f.GetVal(x1);
			f2 = f.GetVal(x2);

			int i = 0;
			while (Math.Abs(x3 - x0) > eps * (System.Math.Abs(x1) + System.Math.Abs(x2)) 
				&& i<maxIter) 
			{
				if (f2 < f1) 
				{
					x0 = x1;
					x1 = x2;
					//moving the bracket
					x2 = Constants.RGOLDEN_SECTION_VAL * x1 + Constants.CGOLDEN_SECTION_VAL * x3;
					f1 = f2;
					f2 = f.GetVal(x2);
				} 
				else 
				{
					x3 = x2;
					x2 = x1;
					x1 = Constants.RGOLDEN_SECTION_VAL * x2 + Constants.CGOLDEN_SECTION_VAL * x0;
					f2 = f1;
					f1 = f.GetVal(x1);
				}

				i++;
				
			}


			xmin = (f1<f2)?x1:x2;

			return i;

		} //end golden section search


        /// <summary>
        /// Finds minimum of the 1-dimensional function via a Brent Method:
        /// http://mathworld.wolfram.com/BrentsMethod.html
        /// </summary>
        /// <param name="f"></param>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="maxIter"></param>
        /// <param name="eps"></param>
        /// <param name="xmin"></param>
        /// <returns></returns>
        public int FindMinimumViaBrent(IOneDFunction f, float x0, float x1, float x2, 
										int maxIter, float eps, ref float xmin)
		{
			float a, b, d, e, u, v, w, x;
			float p, q, r;
			float fx, fu, fv, fw;
			float xm, tol1, tol2;
			float temp;
			int i, flag;
			// a and b must be in ascending order
			a = System.Math.Min(x0, x2);		
			b = System.Math.Max(x0, x2);
			x = w = v = x1;
			d = e = 0;

			fv = fw = fx = f.GetVal(x);

			i=0;
			for (i = 0; i < maxIter; i++) 
			{
				xm = (a + b) / 2;
				// this avoids searching for 0 
				tol1 = eps * System.Math.Abs(x) + Constants.TINY;	
				tol2 = 2 * tol1;
				if (System.Math.Abs(x - xm) <= (tol2 - (b - a) / 2)) 
				{
					xmin = x;
					return i;
				}

				flag = 0;
				//parabolic fit
				if (System.Math.Abs(e) > tol1) 
				{
					r = (x - w) * (fx - fv);
					q = (x - v) * (fx - fw);
					p = (x - v) * q - (x - w) * r;
					q = 2 * (q - r);
					if (q > 0)
						p = -p;
					q = System.Math.Abs(q);
					temp = e;
					e = d;
					// now check if parabolic fit is ok
					if (System.Math.Abs(p) < System.Math.Abs(.5 * q * temp) || (p > q * (a - x) && p < q * (b - x))) 
					{
						d = p / q;
						u = x + d;
						if (u - a < tol2 || b - u < tol2)
							d = tol1 * Math.Sign(xm - x);
						flag = 1;
					}
				}

				if (flag == 0) 
				{
					if (x >= xm)
						e = a - x;
					else
						e = b - x;
					d = Constants.CGOLDEN_SECTION_VAL * e;
				}
				if (System.Math.Abs(d) >= tol1)
					u = x + d;
				else
					u = x + tol1 * System.Math.Sign(d);

				fu = f.GetVal(u);
				if (fu <= fx) 
				{
					if (u >= x)
						a = x;
					else
						b = x;
					v = w;
					fv = fw;
					w = x;
					fw = fx;
					x = u;
					fx = fu;
				} 
				else 
				{
					if (u < x)
						a = u;
					else
						b = u;

					if (fu <= fw || w == x) 
					{
						v = w;
						fv = fw;
						w = u;
						fw = fu;
					} 
					else if (fu <= fv || v == x || v == w) 
					{
						v = u;
						fv = fu;
					}
				} //end of else
			}//end of the loop

			xmin = x;
			return i;
		}//end of Brent
	}
}
