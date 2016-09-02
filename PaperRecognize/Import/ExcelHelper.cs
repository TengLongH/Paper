using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using NPOI.SS.Util;
using System.Text.RegularExpressions;


namespace PaperRecognize.Import
{
    public class ExcelHelper : IDisposable
    {
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed = false;
        private int[] column = null;

        /*private int authors;
        private int address;
        private int connAuthor;
        private int authorName;
        private int firstDept;
        private int oUCOrder;
        private int college;
        private int lab;
        private int connAuthorName;
        private int connAuthorDept;*/

        #region Excel表格字段
        public int PressType { get; set; }
        public int AuthorsShort { get; set; }
        public int AuthorName { get; set; }
        public int FirstDept { get; set; }
        public int OUCOrder { get; set; }
        public int College { get; set; }
        public int Lab { get; set; }
        public int Authors { get; set; }
        public int PaperName { get; set; }
        public int JournalName { get; set; }
        public int Series { get; set; }
        public int Language { get; set; }
        public int PaperType { get; set; }
        public int AuthorKeyWord { get; set; }
        public int KeyWords { get; set; }
        public int Abstract { get; set; }
        public int Address { get; set; }
        public int ConnAuthor { get; set; }
        public int ConnAuthorName { get; set; }
        public int ConnAuthorDept { get; set; }
        public int EmailAddess { get; set; }
        public int CitedReference { get; set; }
        public int CitedReferenceCount { get; set; }
        public int TotalTimesCited { get; set; }
        public int Press { get; set; }
        public int City { get; set; }
        public int PressAdress { get; set; }
        public int ISSN { get; set; }
        public int DI { get; set; }
        public int StandardJournalShort { get; set; }
        public int ISIJournalShort { get; set; }
        public int PublishDate { get; set; }
        public int PublishYear { get; set; }
        public int Volume { get; set; }
        public int Issue { get; set; }
        public int PartNumber { get; set; }
        public int Supplement { get; set; }
        public int SpecialIssue { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int ArticleNumber { get; set; }
        public int PageCount { get; set; }
        public int SubjectCategory { get; set; }
        public int Citations { get; set; }
        public int ISIDeliveryNo { get; set; }
        public int ISIArticleIdentifier { get; set; }
        #endregion

        public ExcelHelper()
        {
            PressType = ConfigHelper.GetConfiguration<int>(int.Parse, "PressType");
            AuthorsShort = ConfigHelper.GetConfiguration<int>(int.Parse, "AuthorsShort");
            AuthorName = ConfigHelper.GetConfiguration<int>(int.Parse, "AuthorName");
            FirstDept = ConfigHelper.GetConfiguration<int>(int.Parse, "FirstDept");
            OUCOrder = ConfigHelper.GetConfiguration<int>(int.Parse, "OUCOrder");
            College = ConfigHelper.GetConfiguration<int>(int.Parse, "College");
            Lab = ConfigHelper.GetConfiguration<int>(int.Parse, "Lab");
            Authors = ConfigHelper.GetConfiguration<int>(int.Parse, "Authors");

            PaperName = ConfigHelper.GetConfiguration<int>(int.Parse, "PaperName");
            JournalName = ConfigHelper.GetConfiguration<int>(int.Parse, "JournalName");
            Series = ConfigHelper.GetConfiguration<int>(int.Parse, "Series");
            Language = ConfigHelper.GetConfiguration<int>(int.Parse, "Language");
            PaperType = ConfigHelper.GetConfiguration<int>(int.Parse, "PaperType");
            AuthorKeyWord = ConfigHelper.GetConfiguration<int>(int.Parse, "AuthorKeyWord");
            KeyWords = ConfigHelper.GetConfiguration<int>(int.Parse, "KeyWords");
            Abstract = ConfigHelper.GetConfiguration<int>(int.Parse, "Abstract");
            Address = ConfigHelper.GetConfiguration<int>(int.Parse, "Address");
            ConnAuthor = ConfigHelper.GetConfiguration<int>(int.Parse, "ConnAuthor");
            ConnAuthorName = ConfigHelper.GetConfiguration<int>(int.Parse, "ConnAuthorName");
            ConnAuthorDept = ConfigHelper.GetConfiguration<int>(int.Parse, "ConnAuthorDept");

            EmailAddess = ConfigHelper.GetConfiguration<int>(int.Parse, "EmailAddess");
            CitedReference = ConfigHelper.GetConfiguration<int>(int.Parse, "CitedReference");
            CitedReferenceCount = ConfigHelper.GetConfiguration<int>(int.Parse, "CitedReferenceCount");
            TotalTimesCited = ConfigHelper.GetConfiguration<int>(int.Parse, "TotalTimesCited");
            Press = ConfigHelper.GetConfiguration<int>(int.Parse, "Press");
            City = ConfigHelper.GetConfiguration<int>(int.Parse, "City");
            PressAdress = ConfigHelper.GetConfiguration<int>(int.Parse, "PressAdress");
            ISSN = ConfigHelper.GetConfiguration<int>(int.Parse, "ISSN");
            DI = ConfigHelper.GetConfiguration<int>(int.Parse, "DI");
            StandardJournalShort = ConfigHelper.GetConfiguration<int>(int.Parse, "StandardJournalShort");
            ISIJournalShort = ConfigHelper.GetConfiguration<int>(int.Parse, "ISIJournalShort");
            PublishDate = ConfigHelper.GetConfiguration<int>(int.Parse, "PublishDate");
            PublishYear = ConfigHelper.GetConfiguration<int>(int.Parse, "PublishYear");
            Volume = ConfigHelper.GetConfiguration<int>(int.Parse, "Volume");
            Issue = ConfigHelper.GetConfiguration<int>(int.Parse, "Issue");
            PartNumber = ConfigHelper.GetConfiguration<int>(int.Parse, "PartNumber");
            Supplement = ConfigHelper.GetConfiguration<int>(int.Parse, "Supplement");
            SpecialIssue = ConfigHelper.GetConfiguration<int>(int.Parse, "SpecialIssue");

            StartPage = ConfigHelper.GetConfiguration<int>(int.Parse, "StartPage");
            EndPage = ConfigHelper.GetConfiguration<int>(int.Parse, "EndPage");
            ArticleNumber = ConfigHelper.GetConfiguration<int>(int.Parse, "ArticleNumber");
            PageCount = ConfigHelper.GetConfiguration<int>(int.Parse, "PageCount");
            SubjectCategory = ConfigHelper.GetConfiguration<int>(int.Parse, "SubjectCategory");
            Citations = ConfigHelper.GetConfiguration<int>(int.Parse, "Citations");
            ISIDeliveryNo = ConfigHelper.GetConfiguration<int>(int.Parse, "ISIDeliveryNo");
            ISIArticleIdentifier = ConfigHelper.GetConfiguration<int>(int.Parse, "ISIArticleIdentifier");

            column = new int[] { Authors, Address, ConnAuthor, AuthorName, FirstDept, OUCOrder, College, Lab, ConnAuthorName, ConnAuthorDept };
            
        }

        /// <summary>
        /// 获取cell的数据，并设置为对应的数据类型
        /// </summary>
        /// <param name="cell">excel单元格</param>
        /// <returns>返回相应的数据类型</returns>
        public object GetCellValue(ICell cell)
        {
            object value = null;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date Type的数据CellType是Numeric
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;
                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                value = "";
            }

            return value;
        }

