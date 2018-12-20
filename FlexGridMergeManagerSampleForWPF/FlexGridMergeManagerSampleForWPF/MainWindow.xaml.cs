using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using C1.WPF.FlexGrid;

namespace FlexGridMergeManagerSampleForWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string CASE1 = "Using grid[r,c]";
        private const string CASE2 = "Using CollectionView";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            switch (button.Content.ToString())
            {
                case CASE1:
                    BindGrid(CASE1);
                    break;
                case CASE2:
                    BindGrid(CASE2);
                    break;
            }
        }

        private void BindGrid(string type)
        {
            FlexGrid.MergeManager = new SampleMergeManager(type);

            var allText = File.ReadAllText("sample.csv");
            if (string.IsNullOrEmpty(allText)) return;
            var lines = allText.Split('\n');

            var items = (from line in lines.Skip(1)
                where string.IsNullOrEmpty(line) == false
                let columns = line.Split(',')
                select new SampleModel
                {
                    OrderID = Convert.ToInt32(columns[0]),
                    ProductID = Convert.ToInt32(columns[1]),
                    ProductName = columns[2],
                    UnitPrice = Convert.ToDouble(columns[3]),
                    Quantity = Convert.ToInt32(columns[4]),
                    Discount = Convert.ToDouble(columns[5]),
                    ExtendedPrice = Convert.ToDouble(columns[6].Replace("\r", ""))
                }).ToList();

            // bind grids to ListCollectionView
            FlexGrid.ItemsSource = items;
        }

        private class SampleMergeManager : IMergeManager
        {
            private static long _count;
            private readonly string _getDataType;

            public SampleMergeManager(string type)
            {
                _getDataType = type;
            }

            public CellRange GetMergedRange(C1FlexGrid grid, CellType cellType, CellRange rng)
            {
                if (cellType != CellType.Cell) return rng;
                var col = grid.Columns[rng.Column];
                if (col.AllowMerging == false) return rng;

                for (var i = rng.Row; i < grid.Rows.Count - 1; i++)
                {
                    if (CompareNext(grid, i, rng.Column))
                        break;
                    rng.Row2 = i + 1;
                }

                for (var i = rng.Row; i > 0; i--)
                {
                    if (ComparePrev(grid, i, rng.Column))
                        break;
                    rng.Row = i - 1;
                }

                _count++;
                Debug.WriteLine($"count : {_count}");
                return rng;
            }

            private bool CompareNext(C1FlexGrid grid, int r, int c)
            {
                if (_getDataType == CASE1)
                    return grid[r, c]?.ToString() != grid[r + 1, c]?.ToString();

                var col = grid.Columns[c];
                if (!(grid.CollectionView is ListCollectionView collection)) return false;
                var item1 = collection.GetItemAt(r);
                var item2 = collection.GetItemAt(r + 1);

                var property = item1.GetType().GetProperty(col.Binding.Path.Path);
                if (property == null) return false;
                var val1 = property.GetValue(item1);
                var val2 = property.GetValue(item2);

                return val1?.ToString() != val2?.ToString();
            }

            private bool ComparePrev(C1FlexGrid grid, int r, int c)
            {
                if (_getDataType == CASE1)
                    return grid[r, c]?.ToString() != grid[r - 1, c]?.ToString();

                var col = grid.Columns[c];
                if (!(grid.CollectionView is ListCollectionView collection)) return false;
                var item1 = collection.GetItemAt(r);
                var item2 = collection.GetItemAt(r - 1);

                var property = item1.GetType().GetProperty(col.Binding.Path.Path);
                if (property == null) return false;
                var val1 = property.GetValue(item1);
                var val2 = property.GetValue(item2);

                return val1?.ToString() != val2?.ToString();
            }
        }
    }
}