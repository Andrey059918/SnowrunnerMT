using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEnumerablePlus {
    static class IEnumerablePlus
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action, Int32 StartShift = 0, Int32 EndShift = 0)
        {
            for (int i = StartShift; i < list.Count() - EndShift; i++)
            {
                action(list.ElementAt(i));
            }
        }
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, Int32> action, Int32 StartShift = 0, Int32 EndShift = 0)
        {
            for (int i = StartShift; i < list.Count() - EndShift; i++)
            {
                action(list.ElementAt(i), i);
            }
        }
        public static IEnumerable<T> Take<T>(this IEnumerable<T> enumerable, Int32 StartShift = 0, Int32 EndShift = 0)
        {
            for (int i = StartShift; i < enumerable.Count() - EndShift; i++)
            {
                yield return enumerable.ElementAt(i);
            }
        }
        public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Boolean TakeCrossElement = false) {
            for (int i = 0; i < enumerable.Count(); i++) {
                if (predicate(enumerable.ElementAt(i)))
                {
                    yield return enumerable.ElementAt(i);
                }
                else {
                    if (TakeCrossElement) {yield return enumerable.ElementAt(i); }
                    break;
                }
            }
        }
        public static IEnumerable<T> EndTakeWhile<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Boolean TakeCrossElement = false)
        {
            Int32 start = 0;
            for (int i = enumerable.Count(); i > 0; i--)
            {
                if (!predicate(enumerable.ElementAt(i - 1)))
                {
                    start = i;
                    break;
                }
            }
            if (TakeCrossElement) { start--; }
            for (int i = start; i < enumerable.Count(); i++)
            {
                yield return enumerable.ElementAt(i);
            }
        }
        public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Boolean SkipCrossElement = false) {
            Boolean started=false;
            for (int i = 0; i < enumerable.Count(); i++) {
                if (started) { yield return enumerable.ElementAt(i); }
                else {
                    if (!predicate(enumerable.ElementAt(i))) {
                        started = true;
                        if (!SkipCrossElement) { yield return enumerable.ElementAt(i); }
                    }
                }
            }
        }
        public static IEnumerable<T> EndSkipWhile<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Boolean SkipCrossElement = false)
        {
            Int32 stop = 0;
            for (int i = enumerable.Count(); i > 0; i--)
            {
                if (!predicate(enumerable.ElementAt(i - 1)))
                {
                    stop = i;
                    break;
                }
            }
            if (SkipCrossElement) { stop--; }
            for (int i = 0; i < stop; i++)
            {
                yield return enumerable.ElementAt(i);
            }
        }
        public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict,
                                          TKey oldKey, TKey newKey) {
            TValue value;
            if (!dict.TryGetValue(oldKey, out value))
                return false;

            dict.Remove(oldKey);  // do not change order
            dict[newKey] = value;  // or dict.Add(newKey, value) depending on ur comfort
            return true;
        }

    }
}
