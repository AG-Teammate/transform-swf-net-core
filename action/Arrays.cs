namespace System
{
    internal class Arrays
    {
        public static T[] copyOf<T>(T[] sourceArr, int num)
        {
            T[] result = new T[num];
            Array.Copy(sourceArr, result, num);
            return result;
        }

        internal static string ToString<T>(T[] v)
        {
            return "[" + string.Join(",", v) + "]";
        }

        internal static int GetHashCode(float[] matrix)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        internal static bool deepEquals(float[][] matrix1, float[][] matrix2)
        {
            if (matrix1.Length != matrix2.Length) return false;
            for (var i = 0; i < matrix1.Length; i++)
            {
                if (matrix1[i].Length != matrix2[i].Length) return false;
                for (var j = 0; j < matrix1[i].Length; i++)
                {
                    if (matrix1[i][j] != matrix2[i][j]) return false;
                }
            }

            return true;
        }

        public static int deepHashCode(float[][] matrix)
        {
            unchecked
            {
                if (matrix == null)
                {
                    return 0;
                }
                int hash = 17;
                for (var i = 0; i < matrix.Length; i++)
                {
                    for (var j = 0; j < matrix[i].Length; i++)
                    {
                        hash = hash * 31 + matrix[i][j].GetHashCode();
                    }
                }
                return hash;
            }
        }
    }
}