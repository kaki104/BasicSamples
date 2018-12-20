using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlexGridMergeManagerSampleForWPF
{
    ///<summary>
    ///SampleModel
    ///</summary>
    public class SampleModel
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        public double ExtendedPrice { get; set; }
    }
}
