using System;
using System.Collections.ObjectModel;

namespace ZPO.App.Extensions
{
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
                this.RemoveAt(0);
                index--;
            }
            base.InsertItem(index, item);
        }
    }
}
