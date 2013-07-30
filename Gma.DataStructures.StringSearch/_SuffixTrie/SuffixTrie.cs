﻿using System.Collections.Generic;
using System.Linq;

namespace Gma.DataStructures.StringSearch
{
    public class SuffixTrie<T> : ITrie<T>
    {
        private readonly SimpleTrie<T> m_InnerTrie;
        private readonly int m_MinSuffixLength;

        public SuffixTrie(int minSuffixLength)
            : this(new SimpleTrie<T>(), minSuffixLength)
        {
        }

        private SuffixTrie(SimpleTrie<T> innerTrie, int minSuffixLength)
        {
            m_InnerTrie = innerTrie;
            m_MinSuffixLength = minSuffixLength;
        }

        public IEnumerable<T> Retrieve(string query)
        {
            return
                m_InnerTrie
                    .Retrieve(query)
                    .Distinct();
        }

        public void Add(string key, T value)
        {
            foreach (string suffix in GetAllSuffixes(m_MinSuffixLength, key))
            {
                m_InnerTrie.Add(suffix, value);
            }
        }

        private static IEnumerable<string> GetAllSuffixes(int minSuffixLength, string word)
        {
            for (int i = word.Length - minSuffixLength; i >= 0; i--)
            {
                var partition = new StringPartition(word, i);
                yield return partition.ToString();
            }
        }
    }
}