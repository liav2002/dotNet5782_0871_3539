using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public class PriorityQueue<T>
    {
        SortedList<Pair<double>, T> _list;
        int count;

        public PriorityQueue()
        {
            _list = new SortedList<Pair<double>, T>(new PairComparer<double>());
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Enqueue(T item, double priority)
        {
            _list.Add(new Pair<double>(priority, count), item);
            count++;
        }

        public T Dequeue()
        {
            T item = _list[_list.Keys[0]];
            _list.RemoveAt(0);
            return item;
        }

        private class Pair<K>
        {
            public K First { get; }
            public K Second { get; }

            public Pair(K first, K second)
            {
                First = first;
                Second = second;
            }

            public override int GetHashCode()
            {
                return First.GetHashCode() ^ Second.GetHashCode();
            }

            public override bool Equals(object other)
            {
                Pair<K> pair = other as Pair<K>;
                if (pair == null)
                {
                    return false;
                }

                return (this.First.Equals(pair.First) && this.Second.Equals(pair.Second));
            }
        }

        private class PairComparer<K> : IComparer<Pair<K>> where K : IComparable
        {
            public int Compare(Pair<K> x, Pair<K> y)
            {
                // in descending order
                if (x.First.CompareTo(y.First) > 0)
                {
                    return -1;
                }
                else if (x.First.CompareTo(y.First) < 0)
                {
                    return 1;
                }
                else
                {
                    return x.Second.CompareTo(y.Second);
                }
            }
        }
    }
}