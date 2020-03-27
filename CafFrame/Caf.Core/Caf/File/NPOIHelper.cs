using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NPOI;
using NPOI.XSSF;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Reflection;
using System.Linq;
using System.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using NPOI.HSSF.Util;
using Caf.Core.DependencyInjection;
using AspectCore.Extensions.Reflection;

namespace Caf.Core
{
    public class NPOIHelper : IExcelHelper
    {
        private readonly IFileUploader _fileUploader;

        public NPOIHelper(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public byte[] Export<T>(List<T> data, Dictionary<string, string> columns, string sheetName = null)
        {
            if (data != null && columns?.Count > 0)
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet(sheetName ?? "Sheet1");
                ExportToSheet<T>(sheet, data, columns);
                using (var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    return ms.ToArray();
                }
            }
            else
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet(sheetName ?? "Sheet1");
                ExportToEmptySheet(sheet, columns);
                using (var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public byte[] Export<T>(List<SheetData<T>> sheetDatas)
        {
            IWorkbook workbook = new XSSFWorkbook();
            foreach (var sheetData in sheetDatas)
            {
                if (sheetData.Datas != null && sheetData.Columns?.Count > 0)
                {
                    ISheet sheet = workbook.CreateSheet(sheetData.SheetName ?? "Sheet1");
                    ExportToSheet<T>(sheet, sheetData.Datas, sheetData.Columns);
                }
                else
                {
                    ISheet sheet = workbook.CreateSheet(sheetData.SheetName ?? "Sheet1");
                    ExportToEmptySheet(sheet, sheetData.Columns);
                }
            }
            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                return ms.ToArray();
            }

        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <param name="sheetName"></param>
        /// <param name="func">自定义</param>
        /// <returns></returns>
        public async Task<byte[]> Export<T>(List<T> data, Dictionary<string, string> columns, Func<string, T, Task<NpoiDataModel>> func, string sheetName = null)
        {
            if (data != null && columns?.Count > 0)
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet(sheetName ?? "Sheet1");
                await ExportToSheet<T>(sheet, data, columns, func, workbook);
                using (var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    return ms.ToArray();
                }
            }
            else
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet(sheetName ?? "Sheet1");
                ExportToEmptySheet(sheet, columns);
                using (var ms = new MemoryStream())
                {
                    workbook.Write(ms);
                    return ms.ToArray();
                }
            }
        }

