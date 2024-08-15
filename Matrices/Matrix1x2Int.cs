using UnityEngine;

namespace SisyphusLab.Matrices
{
    public struct Matrix1x2Int
    {
        public int m00, m01;  // The elements of the 1x2 matrix

        // Constructor
        public Matrix1x2Int(int m00, int m01)
        {
            this.m00 = m00;
            this.m01 = m01;
        }

        public Matrix1x2Int(Vector2Int vector)
        {
            this.m00 = vector.x;
            this.m01 = vector.y;
        }

        public Vector2Int Vector
        {
            get => new Vector2Int(m00, m01);
        }
        public Matrix1x2Int Multiply(Matrix2x2Int other)
        {
            return new Matrix1x2Int(
                // Row 0, Column 0 of the result
                (this.m00 * other.m00) + (this.m01 * other.m10),

                // Row 0, Column 1 of the result
                (this.m00 * other.m01) + (this.m01 * other.m11)
            );
        }

    }
}