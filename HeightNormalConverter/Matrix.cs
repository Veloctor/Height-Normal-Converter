using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeightNormalConverter
{
	internal class Maths
	{
		/// <summary>
		/// square root (a^2 + b^2) without under/over flow
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>float square root</returns>
		public static float Hypot(float a, float b)
		{
			float r;
			if (Math.Abs(a) > Math.Abs(b))
			{
				r = b / a;
				r = Math.Abs(a) * (float)Math.Sqrt(1 + r * r);
			}
			else if (b != 0)
			{
				r = a / b;
				r = Math.Abs(b) * (float)Math.Sqrt(1 + r * r);
			}
			else r = 0;
			return r;
		}
	}
	///<summary>��������:��ȹ̶�(����),�߶ȿɱ�(List����),��������м���.
	///���ص���������ص����½�����,��Ӱ��ԭ�����ֵ,����������*��*�ı�ԭ�����ֵ������</summary>
	public class Matrix
	{
		/// <summary>����ֱ�Ӳ����˾����������ע��ÿ�����鳤����ͬ!!!</summary>
		public List<float[]> Array;

		/// <summary>���������</summary>
		public int Rows { get { return Array.Count; } }

		/// <summary>���������</summary>
		public int Columns { get { return Array[0].Length; } }

		#region constructors
		/// <summary>�½�һ������</summary>
		public Matrix()
		{
			Array = new List<float[]>();
		}
		/// <summary>�½�һ������.����ʱȷ�����������ɽ���List��̬���ݿ���</summary>
		/// <param name="rows">�����ʼ����(List<float[]>�ĳ�ʼ����).</param>
		public Matrix(int rows)
		{
			Array = new List<float[]>(rows);
		}
		public Matrix(int rows,float[] FirstRow)
		{
            Array = new List<float[]>(rows)
            {
                FirstRow
            };
        }
		/// <summary>�½�һ������</summary>
		/// <param name="from">�½��ľ����Ǵ˾���ĸ���.</param>
		public Matrix(Matrix from)
		{
			Array = new List<float[]>(from.Array);
		}
		/// <summary>�½�һ������</summary>
		/// <param name="FirstRow">
		/// Ԫ�صĵ�һ������,�˾���Ŀ�Ƚ����̶�Ϊ������ĳ���
		/// </param>
		public Matrix(float[] FirstRow)
		{
			Array = new List<float[]>();
			float[] copy = new float[FirstRow.Length];
			FirstRow.CopyTo(copy, 0);
			Array.Add(copy);
		}
		#endregion
		/// <summary>��û����ô˾����ĳ��Ԫ��</summary>
		/// <param name="i">Ԫ�����ڵ���</param>
		/// <param name="j">Ԫ�����ڵ���</param>
		public float this[int i,int j]
		{
			get { return Array[i][j]; }
            set { Array[i][j] = value; }
		}

		/// <summary>��û��滻�˾����һ�е�����</summary>
		/// <param name="row">Ҫ��û����滻���к�</param>
		/// <returns>�˾���ĵ�row��ԭ����</returns>
		public float[] this[int row]
		{
			get { return Array[row]; }
			set
			{
				if (value.Length == Columns)
					Array[row] = value;
				else throw new ArgumentException("Ҫ�滻�������鳤��������Ȳ�ͬ.");
			}
		}
		#region Public Methods
		/// <summary>��ô˾����һ��</summary>
		/// <param name="column">Ҫ��õ��к�</param>
		/// <returns>�����д����һ������</returns>
		public float[] GetColumn(int column)
		{
			float[] c = new float[Array.Count];
			for (int i = 0; i < Array.Count; i++)
				c[i] = Array[i][column];
			return c;
		}

		/// <summary>���ƴ˾���.</summary>
		/// <returns>���ƺ�ľ���</returns>
		public Matrix Copy()
		{
            Matrix X = new Matrix
            {
                Array = new List<float[]>(Array)
            };
            return X;
		}
		/// <summary>���һ������.</summary>
		public void AddRow()
        {
			Array.Add(new float[Columns]);
        }
		
		/// <summary>���һ��.</summary>
		 /// <param name="row"> ��������ĸ��Ƽ������.</param>
		public void AddRow(float[] row)
        {
			if (row.Length != Columns)
				throw new ArgumentException("Ҫ��ӵ������鳤��������Ȳ�ͬ.");
			else
			{
				float[] copy = new float[row.Length];
				row.CopyTo(copy, 0);
				Array.Add(copy);
			}
        }
		/// <summary>���һ��.</summary>
		/// <param name="row"> ��������������.
		/// </param>
		public void AddRowNoCopy(float[] row)
		{
			if (row.Length != Columns)
				throw new ArgumentException("Ҫ��ӵ������鳤��������Ȳ�ͬ.");
			else Array.Add(row);
		}
		/*
		public virtual Matrix GetMatrix(int i0, int i1, int j0, int j1)
		{
			Matrix X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
			List<float[]> B = X.Array;
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						B[i - i0][j - j0] = Array[i][j];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		public virtual Matrix GetMatrix(int[] r, int[] c)
		{
			Matrix X = new Matrix(r.Length, c.Length);
			List<float[]> B = X.Array;
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						B[i][j] = Array[r[i]][c[j]];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Get a submatrix.</summary>
		/// <param name="i0">  Initial row index
		/// </param>
		/// <param name="i1">  Final row index
		/// </param>
		/// <param name="c">   Array of column indices.
		/// </param>
		/// <returns>     A(i0:i1,c(:))
		/// </returns>
		/// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
		/// </exception>
		
		public virtual Matrix GetMatrix(int i0, int i1, int[] c)
		{
			Matrix X = new Matrix(i1 - i0 + 1, c.Length);
			List<float[]> B = X.Array;
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						B[i - i0][j] = Array[i][c[j]];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Get a submatrix.</summary>
		/// <param name="r">   Array of row indices.
		/// </param>
		/// <param name="j0">  Initial column index
		/// </param>
		/// <param name="j1">  Final column index
		/// </param>
		/// <returns>     A(r(:),j0:j1)
		/// </returns>
		/// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
		/// </exception>
		
		public virtual Matrix GetMatrix(int[] r, int j0, int j1)
		{
			Matrix X = new Matrix(r.Length, j1 - j0 + 1);
			List<float[]> B = X.Array;
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						B[i][j - j0] = Array[r[i]][j];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Set a single element.</summary>
		/// <param name="i">   Row index.
		/// </param>
		/// <param name="j">   Column index.
		/// </param>
		/// <param name="s">   A(i,j).
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException">  
		/// </exception>
		
		/// <summary>Set a submatrix.</summary>
		/// <param name="i0">  Initial row index
		/// </param>
		/// <param name="i1">  Final row index
		/// </param>
		/// <param name="j0">  Initial column index
		/// </param>
		/// <param name="j1">  Final column index
		/// </param>
		/// <param name="X">   A(i0:i1,j0:j1)
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
		/// </exception>
		
		public virtual void  SetMatrix(int i0, int i1, int j0, int j1, Matrix X)
		{
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						Array[i][j] = X.GetElement(i - i0, j - j0);
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		
		/// <summary>Set a submatrix.</summary>
		/// <param name="r">   Array of row indices.
		/// </param>
		/// <param name="c">   Array of column indices.
		/// </param>
		/// <param name="X">   A(r(:),c(:))
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
		/// </exception>
		
		public virtual void  SetMatrix(int[] r, int[] c, Matrix X)
		{
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						Array[r[i]][c[j]] = X.GetElement(i, j);
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		
		/// <summary>Set a submatrix.</summary>
		/// <param name="r">   Array of row indices.
		/// </param>
		/// <param name="j0">  Initial column index
		/// </param>
		/// <param name="j1">  Final column index
		/// </param>
		/// <param name="X">   A(r(:),j0:j1)
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
		/// </exception>
		
		public virtual void  SetMatrix(int[] r, int j0, int j1, Matrix X)
		{
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						Array[r[i]][j] = X.GetElement(i, j - j0);
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		
		/// <summary>Set a submatrix.</summary>
		/// <param name="i0">  Initial row index
		/// </param>
		/// <param name="i1">  Final row index
		/// </param>
		/// <param name="c">   Array of column indices.
		/// </param>
		/// <param name="X">   A(i0:i1,c(:))
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
		/// </exception>
		
		public virtual void  SetMatrix(int i0, int i1, int[] c, Matrix X)
		{
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						Array[i][c[j]] = X.GetElement(i - i0, j);
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		*/

		/// <summary>����ת��.���ڴ˾������ȹ̶�,����ת�ú�ľ�����ֻ���½�,���ṩת�ñ���ķ���.</summary>
		/// <returns>�½���ת�ú�ľ���</returns>
		public Matrix Transpose()
		{
			Matrix X = new Matrix(Columns);
			Parallel.For(0,Columns, i=>
			{
				X.AddRowNoCopy(GetColumn(i));
			});
			return X;
		}

		/// <summary>One norm</summary>
		/// <returns>     Ԫ��֮������һ��
		/// </returns>
		public virtual float Norm1()
		{
			float f = 0;
			for (int j = 0; j < Columns; j++)
			{
				float s = 0;
				for (int i = 0; i < Rows; i++)
				{
					s += Math.Abs(Array[i][j]);
				}
				f = Math.Max(f, s);
			}
			return f;
		}
		
		/// <summary>Two norm</summary>
		/// <returns>    maximum singular value.
		/// </returns>
		/*
		public virtual float Norm2()
		{
			return (new SingularValueDecomposition(this).Norm2());
		}
		*/
		/// <summary>Infinity norm</summary>
		/// <returns>    ������һ��.
		/// </returns>
		
		public virtual float NormInf()
		{
			float f = 0;
			for (int i = 0; i < Rows; i++)
			{
				float s = 0;
				for (int j = 0; j < Columns; j++)
				{
					s += Math.Abs(Array[i][j]);
				}
				f = Math.Max(f, s);
			}
			return f;
		}
		
		/// <summary>Frobenius norm</summary>
		/// <returns>    sqrt of sum of squares of all elements.
		/// </returns>
		
		public virtual float NormF()
		{
			float f = 0;
			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					f = Maths.Hypot(f, Array[i][j]);
				}
			}
			return f;
		}

		/// <summary>����һ���½���-A����</summary>
		/// <returns>-A </returns>
		public static Matrix operator -(Matrix A)
		{
			Matrix X = new Matrix(A);
			Parallel.ForEach(X.Array, x => { for (int i = 0; i < x.Length; i++) x[i] = -x[i]; });
			return X;
		}

		/// <summary>������ÿ��Ԫ��ȡ��</summary>
		/// <returns>-A</returns>
		public virtual Matrix UnaryMinus()
		{
			Parallel.ForEach(Array,x=> { for(int i =0;i<x.Length;i++) x[i] = -x[i]; });
			return this;
        }
		
		/// <returns>ÿ��Ԫ�ض�Ӧ��ӵ��½�����</returns>
		public static Matrix operator +(Matrix A, Matrix B)
		{
			CheckMatrixDimensions(A, B);
			Matrix result = new Matrix(A);
			Parallel.For(0, A.Columns, i =>
			{
				for (int j = 0; j < A.Rows; j++) result[i, j] += B[i, j];
			});
			return result;
		}
		/// <summary>�˾���ÿһ��Ԫ������һ���������</summary>
		/// <param name="B">��һ������</param>
		/// <returns>��Ӻ�Ĵ˾���</returns>
		public virtual Matrix Add(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, i => {
				for (int j = 0; j < Rows; j++) this[i, j] += B[i, j];
			});
			return this;
		}

		/// <returns>ÿ��Ԫ�ض�Ӧ������½�����</returns>
		public static Matrix operator -(Matrix A, Matrix B)
		{
			CheckMatrixDimensions(A, B);
			Matrix result = new Matrix(A);
			Parallel.For(0, A.Columns, i =>
			{
				for (int j = 0; j < A.Rows; j++) result[i, j] -= B[i, j];
			});
			return result;
		}

		/// <summary>�˾���ÿһ��Ԫ�ؼ�ȥ��һ�������Ӧλ��Ԫ��</summary>
		/// <param name="B">��һ������</param>
		/// <returns>�����Ĵ˾���</returns>
		public virtual Matrix Subtract(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, i => {
				for (int j = 0; j < Rows; j++) this[i, j] += B[i, j];
			});
			return this;
		}

		/// <summary>��Ԫ�����,C = A.*B</summary>
		/// <param name="B">��һ������</param>
		/// <returns>A.*B</returns>
		public virtual Matrix ArrayMultiply(Matrix B)
		{
			CheckMatrixDimensions(B);
			Matrix C = new Matrix(this);
			Parallel.For(0,Columns,j=> { for (int i = 0; i < Rows; i++) C[i][j] *= B[i][j]; });
			return C;
		}

		/// <summary>�˾���ÿ��Ԫ������һ�����ӦԪ�����,A = A.*B</summary>
		/// <param name="B">��һ������</param>
		/// <returns>A.*B</returns>
		public virtual Matrix ArrayMultiplyEquals(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, j => { for (int i = 0; i < Rows; i++) Array[i][j] *= B[i][j]; });
			return this;
		}

		/// <summary>��Ԫ���ҳ�, C = A./B</summary>
		/// <param name="B">��һ������</param>
		/// <returns>A./B</returns>
		public virtual Matrix ArrayRightDivide(Matrix B)
		{
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(this);
			Parallel.For(0, Columns, j =>
			{
				for (int i = 0; i < Rows; i++) X[i][j] = Array[i][j] / B.Array[i][j];
			});
			return X;
		}

		/// <summary>�˾�����Ԫ���ҳ���һ������ A = A./B</summary>
		/// <param name="B">��һ������</param>
		/// <returns>�ҳ���Ĵ˾���A./B</returns>
		public virtual Matrix ArrayRightDivideEquals(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, j =>
			{
				for (int i = 0; i < Rows; i++)
				{
					Array[i][j] /= B[i][j];
				}
			});
            return this;
        }

        /// <summary>�½�������Ԫ�����, C = A.\B</summary>
        /// <param name="B">��һ������</param>
        /// <returns>�½���ľ���A.\B</returns>
        public virtual Matrix ArrayLeftDivide(Matrix B)
		{
			CheckMatrixDimensions(B);
			Matrix X = new Matrix(this);
			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					X[i][j] = B[i][j] / Array[i][j];
				}
			}
			return X;
		}

		/// <summary>�˾�����Ԫ�������һ������, A = A.\B</summary>
		/// <param name="B">��һ������</param>
		/// <returns>A.\B</returns>
		public virtual Matrix ArrayLeftDivideEquals(Matrix B)
		{
			CheckMatrixDimensions(B);
			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					Array[i][j] = B.Array[i][j] / Array[i][j];
				}
			}
			return this;
		}

		/// <summary> ������������,C = s*A</summary>
		/// <param name="s">����</param>
		/// <returns>�½�����,ֵΪA*s</returns>
		public static Matrix operator *(Matrix A, float s)
		{
			Matrix X = new Matrix(A);
			Parallel.For(0, X.Rows, i =>
			 {
				 for (int j = 0; j < X.Columns; j++)
				 {
					 X[i][j] *= s;
				 }
			 });
			return X;
		}
		
		/// <summary>������ÿ��Ԫ�س���һ������,A = s*A</summary>
		/// <param name="s">����</param>
		/// <returns>��˺�ı�����</returns>
		public Matrix Multiply(float s)
		{
			Parallel.For(0,Rows, i=>
			{
				for (int j = 0; j < Columns; j++)
				{
					Array[i][j] = s * Array[i][j];
				}
			});
			return this;
		}

		/// <summary>���Դ�������˷�,C=A * B</summary>
		/// <param name="B">��һ������</param>
		/// <returns>�½�����,������Ļ�A * B</returns>
		/// <exception cref="ArgumentException">A�����������B���������������ͬ.</exception>
		public static Matrix operator *(Matrix A, Matrix B)
		{
			CheckMatrixDimensionsM(A, B);
			Matrix X = new Matrix(A.Rows);
			for (int i = 0; i < A.Rows; i++)
				X.Array.Add(new float[B.Columns]);
			float[] Bcolj = new float[A.Columns];
			Parallel.For(0, B.Columns, j =>
			{
				for (int k = 0; k < A.Columns; k++)
				{
					Bcolj[k] = B[k][j];
				}
				for (int i = 0; i < A.Rows; i++)
				{
					float s = 0;
					for (int k = 0; k < A.Columns; k++)
					{
						s += A[i][k] * Bcolj[k];
					}
					X[i][j] = s;
				}
			});
			return X;
		}

		/// <summary>����ļ�.</summary>
		/// <returns>�Խ���Ԫ��֮��.</returns>
		public float Trace()
		{
			float t = 0;
			Parallel.For(0, Math.Min(Rows, Columns), i =>
			  {
				  t += Array[i][i];
			  });
			return t;
		}

		/// <summary>���������Ԫ����ɵ�һ������</summary>
		/// <param name="rows">����.</param>
		/// <param name="colums">����.</param>
		/// <returns>һ��rows��colums�еľ��ȷֲ��ľ���.</returns>
		public static Matrix Random(int rows, int colums)
		{
			Matrix A = new Matrix(rows, new float[colums]);
			for (int i = 1; i < rows; i++) A.AddRow();
			Parallel.ForEach(A.Array, row =>{
				   Parallel.ForEach(row, x =>{
					   x = (float)((214903917 * DateTime.Now.Millisecond) & ((1 << 48) - 1)) / 12345;
				   });
			});
			return A;
		}

		/// <summary>����һ����λ����.</summary>
		/// <param name="dim">��λ����Ŀ����.</param>
		/// <returns>���ϵ�����Ϊ1����ȫ��Ϊ0�ķ���.</returns>
		public static Matrix Identity(int dim)
		{
			Matrix A = new Matrix(dim);
			for (int i = 0; i < dim; i++)
				A.AddRowNoCopy(new float[dim]);
			Parallel.For(0, dim, i =>
			{
				A[i][i] = 1;
			});
			return A;
		}
		#endregion //Public Methods

		/// <summary>���������Ŀ����߶��Ƿ���ͬ.</summary>
		private static void CheckMatrixDimensions(Matrix A,Matrix B)
		{
			if (B.Rows !=A.Rows || B.Columns != A.Columns)
				throw new ArgumentException("����������������������ͬ.");
		}
		/// <summary>���B��������߶�����˾������ͬ.</summary>
		private void  CheckMatrixDimensions(Matrix B)
		{
			if (B.Rows != Rows || B.Columns != Columns)
				throw new ArgumentException("�˾�����B��������������������ͬ.");
		}
		private static void CheckMatrixDimensionsM(Matrix A, Matrix B)
		{
			if (B.Rows != A.Columns)
				throw new ArgumentException("A�����������B���������������ͬ.");
		}
	}
}