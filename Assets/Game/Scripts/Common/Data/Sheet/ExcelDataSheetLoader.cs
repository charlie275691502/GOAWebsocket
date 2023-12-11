using Cathei.BakingSheet;
using Cathei.BakingSheet.Unity;
using Cysharp.Threading.Tasks;
using Data.Sheet.Container;

namespace Data.Sheet
{
	public class ExcelDataSheetLoader : IExcelDataSheetLoader
	{
		private const string EXCEL_DATA_FOLDER_PATH = "Assets/Game/Resources/DataSheets";
		
		public ExcelDataSheetContainer Container { get; private set; }
		
		public async UniTask Bake()
		{
			var logger = new UnityLogger();

			var excelDataSheetContainer = new ExcelDataSheetContainer(logger);
			var excelSheetConverter = new ExcelSheetConverter(EXCEL_DATA_FOLDER_PATH);
			await excelDataSheetContainer.Bake(excelSheetConverter);
			
			Container = excelDataSheetContainer;
		}
	}
}