        /// <summary>
        /// 根据数据类型设置不同类型的cell
        /// </summary>
        /// <param name="cell">Excel的单元格</param>
        /// <param name="obj">不同类型的数据</param>
        public void SetCellValue(ICell cell, object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                cell.SetCellValue((int)obj);
            }
            else if (obj.GetType() == typeof(double))
            {
                cell.SetCellValue((double)obj);
            }
            else if (obj.GetType() == typeof(IRichTextString))
            {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj.GetType() == typeof(string))
            {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj.GetType() == typeof(DateTime))
            {
                cell.SetCellValue((DateTime)obj);
            }
            else if (obj.GetType() == typeof(bool))
            {
                cell.SetCellValue((bool)obj);
            }
            else
            {
                cell.SetCellValue(obj.ToString());
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="fileName">excel文件</param>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string fileName, string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                using (fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(fs);
                    fs.Close();
                }
                
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                            else
                            {
                                DataColumn column = new DataColumn("");
                                data.Columns.Add(column);
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            {
                                ICell cell = row.GetCell(j);
                                dataRow[j] = GetCellValue(cell);
                                //dataRow[j] = row.GetCell(j).ToString();
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataSet中
        /// </summary>
        /// <param name="fileName">excel文件</param>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataSet</returns>
        /*public DataSet ExcelToDataSet(string fileName, string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataSet ds = new DataSet();
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                using (fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(fs);
                    fs.Close();
                }
                
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                            {
                                ICell cell = row.GetCell(j);
                                dataRow[j] = GetCellValue(cell);
                                //dataRow[j] = row.GetCell(j).ToString();
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                ds.Tables.Add(data);

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }*/

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="fileName">excel文件</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string fileName, string sheetName, bool isColumnWritten)
        {
            //int i = 0;
            //int j = 0;
            int count = 0;
            ISheet sheet = null;

            /*if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }*/
            using (fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                workbook = WorkbookFactory.Create(fs);
                fs.Close();
            }

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试创建这个sheet
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }                
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                ICellStyle rowstyle = workbook.CreateCellStyle();   //创建cell样式
                rowstyle.FillForegroundColor = IndexedColors.Yellow.Index;
                rowstyle.FillPattern = FillPattern.SolidForeground;

                int rowCount = 0;   //下拉列表数据行数

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    bool multiRow = false;
                    IRow row = sheet.CreateRow(count);
                    row.Height = 18 * 20;
                    if (data.Rows[i][AuthorName].ToString().Contains("/"))  //包含重名的情况
                    {
                        row.RowStyle = rowstyle;
                        multiRow = true;
                    }
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        //row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        ICell cell = row.CreateCell(j);
                        SetCellValue(cell, data.Rows[i][j]);    //设置单元格的值
                        if (multiRow)
                        {
                            cell.CellStyle = rowstyle;  
                            if (j == AuthorName)
                            {
                                SetDropDownList(sheet, "DataSheet", "AuthorName", data.Rows[i][j].ToString(), ref rowCount, i + 1, j);  
                                //BuildDropData(data.Rows[i][j].ToString());
                            } 
                        }
                        
                    }
                    ++count;
                }

