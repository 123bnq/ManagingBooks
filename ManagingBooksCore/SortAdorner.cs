using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ManagingBooks
{
    class SortAdorner : Adorner
    {
        private static Geometry ascGeometry = Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");
        private static Geometry desGeometry = Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement adornedElement, ListSortDirection dir) : base(adornedElement)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
            {
                return;
            }

            TranslateTransform transform = new TranslateTransform(AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2);
            drawingContext.PushTransform(transform);
            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
            {
                geometry = desGeometry;
            }

            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }

        public static void SortClick(object sender, RoutedEventArgs e, ref GridViewColumnHeader listViewSortCol, ref SortAdorner listViewSortAdorner, ref ListView list)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string sortBy = column.Tag.ToString();

            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                list.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
            {
                newDir = ListSortDirection.Descending;
            }
            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);

            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            list.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
}
