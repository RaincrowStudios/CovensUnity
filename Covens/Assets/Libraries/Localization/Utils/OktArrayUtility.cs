using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Oktagon.Utils
{
    public static class OktArrayUtility
    {


        /// <summary>
        /// Shuffle the array (Fischer-Yates shuffle algorithm).
        /// source: http://www.dotnetperls.com/fisher-yates-shuffle
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public static void Shuffle<T>(T[] array)
        {
            var random = new System.Random();
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                                        // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        /// <summary>
        /// Shuffle the array (Fischer-Yates shuffle algorithm).
        /// source: http://www.dotnetperls.com/fisher-yates-shuffle
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
		public static void Shuffle<T>(List<T> array)
        {
            var random = new System.Random();
            for (int i = array.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                                        // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        /// <summary>
        /// Merge the specified arrays into a single array.
        /// Actually, just appends each other's values into a new array.
        /// </summary>
        /// <param name='arrays'>
        /// The arrays to merge
        /// </param>
        public static T[] Merge<T>(params T[][] arrays)
        {

            if (arrays == null || arrays.Length < 0)
            {
                return null;
            }

            int iSize = 0;
            // get the size of all arrays
            for (int iArray = 0; iArray < arrays.Length; iArray++)
            {
                if (arrays[iArray] != null)
                {
                    iSize += arrays[iArray].Length;
                }
            }

            var merged = new T[iSize];

            int iCount = 0;
            for (int iArray = 0; iArray < arrays.Length; iArray++)
            {
                if (arrays[iArray] != null)
                {
                    arrays[iArray].CopyTo(merged, iCount);
                    iCount += arrays[iArray].Length;
                }
            }

            return merged;

        }

        /// <summary>
        /// Gets a random index's value.
        /// </summary>
		public static T GetRandomValue<T>(List<T> lArray)
        {

			return lArray[UnityEngine.Random.Range(0, lArray.Count)];
        }

        /// <summary>
        /// Gets a random index's value.
        /// </summary>
        public static T GetRandomValue<T>(T[] array)
        {

            return array[UnityEngine.Random.Range(0, array.Length)];
        }
		
        /// <summary>
        /// Gets a random index's value based on a probability array.
        /// Note: the sum of all indices should be 1f. This method does not normalize them.
        /// e.g.:
        /// vArray = {0,1,2}, vProbs = {0.5f, 0.3f, 0.2f}
        /// it should have 
        /// 50% to return 0,
        /// 30% to return 1,
        /// 20% to return 2.
        /// Note: it's the same as only using vProbs = {0.5f, 0.3f}
        /// </summary>
        public static T GetRandomValue<T>(T[] vArray, float[] vProbability)
        {

            // do some obvious checks before doing random:
            if (vArray.Length == 1)
            {
                return vArray[0];
            }

            int iIndex = vArray.Length - 1;

			float fRandom = UnityEngine.Random.value;

            for (int i = 0; i < vProbability.Length; i++)
            {
                fRandom -= vProbability[i];
                if (fRandom < 0f)
                {
                    iIndex = i;
                    break;
                }
            }

            return vArray[iIndex];
        }

        /// <summary>
        /// Gets a random index's value based on a probability array.
        /// Note: the sum of all indices should be 1f. This method does not normalize them.
        /// e.g.:
        /// vArray = {0,1,2}, vProbs = {0.5f, 0.3f, 0.2f}
        /// it should have 
        /// 50% to return 0,
        /// 30% to return 1,
        /// 20% to return 2.
        /// Note: it's the same as only using vProbs = {0.5f, 0.3f}
        /// </summary>
		public static T GetRandomValue<T>(List<T> lArray, List<float> lProbability)
        {

            // do some obvious checks before doing random:
            if (lArray.Count == 1)
            {
                return lArray[0];
            }

			int iIndex = GetRandomIndex(lProbability);

            return lArray[iIndex];
        }

		public static int GetRandomIndex(List<float> lProbability) {

			int iIndex = lProbability.Count - 1;

			float fRandom = UnityEngine.Random.value;

			for (int i = 0; i < lProbability.Count; i++)
			{
				fRandom -= lProbability[i];
				if (fRandom < 0f)
				{
					iIndex = i;
					break;
				}
			}

			return iIndex;
		}


        /// <summary>
        /// Gets the sub array.
        /// Works the same way of a substring of String.
        /// Throws IndexOutOfRangeException when iStartIndex plus iLength indicates a position not within the array's instance
        /// e.g.:
        /// array = {7,9,11,18}
        /// iStartIndex = 1
        /// iLength = 2
        /// return = {9,11}
        /// </summary>
        public static T[] GetSubArray<T>(T[] array, int iStartIndex, int iLength)
        {

            int iMaxSize = Mathf.Clamp(iLength, 0, array.Length);

            if (iMaxSize == array.Length)
            {
                return array;
            }

            T[] output = new T[iMaxSize];

            for (int index = 0; index < iMaxSize; index++)
            {
                output[index] = array[index + iStartIndex];
            }

            return output;
        }

        /// <summary>
        /// Gets the sub array.
        /// Works the same way of a substring of String.
        /// Will get all values until the end of string
        /// e.g.:
        /// array = {7,9,11,18}
        /// iStartIndex = 1
        /// return = {9,11,18}
        /// </summary>
        public static T[] GetSubArray<T>(T[] array, int iStartIndex)
        {
            return GetSubArray(array, iStartIndex, array.Length - iStartIndex);
        }


        /// <summary>
        /// Copies the given array (shallow copy mode, i.e.: reference copy).
        /// </summary>
        public static T[] CopyArray<T>(T[] array)
        {

            if (array == null)
            {
                return null;
            }
            T[] vCopy = new T[array.Length];

            System.Array.Copy(array, vCopy, array.Length);

            return vCopy;

        }


        /// <summary>
        /// Purge the specified array (removes null values).
        /// e.g.: {2, null, 3, 4},  2 -> returns: {2, 3}
        /// e.g.: {2, null, 3, 4},  3 -> returns: {2, 3, 4}
        /// e.g.: {2, null, 3, 4}, -1 -> returns: {2, 3, 4}
        /// </summary>
        /// <param name='array'>
        /// The array to remove the null indices
        /// </param>
        /// <param name='iCount'>
        /// The expected return array size.
        /// You probably will calculate this before. But in case you didn't, we recalculate (slower)
        /// </param>
        public static T[] Purge<T>(T[] array, int iCount = -1)
        {

            if (array == null)
            {
                return null;
            }

            if (iCount == -1)
            {
                iCount = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != null)
                    {
                        iCount++;
                    }
                }
            }

            T[] vOutput = new T[iCount];
            iCount = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    vOutput[iCount++] = array[i];

                    if (iCount == vOutput.Length)
                    {
                        break;
                    }
                }
            }

            return vOutput;
        }



		/// <summary>
		/// this is the same as string.Join() but uses generic.
		/// transforms an array into a single string separated by a divisor
		/// </summary>
		/// <param name="sSeparator">
		/// A <see cref="System.String"/> separator, this value will be between all
		/// values found at the 'values'
		/// </param>
		/// <param name="values">
		/// A <see cref="T[]"/>, the array with values to be parsed to string.
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>, e.g.: if separator is '-' and values are {1,2,3},
		/// the output will be "1-2-3"
		/// </returns>
		public static string Join<T>(string sSeparator, T[] vValues) {

			if (vValues == null) {
				return string.Empty;
			}
			if (vValues.Length == 0) {
				return string.Empty;
			}

			var sOutput = new System.Text.StringBuilder("");
			int iMax = vValues.Length-1;
			object pValue;

			for (int index = 0; index < iMax; index++) {
				pValue = vValues[index];
				if (pValue != null) {
					sOutput.Append(pValue.ToString());
				}
				else {
					sOutput.Append("null");
				}
				sOutput.Append(sSeparator);
			}

			pValue = vValues[iMax];
			if (pValue != null) {
				// append last one without adding separator
				sOutput.Append(pValue.ToString());
			}
			else {
				sOutput.Append("null");
			}

			return sOutput.ToString();
		}


		/// <summary>
		/// Normalizes a probability array, so that the sum of all indices is 1f
		/// </summary>
		public static void NormalizeProbabilityArray(float[] vRates) {

			float fTotalRate = 0;
			for (int i = 0; i < vRates.Length; i++) {
				fTotalRate += vRates[i];
			}
			for (int i = 0; i < vRates.Length; i++) {
				vRates[i] = (vRates[i] / fTotalRate);
			}

		}

		public static void NormalizeProbabilityArray(List<float> lRates) {

			float fTotalRate = 0;
			for (int i = 0; i < lRates.Count; i++) {
				fTotalRate += lRates[i];
			}
			for (int i = 0; i < lRates.Count; i++) {
				lRates[i] = (lRates[i] / fTotalRate);
			}

		}
		
		/// <summary>
		/// Adds an element to the array, will return the new resized array.
		/// Not recommended use, but this is here just in case.
		/// Use List's Add instead.
		/// </summary>
		public static T[] AddArrayElement<T>(T[] vOriginal, T pElement) {
			T[] vReturn;
			if (vOriginal == null) {
				vReturn = new T[]{pElement};
			}
			else {
				
				vReturn = new T[vOriginal.Length +1];
				System.Array.Copy(vOriginal, vReturn, vOriginal.Length);
				vReturn[vOriginal.Length] = pElement;

			}

			return vReturn;
		}

		/// <summary>
		/// Adds an element to the array, will return the new resized array.
		/// Not recommended use, but this is here just in case.
		/// Use List's Add instead.
		/// </summary>
		public static T[] RemoveArrayElement<T>(T[] vOriginal, T pElement) {
			T[] vReturn;

			if (vOriginal == null) {
				vReturn = new T[]{pElement};
			}
			else {
				
				vReturn = new T[vOriginal.Length -1];
				int iIndex = 0;
				bool bFound = false;
				for (int i = 0; i < vOriginal.Length; i++) {
					if (vOriginal[i].Equals(pElement)) {
						bFound = true;
					}
					else {
						vReturn[iIndex++] = vOriginal[i];
					}
				}


				if (!bFound) {
					vReturn = vOriginal;
				}

			}

			return vReturn;
		}


    }

}