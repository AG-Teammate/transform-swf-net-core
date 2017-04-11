//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class includes methods to convert Java rectangular arrays (jagged arrays
//	with inner arrays of the same length).
//----------------------------------------------------------------------------------------
internal static class RectangularArrays
{
    internal static float[][] ReturnRectangularFloatArray(int size1, int size2)
    {
        float[][] newArray = new float[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new float[size2];
        }

        return newArray;
    }
}