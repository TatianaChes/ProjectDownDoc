using AvaloniaApplication2.ViewModels;
using DbfDataReader;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AvaloniaApplication2.Model
{
    public class SecondModel // класс для работы с БД MS SQL
    {
        public static string? ConnectionString { get; set; }
        Dictionary<string, decimal> _resultReader = new Dictionary<string, decimal> { };
        DateTime lastDayMonthAgo = DateTime.Today.AddDays(-1 * DateTime.Today.Day);
        FifthPageViewModel viewModel = new FifthPageViewModel();

        public bool CheckConnectionDB(string login, string password)
        {
            string connectionString =
             @"Data Source = 192.168.1.237; Initial Catalog = Registr; Integrated Security=False; 
                   User Id = " + login + "; Password = " + password;
            ConnectionString = connectionString;
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally { connection.Close(); }
        }

        // Метод для проверки, существует ли DBF с заданным значением checkColumnName
        private bool CheckIfDbfExists(DateTime rptDate, int specificData)
        {
            string sqlQuery = $"SELECT COUNT(*) FROM otchet_rt WHERE rpt_date = '{rptDate.ToShortDateString()}' AND store_code = {specificData.ToString()}";
            // Создание объекта SqlConnection
            using (SqlConnection connectionSQL = new SqlConnection(ConnectionString))
            {
                // Открытие соединения
                connectionSQL.Open();
                using (var command = new SqlCommand(sqlQuery, connectionSQL))
                {
                    command.Parameters.AddWithValue("@rpt_date", rptDate);
                    command.Parameters.AddWithValue("@store_code", specificData);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        //обход полученных файлов
        private bool FileCrawling()
        {
            bool alreadyExists = false; // проверка на наличие записей

            if (StaticClass.file_pathDBF.Any()) // проверка что файлы были подгружены
            {
                foreach (var dbfPath in StaticClass.file_pathDBF) // перебираем массив с названиями файлов 
                {
                    using (var dbfDataReader = new DbfDataReader.DbfDataReader(dbfPath.Key, StaticClass.dbfOption)) // подключение по пути                         
                    {
                        // Получение значений даты и специфика
                        DateTime rptDate = default;
                        int specificData = 0;

                        if (dbfDataReader.Read())
                        {
                            rptDate = dbfDataReader.GetDateTime("RPT_DATE");
                            specificData = dbfDataReader.GetInt32("SPECIFIC");
                        }
                        // Проверка, загружалась ли такая dbf ранее
                        alreadyExists = CheckIfDbfExists(rptDate, specificData);

                        if (alreadyExists)
                        {
                            StaticClass.ShowMessageBox($"Файл {dbfPath.Key} уже был подгружен в базу!", "Оповещение", ButtonEnum.Ok);
                            return true;
                        }
                    }
                }
            }
            else
            {
                StaticClass.ShowMessageBox("Вы не подгрузили ни 1 файла!", "Предупреждение", ButtonEnum.Ok);
                return true;
            }
            return false;
        }

        public void WriteDBFInSQL()
        {
            bool resultCheck = FileCrawling(); // проверка свежести данных
            if (!resultCheck)
            {
                try
                {
                    foreach (var dbfPath in StaticClass.file_pathDBF) // перебираем массив с названиями файлов 
                    {
                        using (var dbfDataReader = new DbfDataReader.DbfDataReader(dbfPath.Key, StaticClass.dbfOption)) // подключаемся к дбф
                        {
                            // Создание DataTable для хранения данных из DBF
                            DataTable dt = new DataTable();
                            // Добавление столбцов в DataTable
                            foreach (var column in StaticClass.columnMapping.Values) // СТОЛБЦЫ ИЗ СТАТИЧЕСКОГО КЛАССА
                            {
                                dt.Columns.Add(column);
                            }
                            // Чтение данных из DBF и запись в DataTable
                            while (dbfDataReader.Read())
                            {
                                DataRow row = dt.NewRow();
                                foreach (var mapping in StaticClass.columnMapping)
                                {
                                    row[mapping.Value] = dbfDataReader[mapping.Key];
                                }
                                // в процессе чтения переписываем специфик по необходимости
                                switch (dbfDataReader["SPECIFIC"])
                                {
                                    case 11:
                                        row["store_code"] = 15;
                                        break;
                                    case 10:
                                        row["store_code"] = 14;
                                        break;
                                    default:
                                        row["store_code"] = row["store_code"];
                                        break;
                                }
                                dt.Rows.Add(row);
                            }
                            //Подключение к базе данных SQL Server
                            using (SqlConnection sqlConn = new SqlConnection(ConnectionString)) // при использовании using, close не прописывается 
                            {
                                sqlConn.Open();
                                string commandText = "INSERT INTO otchet_rt (rpt_id, rpt_ln_id, rpt_date, rpt_type, ras_code, ras_ogrn, ras_namul, ras_okato," +
                                    "fo_namul, fo_code, fo_ogrn, fo_okato, nomk_ls, nom_id, nom_code, name_med, c_trn, c_mnn, c_lf, d_ls, n_fv, n_fvu, name_fct," +
                                    "sernumb, srok_godn, price_ls, ko_all, sl_all, owner, prod_id, prod_code, storn_sign, store_code, god_k, doz_me) " +
                                    "VALUES (@RPT_ID, @RPT_LN_ID, @RPT_DATE, @RPT_TYPE, @RAS_CODE, @RAS_OGRN, @RAS_NAMUL, @RAS_OKATO, @FO_NAMUL, @FO_CODE, @FO_OGRN," +
                                    "@FO_OKATO, @NOMK_LS, @NOM_ID, @NOM_CODE, @NAME_MED, @C_TRN, @C_MNN, @C_LF, @D_LS, @N_FV, @N_FVU, @NAME_FCT," +
                                    "@SERNUMB, @SROK_GODN, @PRICE_LS, @KO_ALL, @SL_ALL, @OWNER, @PROD_ID, @PROD_CODE, @STORN_SIGN, @SPECIFIC, @GOD_K, @DOZ_ME)";
                                foreach (DataRow row in dt.Rows)
                                {
                                    using (SqlCommand command = new SqlCommand(commandText, sqlConn))
                                    {
                                        command.Parameters.AddWithValue("@RPT_ID", Convert.ToDecimal(row["rpt_id"]));
                                        command.Parameters.AddWithValue("@RPT_LN_ID", Convert.ToDecimal(row["rpt_ln_id"]));
                                        command.Parameters.AddWithValue("@RPT_DATE", Convert.ToDateTime(row["rpt_date"]));
                                        command.Parameters.AddWithValue("@RPT_TYPE", (row["rpt_type"]));
                                        command.Parameters.AddWithValue("@RAS_CODE", Convert.ToString(row["ras_code"]));
                                        command.Parameters.AddWithValue("@RAS_OGRN", Convert.ToString(row["ras_ogrn"]));
                                        command.Parameters.AddWithValue("@RAS_NAMUL", Convert.ToString(row["ras_namul"]));
                                        command.Parameters.AddWithValue("@RAS_OKATO", Convert.ToDecimal(row["ras_okato"]));
                                        command.Parameters.AddWithValue("@FO_NAMUL", Convert.ToString(row["fo_namul"]));
                                        command.Parameters.AddWithValue("@FO_CODE", Convert.ToString(row["fo_code"]));
                                        command.Parameters.AddWithValue("@FO_OGRN", Convert.ToDecimal(row["fo_ogrn"]));
                                        command.Parameters.AddWithValue("@FO_OKATO", Convert.ToDecimal(row["fo_okato"]));
                                        command.Parameters.AddWithValue("@NOMK_LS", Convert.ToDecimal(row["nomk_ls"]));
                                        command.Parameters.AddWithValue("@NOM_ID", Convert.ToDecimal(row["nom_id"]));
                                        command.Parameters.AddWithValue("@NOM_CODE", Convert.ToString(row["nom_code"]));
                                        command.Parameters.AddWithValue("@NAME_MED", Convert.ToString(row["name_med"]));
                                        command.Parameters.AddWithValue("@C_TRN", Convert.ToDecimal(row["c_trn"]));
                                        command.Parameters.AddWithValue("@C_MNN", Convert.ToDecimal(row["c_mnn"]));

                                        command.Parameters.AddWithValue("@C_LF", (row["c_lf"]));
                                        command.Parameters.AddWithValue("@D_LS", Convert.ToString(row["d_ls"]));
                                        command.Parameters.AddWithValue("@N_FV", Convert.ToString(row["n_fv"]));
                                        command.Parameters.AddWithValue("@N_FVU", Convert.ToString(row["n_fvu"]));
                                        command.Parameters.AddWithValue("@NAME_FCT", Convert.ToString(row["name_fct"]));
                                        command.Parameters.AddWithValue("@SERNUMB", Convert.ToString(row["sernumb"]));

                                        command.Parameters.AddWithValue("@SROK_GODN", Convert.ToDateTime(row["srok_godn"]));
                                        command.Parameters.AddWithValue("@PRICE_LS", Convert.ToDecimal(row["price_ls"]));
                                        command.Parameters.AddWithValue("@KO_ALL", Convert.ToDecimal(row["ko_all"]));
                                        command.Parameters.AddWithValue("@SL_ALL", Convert.ToDecimal(row["sl_all"]));
                                        command.Parameters.AddWithValue("@OWNER", (row["owner"]));
                                        command.Parameters.AddWithValue("@PROD_ID", Convert.ToDecimal(row["prod_id"]));

                                        command.Parameters.AddWithValue("@PROD_CODE", Convert.ToString(row["prod_code"]));
                                        command.Parameters.AddWithValue("@STORN_SIGN", (row["storn_sign"]));
                                        command.Parameters.AddWithValue("@SPECIFIC", Convert.ToInt32(row["store_code"]));
                                        command.Parameters.AddWithValue("@GOD_K", Convert.ToString(row["god_k"]));

                                        command.Parameters.AddWithValue("@DOZ_ME", (1));

                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    StaticClass.ShowMessageBox("DBF файлы успешно загружены!", "Оповещение", ButtonEnum.Ok);
                    viewModel.OnButtonClicked();
                }
                catch (Exception ex)
                {
                    StaticClass.ShowMessageBox($"Исключение: {ex.Message}", "Оповещение", ButtonEnum.Ok);
                }
            }
        }

        public bool CheckIfSQLExists()
        {
            string sqlQuery = "SELECT COUNT(*) FROM otchet_nedopost WHERE rpt_date = @rpt_date and store_code = @store_code";

            using (SqlConnection connectionSQL = new SqlConnection(ConnectionString))
            {
                connectionSQL.Open();

                foreach (var dataItem in StaticClass.datas)
                {
                    using (var command = new SqlCommand(sqlQuery, connectionSQL))
                    {
                        command.Parameters.AddWithValue("@rpt_date", lastDayMonthAgo);
                        command.Parameters.AddWithValue("@store_code", dataItem.owner);

                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            StaticClass.ShowMessageBox("Данные из файла " + dataItem.file_name + " уже были подгружены!", "Оповещение", ButtonEnum.OkCancel);
                            StaticClass.LetRead = false;
                            return false;
                        }
                    }
                }
            }
             return true;
        }

        public bool WriteToDataBase()
        {
            if (CheckIfSQLExists())
            {
                foreach (var dataItem in StaticClass.datas)
                {
                    try
                    {
                        //Подключение к базе данных SQL Server
                        using (SqlConnection sqlConn = new SqlConnection(ConnectionString)) // при использовании using, close не прописывается 
                        {
                            sqlConn.Open();
                            string commandText = "INSERT INTO otchet_nedopost (ko_all,nomk_ls,store_code,rpt_date) " +
                                "VALUES (@value1, @value2, @value3, @value4)";
                            foreach (DataRow row in dataItem.table.Rows)
                            {
                                using (SqlCommand command = new SqlCommand(commandText, sqlConn))
                                {
                                    command.Parameters.AddWithValue("@value1", Convert.ToDecimal(row[6]));
                                    command.Parameters.AddWithValue("@value2", Convert.ToDecimal(row[9]));
                                    command.Parameters.AddWithValue("@value3", Convert.ToInt16(dataItem.owner));
                                    command.Parameters.AddWithValue("@value4", Convert.ToDateTime(lastDayMonthAgo));
                                    command.ExecuteNonQuery();
                                }
                            }

                        }
                        //получение общей суммы столбца DataTable через linq
                        decimal sum = dataItem.table.AsEnumerable().Sum(x => Convert.ToDecimal(x["Сумма остатка по контракту"]));
                        string firstColumnValue = dataItem.file_name.Substring(dataItem.file_name.LastIndexOf(("\\")) + 1); // имя файла

                        _resultReader.Add(firstColumnValue, sum); // вставка в словарь

                    }
                    catch (Exception ex) { StaticClass.ShowMessageBox("При записи данных произошла ошибка!", "Оповещение", ButtonEnum.OkCancel); return false; }

                    StaticClass.resultReader = _resultReader;
                }
                StaticClass.ShowMessageBox("Данные успешно записаны в базу данных", "Оповещение", ButtonEnum.OkCancel);
                return true;
            }
            return false;
        }
    }
}
