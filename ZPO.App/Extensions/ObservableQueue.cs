using System;
using System.Collections.ObjectModel;

namespace ZPO.App.Extensions
{
    /// <summary>
    /// Simulates observable queue with limited count of items 
    /// </summary>
    public class ObservableQueue<T> : ObservableCollection<T>
    {
        private readonly int _capacity;

        public ObservableQueue(int capacity)
        {
            if(capacity <= 0)
                throw new ArgumentException("Capacity cannot be zero or negative", nameof(capacity));
            _capacity = capacity;
        }

        protected override void InsertItem(int index, T item)
        {
            if (Count == _capacity)
            {
                this.RemoveAt(this.Count - 1);
            }

            base.InsertItem(index, item);
        }
    }
}
