using System.Collections.Generic;

namespace Schwartz.Siemens.Core.Entities.Rigs
{
    public class FilteredList<T>
    {
        public FilteredList(List<T> list)
        {
            List = list ?? new List<T>();
        }

        public List<T> List { get; }
        public int Count => List.Count;
    }
}