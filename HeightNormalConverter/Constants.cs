using System;

namespace Net.Kniaz.Optimization
{
	class Constants
	{

        //Rosenbrockbrock constants
	    public const float RosenbrockBeta = 0.5f;
	    public const float RosenbrockAlfa  = 3;
        //Nelder-Mead constants
        public const float NelderLambda = 1;
        public const float NelderBeta = 2;
        public const float NelderGamma = 0.5f;

		//one dimensional search constants
		public static int DIV = 8;
		public static float TINY = 1e-5f;
		public static int BRACKET_POINTS = 3;
	
		public static float GOLDEN_SECTION_VAL 
		{
            get { return ((float)Math.Sqrt(5f) + 1) / 2; }
		}

		public static float RGOLDEN_SECTION_VAL
		{
			get {return (GOLDEN_SECTION_VAL-1);}
		}

		public static float CGOLDEN_SECTION_VAL
		{
			get {return (1 - RGOLDEN_SECTION_VAL);}
		}


		/// <summary>
		/// Generates n x m jagged array of doubles
		/// </summary>
		/// <param name="m"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static float[][] GenerateMatrix(int m, int n)
		{
			float[][] A = new float[m][];
			for (int i = 0; i < m; i++)
			{
				A[i] = new float[n];
			}

			return A;
		}


	}
}

