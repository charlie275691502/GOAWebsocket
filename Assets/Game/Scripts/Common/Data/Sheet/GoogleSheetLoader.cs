using System.IO;
using Cathei.BakingSheet;
using Cathei.BakingSheet.Unity;
using Cysharp.Threading.Tasks;
using Data.Sheet.Container;

namespace Data.Sheet
{
	public class GoogleSheetLoader : IGoogleSheetLoader
	{
		private const string GOOGLE_CREDENTIAL_PATH = "Assets/Plugins/goawebsocket-9f0c27c1ca90.json";
		private const string GOOGLE_SHEET_ID = "1UbLqdqDDZZpHL3ImWwaUEDsH8K5NIEpE2KKlTVkChCo";
		
		public GoogleSheetContainer Container { get; private set; }
		
		public async UniTask Bake()
		{
			var logger = new UnityLogger();

			var googleSheetContainer = new GoogleSheetContainer(logger);
			string googleCredential = File.ReadAllText(GOOGLE_CREDENTIAL_PATH);
			var googleConverter = new GoogleSheetConverter(GOOGLE_SHEET_ID, googleCredential);
			await googleSheetContainer.Bake(googleConverter);
			
			Container = googleSheetContainer;
		}
	}
}
