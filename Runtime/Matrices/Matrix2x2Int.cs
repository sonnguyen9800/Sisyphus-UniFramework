using System;
using UnityEngine;

namespace SisyphusFramework.Matrices
{
    public struct Matrix2x2Int
    {
        public static Matrix2x2Int FlipVerticalMatrix
        {
            get
            {
                return new Matrix2x2Int(1, 0, 0, -1);
            }
        }
        public static Matrix2x2Int FlipHorizontalMatrix
        {
            get
            {
                return new Matrix2x2Int(0, 1, 1, 0);
            }
        }
        
        
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

        public static Matrix2x2Int CreateRotationMatrix(float angleDegrees)
        {
            // Convert the angle to radians
            float angleRadians = angleDegrees * (float)Math.PI / 180f;

            // Calculate the elements of the rotation matrix
            float cosTheta = (float)Mathf.Cos(angleRadians);
            float sinTheta = (float)Mathf.Sin(angleRadians);

            // IMPORTANT: Rounding to int to match Matrix2x2Int structure, may lose precision.
            int m00 = (int)Mathf.Round(cosTheta);
            int m01 = (int)Mathf.Round(-sinTheta);
            int m10 = (int)Mathf.Round(sinTheta);
            int m11 = (int)Mathf.Round(cosTheta);

            // Return the rounded rotation matrix
            return new Matrix2x2Int(m00, m01, m10, m11);
        }
        
        
    }
}