                for (int i = 0; i < data.Columns.Count; i++)    //设置列宽，隐藏相应列
                {
                    if (column.Where(c => c == i).Count() > 0)
                    {
                        if (i == AuthorName)
                        {
                            sheet.SetColumnWidth(i, 50 * 256);
                        }
                        else
                        {
                            sheet.SetColumnWidth(i, 15 * 256);
                        }
                        
                        continue;
                    }
                    sheet.SetColumnHidden(i, true);
                }
                    
                using (fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    workbook.Write(fs); //写入到excel
                    fs.Close();
                }

                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 构建下拉列表使用数据
        /// </summary>
        /// <param name="names">待处理的姓名字符串</param>
        /// <returns>迭代组合完成的姓名列表</returns>
        public static string[] BuildDropData(string names)
        {
            string[] nameList = names.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);   //分割姓名
            string[] sameList = null;   //重名列表
            List<string> temp = new List<string>(); //初次模板数据
            Dictionary<int, List<string>> sameDict = new Dictionary<int, List<string>>();   //重名字典，key为在names中的下标，value为各个重名的名字

            for (int i = 0; i < nameList.Count(); i++)  //设置初次迭代模板数据，如格式为a;b/c;d/e;f，则模板数据为a;"";"";f
            {
                if (nameList[i].Contains("/"))  //包含重名
                {
                    sameList = nameList[i].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    temp.Add("");
                    List<string> t = new List<string>(sameList);
                    sameDict.Add(i, t);
                }
                else
                {
                    temp.Add(nameList[i]);
                }
            }

