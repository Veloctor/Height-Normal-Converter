using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeightNormalConverter;

namespace Net.Kniaz.Optimization.QuasiNewton
{
	/// <summary>
	/// Summary description for Optimizer.
	/// </summary>
	public class L_BFGS
	{
		//此Matrix类就是一个List<float[]>
		public Matrix sks, yks;//sk和yk的历史值,每次迭代加一行
		IGradientFunction f;//f
		int Dimension;//变量个数
		public float[] xCurrent,gCurrent;//当前迭代的变量值和梯度值
		public float _alpha;//步长

		public L_BFGS(float[] initialX, IGradientFunction f)//构造函数
		{
			this.f = f;
			Dimension = initialX.Length;
			xCurrent = initialX;
			Random rnd = new Random();
			_alpha = (float)rnd.NextDouble();//步长为0-1之间的随机数;
			float[] g0 = f.GetGradient(initialX);
			OneDWrapper wrapper = new OneDWrapper(f, xCurrent, g0);  //接上行,因为作者只是创建了几个一行n列的矩阵方便做向量计算,n为问题维度
			LineSearch search = new LineSearch();
			float[] interval = new float[Constants.BRACKET_POINTS];
			int it1 = search.FindMinInterval(wrapper, _alpha, 1, 50, ref interval);//第三个参数step
			int it2 = search.FindMinimumViaBrent(wrapper, interval[0], interval[1], interval[2], 50, 0.001f, ref _alpha);//0.1f epsilon
			float[] x1  = VectorSubtract(xCurrent, VectorScalarMul(g0, _alpha));
		    sks= new Matrix(VectorSubtract(x1,initialX));
			yks= new Matrix(VectorSubtract(f.GetGradient(x1), g0));
		}

		public void Update()//迭代更新
        {
			if(gCurrent is null)//如果是第一次迭代,gx就是空的,就要根据初始变量数组求得梯度数组
				gCurrent = f.GetGradient(xCurrent);

            // Compute search direction and step-size
            float[] d = BFGSMultiply(sks, yks, gCurrent);
			//求alpha值,ref传入引用意味着可以在函数内修改这个值,这部分是原BFGS算法作者的,我只去除了矩阵的使用,没有做其它改动
			OneDWrapper wrapper = new OneDWrapper(f, xCurrent, d);	//接上行,因为作者只是创建了几个一行n列的矩阵方便做向量计算,n为问题维度
			LineSearch search = new LineSearch();
			float[] interval = new float[Constants.BRACKET_POINTS];
			int it1 = search.FindMinInterval(wrapper, _alpha,1, 1, ref interval);//第三个参数step
			int it2 = search.FindMinimumViaBrent(wrapper, interval[0], interval[1], interval[2], 50, 0.1f, ref _alpha);//0.1f epsilon
			//求X(n+1)
			var xNext = VectorSubtract(xCurrent, VectorScalarMul(d, _alpha));

			// Store the input and gradient deltas
			var gNext = f.GetGradient(xNext);//g(X(n+1))
			sks.AddRowNoCopy(VectorSubtract(xNext, xCurrent));//sk添加一行历史值,值为X(n+1)-X(n)
			yks.AddRowNoCopy(VectorSubtract(gNext, gCurrent));//梯度,同上
			xCurrent = xNext;
			gCurrent = gNext;
		}

		public float[] BFGSMultiply(Matrix sks, Matrix yks, float[] direction)
		{
			int n = sks.Rows;
			float[] r = direction.Clone() as float[];
			float[] α = new float[n];
			//compute right product
			for (int i = n-1; i >= 0; i--)
			{
				α[i] = VectorDot(sks[i], r) / VectorDot(yks[i], sks[i]);
				r = VectorSubtractEquals(r,VectorScalarMul(yks[i], α[i]));
			}
			//compute center
			/*因为H0^(-1)是单位矩阵,乘r的结果还是r,我就省略了*/
			//compute left product
			for (int i=0;i< n; i++)
            {
				var β=VectorDot(yks[i],r) / VectorDot(yks[i], sks[i]);
				r = VectorAddEquals(r, VectorScalarMul(sks[i],α[n - i-1]- β));
			}
			return r;
		}
        #region vector calculators
        float[] VectorAdd(float[] VectorA, float[] VectorB)//C=A+B
		{
			if (VectorA.Length == VectorB.Length)
			{
				float[] result = new float[VectorA.Length];
				Parallel.For(0, VectorA.Length, i =>
				{
					result[i] = VectorA[i] + VectorB[i];
				});
				return result;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}


        float[] VectorAddEquals(float[] VectorA, float[] VectorB)//A+=B
		{
			if (VectorA.Length == VectorB.Length)
			{
				Parallel.For(0, VectorA.Length, i =>
				{
					VectorA[i] += VectorB[i];
				});
				return VectorA;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}

		float[] VectorSubtract(float[] VectorA, float[] VectorB)//C=A-B
		{
			if (VectorA.Length == VectorB.Length)
			{
				float[] result = new float[VectorA.Length];
				Parallel.For(0, VectorA.Length, i =>
				{
					result[i] = VectorA[i] - VectorB[i];
				});
				return result;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}

		float[] VectorSubtractEquals(float[] VectorA, float[] VectorB)//A-=B
		{
			if (VectorA.Length == VectorB.Length)
			{
				Parallel.For(0, VectorA.Length, i =>
				{
					VectorA[i] -= VectorB[i];
				});
				return VectorA;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}

		float VectorDot(float[] VectorA, float[] VectorB)//C=A·B
		{
			if (VectorA.Length == VectorB.Length)
			{
				float result = 0;
				Parallel.For(0, VectorA.Length, i =>
				{
					result += VectorA[i] * VectorB[i];
				});
				return result;
			}
			else throw new ArgumentException("两个数组长度必须相同.");
		}

		float[] VectorScalarMul(float[] Vector, float Scalar)//C=A*B
		{
            float[] result = new float[Vector.Length];
			Parallel.For(0, Vector.Length, i =>
			{
				result[i] = Vector[i] * Scalar;
			});
            return result;
		}
		float Diff(float[] x1, float[] x2)
		{
			float d = 0;
			for (int i = 0; i < Dimension; i++)
				d += (x1[i] - x2[i]) * (x1[i] - x2[i]);
			return (float)Math.Sqrt(d);
		}
        #endregion
    }
}