        private async Task ExportToSheet<T>(ISheet sheet, List<T> data, Dictionary<string, string> columns, Func<string, T, Task<NpoiDataModel>> func, HSSFWorkbook workbook)
        {
            #region 设置下划线字体样式  
            IFont font = workbook.CreateFont();//创建字体样式  
            font.Color = HSSFColor.Blue.Index;//设置字体颜色  
            font.Underline = FontUnderlineType.Single;
            ICellStyle linkStyle = workbook.CreateCellStyle();//创建单元格样式  
            linkStyle.SetFont(font);//设置单元格样式中的字体样式  
            #endregion

            IRow headerRow = sheet.CreateRow(0);

            var dicProperty = new Dictionary<string, PropertyInfo>();
            var props = typeof(T).GetProperties();

            //设置标题行
            int i = 0;
            foreach (var item in columns)
            {
                headerRow.CreateCell(i).SetCellValue(item.Value);
                //匹配属性
                var prop = props.FirstOrDefault(p => p.Name.ToLower() == item.Key.ToLower());
                if (prop != null)
                {
                    dicProperty.Add(item.Key.ToLower(), prop);
                }
                i++;
            }

            //填充数据
            for (i = 0; i < data.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                var index = 0;
                foreach (var item in columns)
                {
                    NpoiDataModel model = await func(item.Key, data[i]);
                    if (model.CellType == CellType.Image)
                    {
                        row.Height = 80 * 20;
                        await AddCellPicture(sheet, workbook, model.Path, i + 1, index);
                        //row.CreateCell(index).SetCellValue(model.Path);
                    }
                    else if (model.CellType == CellType.Link)
                    {
                        var cell = row.CreateCell(index);
                        cell.SetCellValue(model.Path);
                        HSSFHyperlink link = new HSSFHyperlink(HyperlinkType.Url);//建一个HSSFHyperlink实体，指明链接类型为URL（这里是枚举，可以根据需求自行更改）  
                        link.Address = model.Path;//给HSSFHyperlink的地址赋值  
                        cell.Hyperlink = link;
                        cell.CellStyle = linkStyle;//为单元格设置显示样式   

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.PropertyName))
                        {
                            row.CreateCell(index).SetCellValue(dicProperty[model.PropertyName.ToLower()]?.GetValue(data[i])?.ToString() ?? "");
                        }
                        else
                        {
                            row.CreateCell(index).SetCellValue(model.Value);
                        }
                    }

                    index++;
                }
                //var row = sheet.CreateRow(i + 1);
                //foreach (var item in dicIndexer)
                //{
                //    row.CreateCell(item.Key).SetCellValue(item.Value.GetValue(data[i])?.ToString());
                //}
            }
        }

        private void ExportToSheet<T>(ISheet sheet, List<T> data, Dictionary<string, string> columns)
        {
            IRow headerRow = sheet.CreateRow(0);

            var dicIndexer = new Dictionary<int, PropertyInfo>();
            var props = typeof(T).GetProperties();

            //设置标题行
            int i = 0;
            foreach (var item in columns)
            {
                headerRow.CreateCell(i).SetCellValue(item.Value);

                //匹配属性
                var prop = props.FirstOrDefault(p => p.Name.ToLower() == item.Key.ToLower());
                if (prop != null)
                {
                    dicIndexer.Add(i, prop);
                    i++;
                }
            }

            //填充数据
            for (i = 0; i < data.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                foreach (var item in dicIndexer)
                {
                    row.CreateCell(item.Key).SetCellValue(item.Value.GetValue(data[i])?.ToString());
                }
            }
        }

        private void ExportToEmptySheet(ISheet sheet, Dictionary<string, string> columns)
        {
            IRow headerRow = sheet.CreateRow(0);
            //设置标题行
            int i = 0;
            foreach (var item in columns)
            {
                headerRow.CreateCell(i).SetCellValue(item.Value);
                i++;
            }

        }

        public IWorkbook CreateWorkbook(Stream stream)
        {
            try
            {
                return new XSSFWorkbook(stream); //07
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HSSFWorkbook(stream); //03
            }

        }

        private object GetCellValue(ICell cell, Type valueType, int i = 0)
        {
            if (valueType == typeof(string))
            {
                return cell.ToString();
            }
            else if (valueType == typeof(int))
            {
                if (cell.ToString() == "")
                    return 0;
                return Convert.ToInt32(cell.ToString());
            }
            else if (valueType == typeof(DateTime))
            {
                return cell.DateCellValue;
            }
            else if (valueType == typeof(DateTime?))
            {
                if (cell.ToString() == "")
                    return null;
                return cell.DateCellValue;
            }
            else if (valueType == typeof(decimal))
            {
                if (cell.ToString() == "")
                {
                    return (decimal)0;
                }
                return Convert.ToDecimal(cell.ToString());
            }
            else
            {
                return cell.ToString();
            }

            //string value = cell.ToString();//cell.ToString()即可获取string Value

            //if (!string.IsNullOrEmpty(value))
            //{
            //    if(valueType == typeof(DateTime))
            //    {
            //        return cell.DateCellValue;
            //        //return Convert.ToDateTime(value);
            //    }
            //    else
            //    {
            //        try { return Convert.ChangeType(value, valueType); }
            //        catch(Exception e)
            //        {
            //            var a = 1;
            //        }                  
            //    }
            //}
            //else
            //{
            //    if(valueType== typeof(decimal))
            //    {
            //        return 0;
            //    }
            //}
            //return value;
        }

        void SetCellStyle(ICell cell)
        {
            var workbook = cell.Sheet.Workbook;
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = false;//换行显示
            style.ShrinkToFit = false;//缩放显示在整个cell内

            IFont font = workbook.CreateFont();
            //font.FontHeightInPoints = 16;
            //font.Boldweight = (short)FontBoldWeight.Bold;
            font.FontName = "微软雅黑";
            style.SetFont(font);//HEAD 样式

            cell.CellStyle = style;
        }

        /// <summary>
        /// 属性和Excel Column索引关系
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>
        private Dictionary<int, PropertyInfo> GetPropertyInfoInderxer<T>(IRow row,
            Dictionary<string, string> columns)
        {
            var dicIndexer = new Dictionary<int, PropertyInfo>();

            if (row == null)
            {
                throw new Exception($"header row is null");
            }

            var props = typeof(T).GetProperties();
            for (int i = 0; i < row.LastCellNum; i++)
            {
                var cellValue = row.Cells[i].StringCellValue;
                if (cellValue != null)
                {
                    foreach (var item in columns)
                    {
                        if (item.Value.ToLower() == cellValue.ToLower())
                        {
                            //匹配属性
                            var prop = props.FirstOrDefault(p => p.Name.ToLower() == item.Key.ToLower());
                            if (prop != null)
                            {
                                dicIndexer.Add(i, prop);
                            }
                            break;
                        }
                    }
                }
            }

            return dicIndexer;
        }

        /// <summary>
        /// 从Excel解析数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelStream"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>F
        public List<T> GetDataList<T>(Stream excelStream, Dictionary<string, string> columns, int headerIndex = 0, bool isValidateHeader = false)
            where T : new()
        {
            if (excelStream == null || columns?.Count == 0)
            {
                return new List<T>();
            }

            var list = new List<T>();

            var workBook = CreateWorkbook(excelStream);
            ISheet sheet = workBook.GetSheetAt(0);

            var headerRow = sheet.GetRow(headerIndex);

            if (isValidateHeader)
            {
                var templateColumns = columns.Select(p => p.Value).ToList();
                ValidateHeader(headerRow, templateColumns);
            }

            var dicIndexer = GetPropertyInfoInderxer<T>(headerRow, columns);

            int lastRowNum = sheet.LastRowNum;

            for (int i = headerIndex + 1; i <= lastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    T obj = new T();
                    foreach (var item in dicIndexer)
                    {
                        try
                        {
                            object val = GetCellValue(row.GetCell(item.Key), item.Value.PropertyType, i);
                            if (val != null)
                            {
                                item.Value.GetReflector().SetValue(obj, val);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                    list.Add(obj);
                }
            }

            return list;
        }

        private void ValidateHeader(IRow headerRow, List<string> templateColumns)
        {
            if (headerRow == null)
            {
                throw new Exception($"header row is null");
            }

            var values = headerRow.Cells.Select(p => p.StringCellValue);

            if (templateColumns.Count != values.Count() || values.Except(templateColumns).Count() > 0)
            {
                throw new Exception("导入表头不符合导入模板");
            }
        }

        public async Task ProcessPaged<T>(IWorkbook workBook, string sheetName, Dictionary<string, string> columns, Func<List<T>, Task> act) where T : new()
        {
            ISheet sheet = workBook.GetSheet(sheetName);

            var dicIndexer = GetPropertyInfoInderxer<T>(sheet.GetRow(0), columns);

            int lastRowNum = sheet.LastRowNum;

            var list = new List<T>();

            for (int i = 1; i <= lastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    T obj = GetOne<T>(dicIndexer, row);
                    list.Add(obj);
                    //if (list.Count % 400 == 0)
                    //{
                    //    await act(list);
                    //    list = new List<T>();
                    //}
                }
            }
            if (list.Count > 0)
            {
                await act(list);
            }
        }

        private T GetOne<T>(Dictionary<int, PropertyInfo> dicIndexer, IRow row) where T : new()
        {
            T obj = new T();
            foreach (var item in dicIndexer)
            {
                try
                {
                    object val = GetCellValue(row.GetCell(item.Key), item.Value.PropertyType);
                    if (val != null)
                    {
                        item.Value.SetValue(obj, val, null);//属性赋值
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return obj;
        }

        /// <summary>
        /// 从Excel解析数据
        /// </summary>
        /// <param name="excelStream"></param>
        /// <param name="columns">DataTable,Excel列映射关系{datatable column,excel column}</param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>
        public DataTable GetDataTable(Stream excelStream, Dictionary<string, string> columns, int headerIndex = 0, bool isMapChecked = false)
        {
            if (excelStream == null || columns?.Count == 0)
            {
                return new DataTable();
            }

            var workBook = CreateWorkbook(excelStream);
            ISheet sheet = workBook.GetSheetAt(0);

            //获取Excel 和 datatable 列映射关系
            var cIndexer = new Dictionary<string, int>();//dt column => excel index
            var headerRow = sheet.GetRow(headerIndex);

            var values = columns.Values.ToArray();//excel columns
            var keys = columns.Keys.ToArray();//datatable columns
            var unmapcolumns = string.Empty;
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                var excelValue = headerRow.GetCell(i)?.StringCellValue;
                var isexist = false;
                for (int j = 0; j < columns.Count; j++)
                {
                    if (excelValue?.ToLower() == values[j].ToLower())
                    {
                        cIndexer.Add(keys[j], i);
                        isexist = true;
                        break;
                    }
                }
                if (!isexist)
                {
                    unmapcolumns += excelValue + ",";
                }
            }
            if (isMapChecked)
            {
                //var row = headerRow.GetCell(2).StringCellValue;
                if (columns.Count != headerRow.LastCellNum)
                {
                    throw new Exception($"导入字段列数与模板不一致");
                }
                if (!string.IsNullOrEmpty(unmapcolumns))
                {
                    throw new Exception($"{unmapcolumns.TrimEnd(',')} 不在导入字段范围内");
                }
            }
            return SheetToDataTable(sheet, cIndexer, headerIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columns">datatable columnName：excel cell index</param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>
        DataTable SheetToDataTable(ISheet sheet, Dictionary<string, int> columns, int headerIndex = 0)
        {
            var dt = new DataTable();
            dt.Columns.AddRange(columns.Select(p => new DataColumn(p.Key)).ToArray());
            int lastRowNum = sheet.LastRowNum;
            for (int i = headerIndex + 1; i <= lastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null)
                {
                    var dr = dt.NewRow();
                    foreach (var item in columns)
                    {
                        dr[item.Key] = row.GetCell(item.Value)?.ToString();
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        /// <summary>
        /// 导出Excel(mutiple sheets）
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="columns">excel列名称</param>
        /// <returns></returns>
        public byte[] ExportDataSet(DataSet ds)
        {
            byte[] data = null;

            IWorkbook workbook = new XSSFWorkbook();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                string sheetName = dt.TableName ?? $"Sheet{i + 1}";
                ISheet sheet = workbook.CreateSheet(sheetName);
                DataTableToSheet(sheet, dt);
            }

            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                data = ms.ToArray();
            }

            workbook = null;
            return data;
        }

        public void ExportList<T>(ISheet worksheet, IEnumerable<T> list, Dictionary<string, string> dict)
        {
            var headerRow = worksheet.CreateRow(0);

            int i = 0;

            foreach (var item in dict)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(item.Key);
                i++;
            }

            i = 1;
            foreach (var item in list)
            {
                var row = worksheet.CreateRow(i);
                int j = 0;
                foreach (var d in dict)
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(d.Value);
                    var v = propertyInfo.GetValue(item);
                    var cell = row.CreateCell(j);
                    if (v != null)
                    {
                        cell.SetCellValue(v.ToString());
                    }
                    else
                    {
                        cell.SetCellValue(string.Empty);
                    }
                    j++;
                }
                i++;
            }
        }

        void AutoFitColumns(ISheet sheet)
        {
            var row = sheet.GetRow(0);
            if (row != null)
            {
                int lastCellNumber = row.LastCellNum > 26 ? 26 : row.LastCellNum;

                for (int columnNum = 0; columnNum < lastCellNumber; columnNum++)
                {
                    int columnWidth = sheet.GetColumnWidth(columnNum) / 256;//获取当前列宽度  
                    for (int rowNum = 0; rowNum < sheet.LastRowNum; rowNum++)
                    {
                        IRow currentRow = sheet.GetRow(rowNum);
                        ICell currentCell = currentRow.GetCell(columnNum);
                        int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;//获取当前单元格的内容宽度  
                        if (columnWidth < length + 1)
                        {
                            columnWidth = length + 1;//若当前单元格内容宽度大于列宽，则调整列宽为当前单元格宽度
                        }
                    }
                    sheet.SetColumnWidth(columnNum, columnWidth * 256);
                }
            }
        }

        void DataTableToSheet(ISheet sheet, DataTable data, bool setStyle = false)
        {
            if (sheet != null && data != null)
            {
                //填充标题
                IRow headerRow = sheet.CreateRow(0);
                int i = 0;

                foreach (DataColumn item in data.Columns)
                {
                    var cell = headerRow.CreateCell(i);
                    cell.SetCellValue(item.ColumnName);
                    SetCellStyle(cell);
                    i++;
                }

                //填充数据
                i = 0;
                foreach (DataRow item in data.Rows)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        var cell = row.CreateCell(j);
                        cell.SetCellValue(data.Rows[i][j].ToString());
                        if (setStyle)
                        {
                            SetCellStyle(cell);
                        }
                    }
                    i++;
                }
            }
        }

        public byte[] ExportDataTable(DataTable dt)
        {
            byte[] data = null;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(dt.TableName);

            DataTableToSheet(sheet, dt);

            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                data = ms.ToArray();
            }

            sheet = null;
            workbook = null;

            return data;
        }


        /// <summary>
        /// 向sheet插入图片
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="workbook"></param>
        /// <param name="fileurl"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private async Task AddCellPicture(ISheet sheet, HSSFWorkbook workbook, string fileurl, int row, int col)
        {
            try
            {
                var imageStream = await _fileUploader.GetFileStreamAsync(fileurl);

                if (imageStream != null && imageStream.Length > 0)
                {
                    byte[] bytes = new byte[imageStream.Length];
                    await imageStream.ReadAsync(bytes);
                    int pictureIdx = workbook.AddPicture(bytes, NPOI.SS.UserModel.PictureType.JPEG);
                    HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                    HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, col, row, col + 1, row + 1);
                    HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class NpoiDataModel
    {
        public string PropertyName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public CellType CellType { get; set; }

        public string Path { get; set; }

        public string Value { get; set; }
    }
    public enum CellType
    {
        Text = 1,
        Image = 2,
        Link = 3,

    }
    public class SheetData<T>
    {
        public List<T> Datas { get; set; }
        public string SheetName { get; set; }
        public Dictionary<string, string> Columns { get; set; }
    }
}
