using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Matrices
{
    /*
     * Class to contain all of the functions used in Cognitive Algebra Computations
     * for matrices. The rules of Cognitive Algebra are specified in the
     * instructions sheet
     */

    public sealed class CognitiveAlgebra
    {
        // Constants
        public const double NEGATIVE_RELATIONSHIP = -1;
        public const double NO_RELATIONSHIP = 0;
        public const double POSITIVE_RELATIONSHIP = 1;
        public const double NON_NEGATIVE_RELATIONSHIP = 5;
        public const double NON_POSITIVE_RELATIONSHIP = 6;
        public const double NON_ZERO_RELATIONSHIP = 7;
        public const double UNIVERSAL = 8;
        

        public static Matrix Add(Matrix lhs, Matrix rhs)
        {
            Matrix result = new Matrix(lhs);
            result.Clear();
            for (int row = 0; row < lhs.Rows; row++)
            {
                for (int col = 0; col < lhs.Cols; col++)
                {
                    result[row, col] = CognitiveAdditionLogic(lhs[row, col], rhs[row, col]);
                }
            }
            return result;
        }

        public static Matrix Multiply(Matrix lhs, Matrix rhs)
        {
            Matrix result = new Matrix(lhs);
            result.Clear();
            for (int i = 0; i < lhs.Rows; i++)
            {
                for (int j = 0; j < rhs.Cols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < lhs.Rows; k++)
                    {
                        double product = CognitiveMultiplicationLogic(lhs[i, k], rhs[k, j]);
                        sum = CognitiveAdditionLogic(sum, product);
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }

        private static double CognitiveAdditionLogic(double lhs, double rhs)
        {
            // Comparison of NEGATIVE_RELATIONSHIP
            if (lhs == NEGATIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NEGATIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NEGATIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NO_RELATIONSHIP
            else if (lhs == NO_RELATIONSHIP) return rhs;

            // Comparison of POSITIVE_RELATIONSHIP
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return POSITIVE_RELATIONSHIP;
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return POSITIVE_RELATIONSHIP;
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == POSITIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NON_NEGATIVE_RELATIONSHIP
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NON_POSITIVE_RELATIONSHIP
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NON_ZERO_RELATIONSHIP
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of UNIVERSAL
            else if (lhs == UNIVERSAL) return UNIVERSAL;

            else return -2.0; // need something to return to prevent compiler from complaining
        }

        private static double CognitiveMultiplicationLogic(double lhs, double rhs)
        {
            // Comparison of NEGATIVE_RELATIONSHIP
            if (lhs == NEGATIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return POSITIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NO_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NEGATIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NEGATIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NO_RELATIONSHIP
            else if (lhs == NO_RELATIONSHIP) return NO_RELATIONSHIP;

            // Comparison of POSITIVE_RELATIONSHIP
            else if (lhs == POSITIVE_RELATIONSHIP) return rhs;

            // Comparison of NON_NEGATIVE_RELATIONSHIP
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NO_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_NEGATIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NON_POSITIVE_RELATIONSHIP
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NO_RELATIONSHIP;
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return NON_POSITIVE_RELATIONSHIP;
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return NON_NEGATIVE_RELATIONSHIP;
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_POSITIVE_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of NON_ZERO_RELATIONSHIP
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NEGATIVE_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NO_RELATIONSHIP) return NO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == POSITIVE_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NON_NEGATIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NON_POSITIVE_RELATIONSHIP) return UNIVERSAL;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == NON_ZERO_RELATIONSHIP) return NON_ZERO_RELATIONSHIP;
            else if (lhs == NON_ZERO_RELATIONSHIP && rhs == UNIVERSAL) return UNIVERSAL;

            // Comparison of UNIVERSAL
            else if (lhs == UNIVERSAL && rhs == NO_RELATIONSHIP) return NO_RELATIONSHIP;
            else if (lhs == UNIVERSAL) return UNIVERSAL;

            else return -2.0; // need something to return to prevent compiler from complaining
        }
    }
}
