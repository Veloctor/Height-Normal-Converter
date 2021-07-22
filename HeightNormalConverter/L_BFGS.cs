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
		//��Matrix�����һ��List<float[]>
		public Matrix sks, yks;//sk��yk����ʷֵ,ÿ�ε�����һ��
		IGradientFunction f;//f
		int Dimension;//��������
		public float[] xCurrent,gCurrent;//��ǰ�����ı���ֵ���ݶ�ֵ
		public float _alpha;//����

		public L_BFGS(float[] initialX, IGradientFunction f)//���캯��
		{
			this.f = f;
			Dimension = initialX.Length;
			xCurrent = initialX;
			Random rnd = new Random();
			_alpha = (float)rnd.NextDouble();//����Ϊ0-1֮��������;
			float[] g0 = f.GetGradient(initialX);
			OneDWrapper wrapper = new OneDWrapper(f, xCurrent, g0);  //������,��Ϊ����ֻ�Ǵ����˼���һ��n�еľ��󷽱�����������,nΪ����ά��
			LineSearch search = new LineSearch();
			float[] interval = new float[Constants.BRACKET_POINTS];
			int it1 = search.FindMinInterval(wrapper, _alpha, 1, 50, ref interval);//����������step
			int it2 = search.FindMinimumViaBrent(wrapper, interval[0], interval[1], interval[2], 50, 0.001f, ref _alpha);//0.1f epsilon
			float[] x1  = VectorSubtract(xCurrent, VectorScalarMul(g0, _alpha));
		    sks= new Matrix(VectorSubtract(x1,initialX));
			yks= new Matrix(VectorSubtract(f.GetGradient(x1), g0));
		}

		public void Update()//��������
        {
			if(gCurrent is null)//����ǵ�һ�ε���,gx���ǿյ�,��Ҫ���ݳ�ʼ������������ݶ�����
				gCurrent = f.GetGradient(xCurrent);

            // Compute search direction and step-size
            float[] d = BFGSMultiply(sks, yks, gCurrent);
			//��alphaֵ,ref����������ζ�ſ����ں������޸����ֵ,�ⲿ����ԭBFGS�㷨���ߵ�,��ֻȥ���˾����ʹ��,û���������Ķ�
			OneDWrapper wrapper = new OneDWrapper(f, xCurrent, d);	//������,��Ϊ����ֻ�Ǵ����˼���һ��n�еľ��󷽱�����������,nΪ����ά��
			LineSearch search = new LineSearch();
			float[] interval = new float[Constants.BRACKET_POINTS];
			int it1 = search.FindMinInterval(wrapper, _alpha,1, 1, ref interval);//����������step
			int it2 = search.FindMinimumViaBrent(wrapper, interval[0], interval[1], interval[2], 50, 0.1f, ref _alpha);//0.1f epsilon
			//��X(n+1)
			var xNext = VectorSubtract(xCurrent, VectorScalarMul(d, _alpha));

			// Store the input and gradient deltas
			var gNext = f.GetGradient(xNext);//g(X(n+1))
			sks.AddRowNoCopy(VectorSubtract(xNext, xCurrent));//sk���һ����ʷֵ,ֵΪX(n+1)-X(n)
			yks.AddRowNoCopy(VectorSubtract(gNext, gCurrent));//�ݶ�,ͬ��
			xCurrent = xNext;
			gCurrent = gNext;
		}

		public float[] BFGSMultiply(Matrix sks, Matrix yks, float[] direction)
		{
			int n = sks.Rows;
			float[] r = direction.Clone() as float[];
			float[] �� = new float[n];
			//compute right product
			for (int i = n-1; i >= 0; i--)
			{
				��[i] = VectorDot(sks[i], r) / VectorDot(yks[i], sks[i]);
				r = VectorSubtractEquals(r,VectorScalarMul(yks[i], ��[i]));
			}
			//compute center
			/*��ΪH0^(-1)�ǵ�λ����,��r�Ľ������r,�Ҿ�ʡ����*/
			//compute left product
			for (int i=0;i< n; i++)
            {
				var ��=VectorDot(yks[i],r) / VectorDot(yks[i], sks[i]);
				r = VectorAddEquals(r, VectorScalarMul(sks[i],��[n - i-1]- ��));
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
			else throw new ArgumentException("�������鳤�ȱ�����ͬ.");
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
			else throw new ArgumentException("�������鳤�ȱ�����ͬ.");
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
			else throw new ArgumentException("�������鳤�ȱ�����ͬ.");
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
			else throw new ArgumentException("�������鳤�ȱ�����ͬ.");
		}

		float VectorDot(float[] VectorA, float[] VectorB)//C=A��B
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
			else throw new ArgumentException("�������鳤�ȱ�����ͬ.");
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