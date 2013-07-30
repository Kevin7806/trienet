﻿// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gma.DataStructures.StringSearch
{
    [DebuggerDisplay(
        "{m_Origin.Substring(0,m_StartIndex)} [ {m_Origin.Substring(m_StartIndex,m_PartitionLength)} ] {m_Origin.Substring(m_StartIndex + m_PartitionLength)}"
        )]
    public struct StringPartition : IEnumerable<char>
    {
        private readonly string m_Origin;
        private readonly int m_PartitionLength;
        private readonly int m_StartIndex;

        public StringPartition(string origin)
            : this(origin, 0, origin.Length)
        {
        }

        public StringPartition(string origin, int startIndex)
            : this(origin, startIndex, origin.Length - startIndex)
        {
        }

        public StringPartition(string origin, int startIndex, int partitionLength)
        {
            m_Origin = string.Intern(origin);
            m_StartIndex = startIndex;
            int availableLength = m_Origin.Length - startIndex;
            m_PartitionLength = Math.Min(partitionLength, availableLength);
        }

        public char this[int index]
        {
            get { return m_Origin[m_StartIndex + index]; }
        }

        public int Length
        {
            get { return m_PartitionLength; }
        }

        #region IEnumerable<char> Members

        public IEnumerator<char> GetEnumerator()
        {
            for (int i = 0; i < m_PartitionLength; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public bool StartsWith(StringPartition other)
        {
            if (Length < other.Length)
            {
                return false;
            }

            for (int i = 0; i < other.Length; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public SplitResult Split(int splitAt)
        {
            var head = new StringPartition(m_Origin, m_StartIndex, splitAt);
            var rest = new StringPartition(m_Origin, m_StartIndex + splitAt, Length - splitAt);
            return new SplitResult(head, rest);
        }

        public ZipResult ZipWith(StringPartition other)
        {
            int splitIndex = 0;
            using (IEnumerator<char> thisEnumerator = GetEnumerator())
            using (IEnumerator<char> otherEnumerator = other.GetEnumerator())
            {
                while (thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
                {
                    if (thisEnumerator.Current != otherEnumerator.Current)
                    {
                        break;
                    }
                    splitIndex++;
                }
            }

            SplitResult thisSplitted = Split(splitIndex);
            SplitResult otherSplitted = other.Split(splitIndex);

            StringPartition commonHead = thisSplitted.Head;
            StringPartition restThis = thisSplitted.Rest;
            StringPartition restOther = otherSplitted.Rest;
            return new ZipResult(commonHead, restThis, restOther);
        }

        public override string ToString()
        {
            var result = new string(this.ToArray());
            return string.Intern(result);
        }
    }
}