            List<string> dropData = new List<string>(); //最终处理完成的姓名列表            
            List<string> slist = new List<string>();    //保存重名列表
            List<List<string>> lList = new List<List<string>>();    //每次迭代组合姓名时的模板数据
            List<List<string>> tList = new List<List<string>>();    //中间数据
            lList.Add(temp);

            foreach (int key in sameDict.Keys)  //迭代组合姓名
            {
                slist = sameDict[key];
                for (int j = 0; j < slist.Count(); j++)
                {
                    foreach (List<string> l in lList)
                    {
                        List<string> t = new List<string>(l.ToArray());    
                        t[key] = slist[j];
                        tList.Add(t);
                    }                 
                }
                lList.Clear();
                tList.ForEach(l => lList.Add(new List<string>(l.ToArray())));   //复制中间产生数据为下一次迭代开始时的数据
                tList.Clear();
            }

            lList.ForEach(l => dropData.Add(string.Join(";", l.ToArray())));    //将处理完成的数据再次格式化

            return dropData.ToArray();
        }

        /// <summary>
        /// 设置excel下拉列表
        /// </summary>
        /// <param name="sheet">excel工作表</param>
        /// <param name="sheetName">下拉列表使用数据的sheet的名字</param>
        /// <param name="rangeName">下拉列表使用数据的域</param>
        /// <param name="names">待处理的姓名字符串</param>
        /// <param name="count">数据表中数据行数，ref引用格式</param>
        /// <param name="Row">所要设置的下拉列表所在行</param>
        /// <param name="Col">所要设置的下拉列表所在列</param>
        public static void SetDropDownList(ISheet sheet, string sheetName, string rangeName, string names, ref int count, int Row, int Col)
        {
            ISheet dataSheet = null;
            IRow row = null;
            ICell cell = null;
            IName name = null;

            IDataValidationHelper dvHelper = null;
            IDataValidationConstraint dvConstraint = null;
            IDataValidation validation = null;
            CellRangeAddressList addressList = null;

            dataSheet = sheet.Workbook.GetSheet(sheetName); //获取存储下拉数据的sheet
            if (dataSheet == null)  //若不存在，则创建
            {
                dataSheet = sheet.Workbook.CreateSheet(sheetName);
            }

            //string[] list = new string[] { "123", "456", "789" };
            string[] list = BuildDropData(names);   //获取下拉数据

            row = dataSheet.CreateRow(count++);

            for (int i = 0; i < list.Count(); i++)  //将数据写入sheet中
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(list[i]);
            }

            //生成一个列表引用区域,并唯一标识它
            name = sheet.Workbook.CreateName();
            name.RefersToFormula = string.Format("'{0}'!$A${1}:${2}${3}", sheetName, count, IndexToColumn(list.Count()), count);
            name.NameName = rangeName.ToUpper() + Row.ToString();

            addressList = new CellRangeAddressList(Row, Row, Col, Col); //设置生成下拉框的行和列
            dvHelper = sheet.GetDataValidationHelper();
            dvConstraint = dvHelper.CreateFormulaListConstraint(rangeName.ToUpper() + Row.ToString());
            validation = dvHelper.CreateValidation(dvConstraint, addressList);  //绑定下拉框和作用区域
            sheet.AddValidationData(validation);

            IWorkbook wb = sheet.Workbook;
            wb.SetSheetHidden(wb.GetSheetIndex(dataSheet), 1);  //隐藏数据sheet
        }

        /// <summary>
        /// 用于excel表格中列号字母转成列索引，从1对应A开始
        /// </summary>
        /// <param name="column">列号</param>
        /// <returns>列索引</returns>
        public static int ColumnToIndex(string column)
        {
            if (!Regex.IsMatch(column.ToUpper(), @"[A-Z]+"))
            {
                throw new Exception("Invalid parameter");
            }
            int index = 0;
            char[] chars = column.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index;
        }

        /// <summary>
        /// 用于将excel表格中列索引转成列号字母，从A对应1开始
        /// </summary>
        /// <param name="index">列索引</param>
        /// <returns>列号</returns>
        public static string IndexToColumn(int index)
        {
            if (index <= 0)
            {
                throw new Exception("Invalid parameter");
            }
            index--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return column;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
    }
}
