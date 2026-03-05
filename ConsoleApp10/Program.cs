using System;

namespace MatrixCalculatorApp {

  class MatrixSizeMismatchException : Exception {
    public MatrixSizeMismatchException() : base("Matrix sizes must match.") { }
  }

  class MatrixNotInvertibleException : Exception {
    public MatrixNotInvertibleException() : base("Matrix determinant is zero. Matrix is not invertible.") { }
  }

  class SquareMatrix : IComparable<SquareMatrix>, ICloneable {

    private double[,] _matrix;
    public int Size { get; }
    private static readonly Random _random = new Random();

    public SquareMatrix(int size) {
      if (size <= 0) { 
        throw new ArgumentException("Size must be positive.");
      }

      Size = size;
      _matrix = new double[size, size];
    }

    public SquareMatrix(int size, int minValue, int maxValue) : this(size) { 
      for (int rowIndex = 0; rowIndex < size; ++rowIndex) { 
        for (int columnIndex = 0; columnIndex < size; ++columnIndex) { 
          _matrix[rowIndex, columnIndex] = _random.Next(minValue, maxValue);
        }
      }
    }

    public double this[int rowIndex, int columnIndex] {
      get { 
        return _matrix[rowIndex, columnIndex]; 
      }
      set { 
        _matrix[rowIndex, columnIndex] = value; 
      }
    }

    public override string ToString() {
      string output = "";

      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) { 
          output += _matrix[rowIndex, columnIndex].ToString("0.##") + "\t";
        }
        output += Environment.NewLine;
      }

