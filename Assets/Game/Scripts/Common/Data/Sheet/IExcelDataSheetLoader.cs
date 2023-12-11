using Cysharp.Threading.Tasks;

namespace Data.Sheet
{
    using Container;
    public interface IExcelDataSheetLoader
    {
		ExcelDataSheetContainer Container { get; }
		UniTask Bake();
    }
}
