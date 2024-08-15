namespace SisyphusLab.Matrices
{
    public struct Matrix2x2Int
    {
        public int m00, m01;
        public int m10, m11;
    
        
        public Matrix2x2Int(int m00, int m01, int m10, int m11)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m10 = m10;
            this.m11 = m11;
        }

        
        public Matrix2x2Int Multiply(Matrix2x2Int other)
        {
            return new Matrix2x2Int(
                // Row 0, Column 0
                (this.m00 * other.m00) + (this.m01 * other.m10),
                
                // Row 0, Column 1
                (this.m00 * other.m01) + (this.m01 * other.m11),

                // Row 1, Column 0
                (this.m10 * other.m00) + (this.m11 * other.m10),

                // Row 1, Column 1
                (this.m10 * other.m01) + (this.m11 * other.m11)
            );
        }
        
    }
}