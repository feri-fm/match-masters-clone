using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3
{
    public class RandomTable
    {
        private List<int> overrides { get; } = new List<int>();
        private Dictionary<int, int> numbers { get; } = new Dictionary<int, int>();

        public int seed { get; private set; } = 12345;
        public int tableIndex { get; private set; } = 0;
        public int max { get; private set; } = 100;

        public int nextIndex { get; set; } = 0;

        private Random _random;
        private Random random => _random ?? BuildRandom();

        private Random BuildRandom()
        {
            _random = new Random(seed);
            tableIndex = 0;
            return _random;
        }

        public void Load(RandomTableData data)
        {
            tableIndex = 0;
            numbers.Clear();
            overrides.Clear();
            seed = data.seed;
            max = data.max;
            nextIndex = data.nextIndex;
            _random = null;
            for (int i = 0; i < data.indics.Length; i++)
            {
                overrides.Add(data.indics[i]);
                numbers[data.indics[i]] = data.values[i];
            }
        }

        public RandomTableData Save()
        {
            return new RandomTableData()
            {
                seed = seed,
                max = max,
                nextIndex = nextIndex,
                indics = overrides.ToArray(),
                values = overrides.Select(e => numbers[e]).ToArray(),
            };
        }

        public void BuildTo(int length)
        {
            for (int i = tableIndex; i < length; i++)
            {
                var r = random.Next(max);
                if (!numbers.ContainsKey(i))
                {
                    numbers[i] = r;
                }
                tableIndex++;
            }
        }

        public void SetSeed(int seed)
        {
            var old = Save();
            old.seed = seed;
            Load(old);
        }
        public void SetMax(int max)
        {
            var old = Save();
            old.max = max;
            Load(old);
        }

        public int GetAt(int index)
        {
            BuildTo(index + 1);
            return numbers[index];
        }

        public void SetAt(int index, int value)
        {
            BuildTo(index + 1);
            if (!overrides.Contains(index))
                overrides.Add(index);
            numbers[index] = value;
        }

        public void Clear(int index)
        {
            overrides.Remove(index);
            var i = nextIndex;
            var old = Save();
            Load(old);
            nextIndex = i;
        }

        public RandomResult Next()
        {
            var i = nextIndex++;
            var value = GetAt(i);
            return new RandomResult(this, value, i, overrides.Contains(i));
        }
    }

    [System.Serializable]
    public class RandomResult
    {
        public int value { get; private set; }
        public int index { get; }
        public bool isOverride { get; private set; }
        public RandomTable table { get; }

        public RandomResult(RandomTable table, int value, int index, bool isOverride)
        {
            this.table = table;
            this.value = value;
            this.index = index;
            this.isOverride = isOverride;
        }

        public void Set(int value)
        {
            table.SetAt(index, value);
            this.value = value;
            isOverride = true;
        }

        public void Clear()
        {
            table.Clear(index);
            isOverride = false;
            value = table.GetAt(index);
        }
    }

    [System.Serializable]
    public class RandomTableData
    {
        public int seed;
        public int max;
        public int nextIndex;
        public int[] indics;
        public int[] values;
    }
}