      return output;
    }

    public double Determinant() {
      return CalculateDeterminant(_matrix);
    }

    private double CalculateDeterminant(double[,] sourceMatrix) {
      int matrixSize;
      matrixSize = sourceMatrix.GetLength(0);

      if (matrixSize == 1) { 
        return sourceMatrix[0, 0];
      }

      if (matrixSize == 2) { 
        return sourceMatrix[0, 0] * sourceMatrix[1, 1] - sourceMatrix[0, 1] * sourceMatrix[1, 0];
      }

      double determinantValue = 0.0;

      for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
        double[,] minorMatrix;
        minorMatrix = GetMinorMatrix(sourceMatrix, 0, columnIndex);

        determinantValue += Math.Pow(-1, columnIndex) * sourceMatrix[0, columnIndex] * CalculateDeterminant(minorMatrix);
      }

      return determinantValue;
    }

    private double[,] GetMinorMatrix(double[,] sourceMatrix, int excludedRowIndex, int excludedColumnIndex) {

      int matrixSize;
      double[,] minorMatrix;

      matrixSize = sourceMatrix.GetLength(0);
      minorMatrix = new double[matrixSize - 1, matrixSize - 1];

      int minorRowIndex = 0;

      for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex) {

        if (rowIndex == excludedRowIndex) { 
          continue;
        }

        int minorColumnIndex = 0;

        for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {

          if (columnIndex == excludedColumnIndex) { 
            continue;
          }

          minorMatrix[minorRowIndex, minorColumnIndex] = sourceMatrix[rowIndex, columnIndex];

          ++minorColumnIndex;
        }

        ++minorRowIndex;
      }

      return minorMatrix;
    }

    public SquareMatrix Inverse() {

      double determinantValue;
      determinantValue = Determinant();

      if (determinantValue == 0) { 
        throw new MatrixNotInvertibleException();
      }

      int matrixSize;
      matrixSize = Size;

      SquareMatrix inverseMatrix;
      inverseMatrix = new SquareMatrix(matrixSize);

      for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {

          double[,] minorMatrix;
          double cofactorValue;

          minorMatrix = GetMinorMatrix(_matrix, rowIndex, columnIndex);
          cofactorValue = Math.Pow(-1, rowIndex + columnIndex) * CalculateDeterminant(minorMatrix);


          inverseMatrix[columnIndex, rowIndex] = cofactorValue / determinantValue;
        }
      }

      return inverseMatrix;
    }

    public static SquareMatrix operator + (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      if (leftMatrix.Size != rightMatrix.Size) { 
        throw new MatrixSizeMismatchException();
      }

      SquareMatrix resultMatrix;
      resultMatrix = new SquareMatrix(leftMatrix.Size);

      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex) {

          resultMatrix[rowIndex, columnIndex] = leftMatrix[rowIndex, columnIndex] + rightMatrix[rowIndex, columnIndex];
        }
      }

      return resultMatrix;
    }

    public static SquareMatrix operator * (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      if (leftMatrix.Size != rightMatrix.Size) { 
        throw new MatrixSizeMismatchException();
      }

      SquareMatrix resultMatrix;
      resultMatrix = new SquareMatrix(leftMatrix.Size);

      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex) {
          for (int multiplierIndex = 0; multiplierIndex < leftMatrix.Size; ++multiplierIndex) {

            resultMatrix[rowIndex, columnIndex] += leftMatrix[rowIndex, multiplierIndex] * rightMatrix[multiplierIndex, columnIndex];
          }
        }
      }

      return resultMatrix;
    }

    public static bool operator > (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      return leftMatrix.Determinant() > rightMatrix.Determinant();
    }

    public static bool operator < (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      return leftMatrix.Determinant() < rightMatrix.Determinant();
    }

    public static bool operator >= (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      return leftMatrix.Determinant() >= rightMatrix.Determinant();
    }

    public static bool operator <= (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      return leftMatrix.Determinant() <= rightMatrix.Determinant();
    }

    public static bool operator == (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      return leftMatrix.Equals(rightMatrix);
    }

    public static bool operator != (SquareMatrix leftMatrix, SquareMatrix rightMatrix) {

      return !leftMatrix.Equals(rightMatrix);
    }

    public static implicit operator SquareMatrix(double value) {

      SquareMatrix matrix;
      matrix = new SquareMatrix(1);

      matrix[0, 0] = value;

      return matrix;
    }

    public static bool operator true(SquareMatrix matrix) {
      return matrix.Determinant() != 0;
    }

    public static bool operator false(SquareMatrix matrix) {
      return matrix.Determinant() == 0;
    }

    public override bool Equals(object obj) {
       if (obj == null) {
        return false;
      }
      SquareMatrix otherMatrix = obj as SquareMatrix;
      if (otherMatrix == null || Size != otherMatrix.Size) { 
        return false;
      }

      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {
          if (_matrix[rowIndex, columnIndex] != otherMatrix[rowIndex, columnIndex]) { 
            return false;
          }
        }
      }

      return true;
    }

    public override int GetHashCode() {
      return Determinant().GetHashCode();
    }

    public int CompareTo(SquareMatrix otherMatrix) {
      return Determinant().CompareTo(otherMatrix.Determinant());
    }

    public object Clone() {

      SquareMatrix copiedMatrix;
      copiedMatrix = new SquareMatrix(Size);

      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex) {

          copiedMatrix[rowIndex, columnIndex] = _matrix[rowIndex, columnIndex];
        }
      }

      return copiedMatrix;
    }
  }

  class Program {

    static void Main() {

      try {

        SquareMatrix firstMatrix;
        SquareMatrix secondMatrix;

        firstMatrix = new SquareMatrix(3, -5, 10);
        secondMatrix = new SquareMatrix(3, -5, 10);

        Console.WriteLine($"First matrix: \n{firstMatrix}");

        Console.WriteLine($"Second matrix: \n{secondMatrix}");

        Console.WriteLine($"Sum: \n{firstMatrix + secondMatrix}");

        Console.WriteLine($"Product: \n{firstMatrix * secondMatrix}");

        Console.WriteLine($"Determinant: \n{firstMatrix.Determinant()}");

        if (firstMatrix >= secondMatrix) { 
          Console.WriteLine("First matrix determinant is greater or equal");
        }

        double determinantValue;
        determinantValue = firstMatrix.Determinant();

        Console.WriteLine("Determinant via cast: " + determinantValue);
      } catch (Exception exception) {
        Console.WriteLine("Error: " + exception.Message);
      }

      Console.ReadLine();
    }
  }
}