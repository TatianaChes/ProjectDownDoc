using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Collections.Generic;
using System.Data;
using DbfDataReader;
using AvaloniaApplication2.Views;

namespace AvaloniaApplication2.Model
{
    public class StaticClass
    {
        public DataTable table;
        public int owner;
        public string file_name;
        public static bool LetRead = true;
        public StaticClass(DataTable table, int owner, string file_name) // для вывода итог прочтения
        {
            this.table = table;
            this.owner = owner;
            this.file_name = file_name;
        }

        private static string _currentTitle = "Вход"; // название окна приложения по умолчанию
        public static string CurrentTitle // свойство с обновлением
        {
            get => _currentTitle;
            set
            {
                _currentTitle = value;
                if (_mainWindow != null)
                {
                    _mainWindow.Title = _currentTitle; 
                }
            }
        }
        public static void SetTitle(string title) // установка свойства
        {
            CurrentTitle = title;
        }

        private static MainWindow _mainWindow; // получение главного окна
        public static void Initialize(MainWindow window)
        {
            _mainWindow = window;
            _mainWindow.Title = _currentTitle; 
        }

        // словарь для отображения названия документа и итоговой суммы
        public static Dictionary<string, decimal> resultReader { get; set; }
        public static List<StaticClass> datas = new List<StaticClass>();
        public static Dictionary<string, int> file_pathDBF = new Dictionary<string, int>();

        public static void ShowMessageBox(string title, string content, ButtonEnum ok) // реализация показа диалогового окна для всех страниц
        {
            MessageBoxManager.GetMessageBoxStandard(content, title, ok).ShowAsync();
        }
        // Определение столбцов для импорта и их соответствие столбцам в БД 
        // данный список позволяет учесть нужные столбцы, т.к импортируются не все
        public static Dictionary<string, string> columnMapping = new Dictionary<string, string>()
                            {
                                {"RPT_ID", "rpt_id"}, // пример: столбец "RPT_ID" в DBF соответствует столбцу "rpt_id" в БД
                                {"RPT_LN_ID", "rpt_ln_id"},  // пример: столбец "RPT_LN_ID" в DBF соответствует столбцу "rpt_ln_id" в БД
                                {"RPT_DATE", "rpt_date"},
                                {"RPT_TYPE", "rpt_type"},
                                {"RAS_CODE", "ras_code"},
                                {"RAS_OGRN", "ras_ogrn"},
                                {"RAS_NAMUL", "ras_namul"},
                                {"RAS_OKATO", "ras_okato"},
                                {"FO_NAMUL", "fo_namul"},
                                {"FO_CODE", "fo_code"},
                                {"FO_OGRN", "fo_ogrn"},
                                {"FO_OKATO", "fo_okato"},
                                {"NOMK_LS", "nomk_ls"},
                                {"NOM_ID", "nom_id"},
                                {"NOM_CODE", "nom_code"},
                                {"NAME_MED", "name_med"},
                                {"C_TRN", "c_trn"},
                                {"C_MNN", "c_mnn"},
                                {"C_LF", "c_lf"},
                                {"D_LS", "d_ls"},
                                {"N_FV", "n_fv"},
                                {"N_FVU", "n_fvu"},
                                {"NAME_FCT", "name_fct"},
                                {"NAME_CNF", "name_cnf"},
                                {"SERNUMB", "sernumb"},
                                {"SROK_GODN", "srok_godn"},
                                {"PRICE_LS", "price_ls"},
                                {"KO_ALL", "ko_all"},
                                {"SL_ALL", "sl_all"},
                                {"OWNER", "owner"},
                                {"PROD_ID", "prod_id"},
                                {"PROD_CODE", "prod_code"},
                                {"STORN_SIGN", "storn_sign"},
                                {"SPECIFIC", "store_code"},
                                {"GOD_K", "god_k"},

                            };
        public static DbfDataReaderOptions dbfOption = new DbfDataReaderOptions // опции для чтения дбф файлов
        {
            SkipDeletedRecords = true
            // Encoding = EncodingProvider.GetEncoding(1252);
        };
    }
}
