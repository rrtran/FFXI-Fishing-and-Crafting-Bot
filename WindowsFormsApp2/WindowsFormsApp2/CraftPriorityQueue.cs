using System;

namespace WindowsFormsApp2
{
    class CraftPriorityQueue
    {
        Tuple<int, Class3>[] x;
        int count;
        int last;
        int capacity;

        public CraftPriorityQueue()
        {
            last = -1;
            count = 0;
            capacity = 256;
            x = new Tuple<int, Class3>[capacity];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = null;
            }
        }

        public void insert(int priority, Class3 data)
        {
            if (count == 0)
            {
                x[0] = new Tuple<int, Class3>(priority, data);
                last = 0;
                count++;
                return;
            }
            else if (count < capacity)
            {
                Tuple<int, Class3> tuple = new Tuple<int, Class3>(priority, data);
                x[last + 1] = tuple;
                int i = last + 1;
                int parentIndex = (i - 1) / 2;
                while (x[i].Item1 > x[parentIndex].Item1)
                {
                    Tuple<int, Class3> temp = x[i];
                    x[i] = x[parentIndex];
                    x[parentIndex] = temp;
                    i = parentIndex;
                    parentIndex = (i - 1) / 2;
                }

                last = last + 1;
                count++;
                return;
            }
            else
            {
                int newCapacity = capacity * 2;
                Tuple<int, Class3>[] x2 = new Tuple<int, Class3>[newCapacity];
                for (int i = 0; i < newCapacity; i++)
                {
                    x2[i] = null;
                }

                for (int i = 0; i < capacity; i++)
                {
                    x2[i] = x[i];
                    x[i] = null;
                }

                x = x2;
                capacity = newCapacity;

                insert(priority, data);
            }
        }

        public void remove()
        {
            if (count == 0)
            {
                return;
            }
            else if (count == 1)
            {
                x[0] = null;
                last = -1;
                count--;
                return;
            }
            else
            {
                x[0] = x[last];
                x[last] = null;
                last = last - 1;
                count--;

                int i = 0;
                while (2 * i + 1 < count && 2 * i + 2 < count)
                {
                    if (x[2 * i + 1].Item1 > x[i].Item1)
                    {
                        if (x[2 * i + 2].Item1 > x[i].Item1)
                        {
                            if (x[2 * i + 1].Item1 > x[2 * i + 2].Item1)
                            {
                                Tuple<int, Class3> temp = x[i];
                                x[i] = x[2 * i + 1];
                                x[2 * i + 1] = temp;
                                i = 2 * i + 1;
                            }
                            else
                            {
                                Tuple<int, Class3> temp = x[i];
                                x[i] = x[2 * i + 2];
                                x[2 * i + 2] = temp;
                                i = 2 * i + 2;
                            }
                        }
                        else
                        {
                            Tuple<int, Class3> temp = x[i];
                            x[i] = x[2 * i + 1];
                            x[2 * i + 1] = temp;
                            i = 2 * i + 1;
                        }
                    }
                    else if (x[2 * i + 2].Item1 > x[i].Item1)
                    {
                        if (x[2 * i + 1].Item1 > x[i].Item1)
                        {
                            if (x[2 * i + 2].Item1 > x[2 * i + 1].Item1)
                            {
                                Tuple<int, Class3> temp = x[i];
                                x[i] = x[2 * i + 2];
                                x[2 * i + 2] = temp;
                                i = 2 * i + 2;
                            }
                            else
                            {
                                Tuple<int, Class3> temp = x[i];
                                x[i] = x[2 * i + 1];
                                x[2 * i + 1] = temp;
                                i = 2 * i + 1;
                            }
                        }
                        else
                        {
                            Tuple<int, Class3> temp = x[i];
                            x[i] = x[2 * i + 2];
                            x[2 * i + 2] = temp;
                            i = 2 * i + 2;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public int size()
        {
            return count;
        }

        public int getPriority()
        {
            return x[0].Item1;
        }

        public Class3 getData()
        {
            return x[0].Item2;
        }
    }
}