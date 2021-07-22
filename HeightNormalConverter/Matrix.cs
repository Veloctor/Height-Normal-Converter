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
	///<summary>矩阵特性:宽度固定(数组),高度可变(List容器),运算符并行计算.
	///重载的运算符返回的是新建矩阵,不影响原矩阵的值,而方法运算*大都*改变原矩阵的值并返回</summary>
	public class Matrix
	{
		/// <summary>如需直接操作此矩阵数据务必注意每行数组长度相同!!!</summary>
		public List<float[]> Array;

		/// <summary>矩阵的行数</summary>
		public int Rows { get { return Array.Count; } }

		/// <summary>矩阵的列数</summary>
		public int Columns { get { return Array[0].Length; } }

		#region constructors
		/// <summary>新建一个矩阵</summary>
		public Matrix()
		{
			Array = new List<float[]>();
		}
		/// <summary>新建一个矩阵.构造时确定矩阵行数可降低List动态扩容开销</summary>
		/// <param name="rows">矩阵初始行数(List<float[]>的初始长度).</param>
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
		/// <summary>新建一个矩阵</summary>
		/// <param name="from">新建的矩阵是此矩阵的复制.</param>
		public Matrix(Matrix from)
		{
			Array = new List<float[]>(from.Array);
		}
		/// <summary>新建一个矩阵</summary>
		/// <param name="FirstRow">
		/// 元素的第一行数组,此矩阵的宽度将被固定为此数组的长度
		/// </param>
		public Matrix(float[] FirstRow)
		{
			Array = new List<float[]>();
			float[] copy = new float[FirstRow.Length];
			FirstRow.CopyTo(copy, 0);
			Array.Add(copy);
		}
		#endregion
		/// <summary>获得或设置此矩阵的某个元素</summary>
		/// <param name="i">元素所在的行</param>
		/// <param name="j">元素所在的列</param>
		public float this[int i,int j]
		{
			get { return Array[i][j]; }
            set { Array[i][j] = value; }
		}

		/// <summary>获得或替换此矩阵的一行的数组</summary>
		/// <param name="row">要获得或者替换的行号</param>
		/// <returns>此矩阵的第row行原数组</returns>
		public float[] this[int row]
		{
			get { return Array[row]; }
			set
			{
				if (value.Length == Columns)
					Array[row] = value;
				else throw new ArgumentException("要替换的行数组长度与矩阵宽度不同.");
			}
		}
		#region Public Methods
		/// <summary>获得此矩阵的一列</summary>
		/// <param name="column">要获得的列号</param>
		/// <returns>将此列存入的一个数组</returns>
		public float[] GetColumn(int column)
		{
			float[] c = new float[Array.Count];
			for (int i = 0; i < Array.Count; i++)
				c[i] = Array[i][column];
			return c;
		}

		/// <summary>复制此矩阵.</summary>
		/// <returns>复制后的矩阵</returns>
		public Matrix Copy()
		{
            Matrix X = new Matrix
            {
                Array = new List<float[]>(Array)
            };
            return X;
		}
		/// <summary>添加一个空行.</summary>
		public void AddRow()
        {
			Array.Add(new float[Columns]);
        }
		
		/// <summary>添加一行.</summary>
		 /// <param name="row"> 将此数组的复制加入矩阵.</param>
		public void AddRow(float[] row)
        {
			if (row.Length != Columns)
				throw new ArgumentException("要添加的行数组长度与矩阵宽度不同.");
			else
			{
				float[] copy = new float[row.Length];
				row.CopyTo(copy, 0);
				Array.Add(copy);
			}
        }
		/// <summary>添加一行.</summary>
		/// <param name="row"> 将此数组加入矩阵.
		/// </param>
		public void AddRowNoCopy(float[] row)
		{
			if (row.Length != Columns)
				throw new ArgumentException("要添加的行数组长度与矩阵宽度不同.");
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

		/// <summary>矩阵转置.由于此矩阵类宽度固定,所以转置后的矩阵类只能新建,不提供转置本身的方法.</summary>
		/// <returns>新建的转置后的矩阵</returns>
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
		/// <returns>     元素之和最大的一列
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
		/// <returns>    和最大的一行.
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

		/// <summary>返回一个新建的-A矩阵</summary>
		/// <returns>-A </returns>
		public static Matrix operator -(Matrix A)
		{
			Matrix X = new Matrix(A);
			Parallel.ForEach(X.Array, x => { for (int i = 0; i < x.Length; i++) x[i] = -x[i]; });
			return X;
		}

		/// <summary>矩阵中每个元素取负</summary>
		/// <returns>-A</returns>
		public virtual Matrix UnaryMinus()
		{
			Parallel.ForEach(Array,x=> { for(int i =0;i<x.Length;i++) x[i] = -x[i]; });
			return this;
        }
		
		/// <returns>每个元素对应相加的新建矩阵</returns>
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
		/// <summary>此矩阵每一个元素与另一个矩阵相加</summary>
		/// <param name="B">另一个矩阵</param>
		/// <returns>相加后的此矩阵</returns>
		public virtual Matrix Add(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, i => {
				for (int j = 0; j < Rows; j++) this[i, j] += B[i, j];
			});
			return this;
		}

		/// <returns>每个元素对应相减的新建矩阵</returns>
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

		/// <summary>此矩阵每一个元素减去另一个矩阵对应位置元素</summary>
		/// <param name="B">另一个矩阵</param>
		/// <returns>相减后的此矩阵</returns>
		public virtual Matrix Subtract(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, i => {
				for (int j = 0; j < Rows; j++) this[i, j] += B[i, j];
			});
			return this;
		}

		/// <summary>逐元素相乘,C = A.*B</summary>
		/// <param name="B">另一个矩阵</param>
		/// <returns>A.*B</returns>
		public virtual Matrix ArrayMultiply(Matrix B)
		{
			CheckMatrixDimensions(B);
			Matrix C = new Matrix(this);
			Parallel.For(0,Columns,j=> { for (int i = 0; i < Rows; i++) C[i][j] *= B[i][j]; });
			return C;
		}

		/// <summary>此矩阵每个元素与另一矩阵对应元素相乘,A = A.*B</summary>
		/// <param name="B">另一个矩阵</param>
		/// <returns>A.*B</returns>
		public virtual Matrix ArrayMultiplyEquals(Matrix B)
		{
			CheckMatrixDimensions(B);
			Parallel.For(0, Columns, j => { for (int i = 0; i < Rows; i++) Array[i][j] *= B[i][j]; });
			return this;
		}

		/// <summary>逐元素右除, C = A./B</summary>
		/// <param name="B">另一个矩阵</param>
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

		/// <summary>此矩阵逐元素右除另一个矩阵 A = A./B</summary>
		/// <param name="B">另一个矩阵</param>
		/// <returns>右除后的此矩阵A./B</returns>
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

        /// <summary>新建矩阵并逐元素左除, C = A.\B</summary>
        /// <param name="B">另一个矩阵</param>
        /// <returns>新建后的矩阵A.\B</returns>
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

		/// <summary>此矩阵逐元素左除另一个矩阵, A = A.\B</summary>
		/// <param name="B">另一个矩阵</param>
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

		/// <summary> 矩阵与标量相乘,C = s*A</summary>
		/// <param name="s">标量</param>
		/// <returns>新建矩阵,值为A*s</returns>
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
		
		/// <summary>本矩阵每个元素乘以一个标量,A = s*A</summary>
		/// <param name="s">标量</param>
		/// <returns>相乘后的本矩阵</returns>
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

		/// <summary>线性代数矩阵乘法,C=A * B</summary>
		/// <param name="B">另一个矩阵</param>
		/// <returns>新建矩阵,两矩阵的积A * B</returns>
		/// <exception cref="ArgumentException">A矩阵的列数与B矩阵的行数必须相同.</exception>
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

		/// <summary>矩阵的迹.</summary>
		/// <returns>对角线元素之和.</returns>
		public float Trace()
		{
			float t = 0;
			Parallel.For(0, Math.Min(Rows, Columns), i =>
			  {
				  t += Array[i][i];
			  });
			return t;
		}

		/// <summary>生成由随机元素组成的一个矩阵</summary>
		/// <param name="rows">行数.</param>
		/// <param name="colums">列数.</param>
		/// <returns>一个rows行colums列的均匀分布的矩阵.</returns>
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

		/// <summary>生成一个单位矩阵.</summary>
		/// <param name="dim">单位矩阵的宽与高.</param>
		/// <returns>左上到右下为1其余全部为0的方阵.</returns>
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

		/// <summary>检查两矩阵的宽度与高度是否相同.</summary>
		private static void CheckMatrixDimensions(Matrix A,Matrix B)
		{
			if (B.Rows !=A.Rows || B.Columns != A.Columns)
				throw new ArgumentException("两矩阵行数与列数必须相同.");
		}
		/// <summary>检查B矩阵宽度与高度是与此矩阵否相同.</summary>
		private void  CheckMatrixDimensions(Matrix B)
		{
			if (B.Rows != Rows || B.Columns != Columns)
				throw new ArgumentException("此矩阵与B矩阵行数与列数必须相同.");
		}
		private static void CheckMatrixDimensionsM(Matrix A, Matrix B)
		{
			if (B.Rows != A.Columns)
				throw new ArgumentException("A矩阵的列数与B矩阵的行数必须相同.");
		}
	}
}