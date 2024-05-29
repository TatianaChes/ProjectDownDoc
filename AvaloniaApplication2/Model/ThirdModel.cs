using MsBox.Avalonia.Enums;
using OfficeOpenXml;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AvaloniaApplication2.Model
{
    public class ThirdModel //Класс для чтения Excel
    {
        public void WorkWithExcel(string files_path, int owner)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // установка некоммерческой лицензии 
            try
            {
                using (var package = new ExcelPackage(new FileInfo(@files_path))) // считываем по порядку файлы 
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); // получение первого листа 
                    DataTable dt = new DataTable();
                    foreach (var firstRowCell in worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]) // (2,1 - верхний левый угол), (2,end.column - нижний угол)  
                    {
                        dt.Columns.Add(firstRowCell.Text); // вставка шапки 
                    }
                    for (var rowNumber = 3; rowNumber <= worksheet.Dimension.End.Row - 1; rowNumber++)
                    {
                        var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column - 1];
                        var newRow = dt.Rows.Add();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                    }

                    if (checkExcelData(dt)) // проверка листа excel
                    {
                        StaticClass.datas.Add(new StaticClass(dt, owner, files_path)); // добавление таблицы, направления, файла
                    }
                    else {
                        StaticClass.ShowMessageBox($"Проверьте содержимое столбцов {files_path.Substring(files_path.LastIndexOf(("\\")) + 1)} на соответствие!", "Оповещение", ButtonEnum.OkCancel);
                        //StaticClass.datas.Clear();
                        StaticClass.LetRead = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticClass.ShowMessageBox(ex.ToString(), "Оповещение", ButtonEnum.OkCancel);
                return; // для полного выхода из метода buttonClick
            }
        }
        private bool checkExcelData(DataTable dt)
        {
            // 3 проверки - тип данных, пустота, null
            foreach (DataRow row in dt.Rows)
            {
                if ((!decimal.TryParse(row["Код ФГУ"].ToString(), out _)) 
                    || row["Код ФГУ"].ToString() == "" || row["Код ФГУ"] == null)
                {
                    return false;
                }
                if ((!decimal.TryParse(row["Остаток по контракту"].ToString(), out _)) 
                    || row["Остаток по контракту"].ToString() == ""
                    || row["Остаток по контракту"] == DBNull.Value)
                {
                    //isColumnNumeric = false;
                    //break;
                    return false;
                }
                if (row["Сумма остатка по контракту"].ToString() == "" || row["Сумма остатка по контракту"] == DBNull.Value)
                {
                    row["Сумма остатка по контракту"] = 0.0;
                }
                else if (!decimal.TryParse(row["Сумма остатка по контракту"].ToString(), out _))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
