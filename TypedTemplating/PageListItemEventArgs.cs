using System;

namespace TypedTemplating
{
    public class PageListItemEventArgs : EventArgs
    {
        public PageListItemEventArgs(PageListItem item)
        {
            Item = item;
        }

        public PageListItem Item { get; private set; }
    }
}
