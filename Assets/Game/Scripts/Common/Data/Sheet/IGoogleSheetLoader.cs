using Cysharp.Threading.Tasks;

namespace Data.Sheet
{
    using Container;
    public interface IGoogleSheetLoader
    {
        GoogleSheetContainer Container { get; }
        UniTask Bake();
    }
}
