using System;

namespace Net.Kniaz.Optimization
{
	/// <summary>
	/// Summary description for Optimizer.
	/// </summary>
	public interface IGradientFunction
	{

        /// <summary>
        /// returns the value of the derivative 
        /// for the nth varaible at some point
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        float GetVal(float[] x);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        float[] GetGradient(float[]x);
	}
}
