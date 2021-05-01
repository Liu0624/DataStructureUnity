using System;

namespace WJF_CodeLibrary.CommonUtility.Sort
{
    public static class SortUtility
    {
        /// <summary>
        /// 冒泡排序（时间复杂度：O(n^2)， 稳定性：稳定）
        /// </summary>
        /// <typeparam name="T">集合的元素类型</typeparam>
        /// <param name="t">需要排序的集合</param>
        /// <param name="comparison">比较条件（满足条件即交换，由小到大：x > y）</param>
        public static void Bubble<T>(T[] t, Func<T, T, bool> comparison)
        {
            for (int i = 0; i < t.Length; i++)
            {
                for (int j = 0; j < t.Length - i - 1; j++)
                {
                    if (comparison(t[j], t[j + 1]))
                    {
                        T temp = t[j];
                        t[j] = t[j + 1];
                        t[j + 1] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 选择排序（时间复杂度：O(n^2)， 稳定性：不稳定）
        /// </summary>
        /// <typeparam name="T">集合的元素类型</typeparam>
        /// <param name="t">需要排序的集合</param>
        /// <param name="comparison">比较条件</param>
        public static void Selection<T>(T[] t, Func<T, T, bool> comparison)
        {
            int minIndex;

            for (int i = 0; i < t.Length - 1; i++)
            {
                minIndex = i;
                for (int j = i + 1; j < t.Length; j++)
                {
                    if (comparison(t[minIndex], t[j]))
                    {
                        minIndex = j;
                    }
                }

                if (minIndex != i)
                {
                    T temp = t[minIndex];
                    t[minIndex] = t[i];
                    t[i] = temp;
                }                
            }
        }

        /// <summary>
        /// 插入排序（时间复杂度：O(n^2)， 稳定性：稳定）
        /// </summary>
        /// <typeparam name="T">集合的元素类型</typeparam>
        /// <param name="t">需要排序的集合</param>
        /// <param name="comparison">比较条件</param>
        public static void Insertion<T>(T[] t, Func<T, T, bool> comparison)
        {
            for (int i = 1; i < t.Length; i++)
            {
                int preIndex = i - 1;
                T current = t[i];

                while (preIndex >= 0 && comparison(t[preIndex], current))
                {
                    t[preIndex + 1] = t[preIndex];
                    preIndex--;
                }

                t[preIndex + 1] = current;
            }
        }

        /// <summary>
        /// 希尔排序（时间复杂度：O(n^2)， 稳定性：不稳定）
        /// </summary>
        /// <typeparam name="T">集合的元素类型</typeparam>
        /// <param name="t">需要排序的集合</param>
        /// <param name="comparison">比较条件</param>
        public static void Shell<T>(T[] t, Func<T, T, bool> comparison)
        {
            for (int gap = t.Length / 2; gap >= 1; gap = gap / 2)
            {
                for (int i = 0; i < t.Length - gap; i++)
                {
                    for (int j = 0; j < t.Length - gap; j += gap)
                    {
                        if (comparison(t[j], t[j + gap]))
                        {
                            T temp = t[j];
                            t[j] = t[j + gap];
                            t[j + gap] = temp;
                        }
                    }
                }
            }
        }

        #region 归并排序

        /// <summary>
        /// 归并排序（时间复杂度：O(nlog2n)， 稳定性：稳定）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static T[] Merge<T>(T[] t, Func<T, T, bool> comparison)
        {
            if (t.Length < 2)
                return t;

            int mid = t.Length / 2;

            T[] left = new T[mid];
            T[] right = new T[t.Length - mid];

            for (int i = 0; i < mid; i++)
                left[i] = t[i];

            for (int i = mid; i < t.Length; i++)
                right[i - mid] = t[i];

            return MergeSort(Merge(left, comparison), Merge(right, comparison), comparison);
        }

        /// <summary>
        /// 序列合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        private static T[] MergeSort<T>(T[] left, T[] right, Func<T, T, bool> comparison)
        {
            T[] result = new T[left.Length + right.Length];

            int i = 0, lIndex = 0, rIndex = 0;

            while (lIndex < left.Length && rIndex < right.Length)
            {
                if (comparison(left[lIndex], right[rIndex]))
                {
                    result[i++] = left[lIndex++];
                }
                else
                {
                    result[i++] = right[rIndex++];
                }
            }

            while (lIndex < left.Length)
                result[i++] = left[lIndex++];

            while (rIndex < right.Length)
                result[i++] = right[rIndex++];

            return result;
        }

        #endregion
    }
}