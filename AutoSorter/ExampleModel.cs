using AutoSorter.Attributes;

namespace AutoSorter
{
    public class ExampleModel
    {
        [AutoSort(0)]
        public int Id { get; set; }

        [AutoSortArray(new[] {1, 2, 3}, new[] {0, 2, 3})]
        public int[] Array { get; set; } = {0, 1, 2, 3};
    }

}