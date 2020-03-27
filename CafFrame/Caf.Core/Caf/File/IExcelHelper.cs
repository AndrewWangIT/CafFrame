using Caf.Core.DependencyInjection;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core
{
    public interface IExcelHelper: ISingleton
    {
        /// <summary>
        /// 导出Excel columns={propName,columnName}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <returns></returns>
        byte[] Export<T>(List<T> data, Dictionary<string, string> columns, string sheetName = null);

        /// <summary>
        /// 导出Excel columns={propName,columnName}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <param name="func">自定义委托</param>
        /// <returns></returns>
        Task<byte[]> Export<T>(List<T> data, Dictionary<string, string> columns, Func<string, T, Task<NpoiDataModel>> func, string sheetName = null);
        byte[] Export<T>(List<SheetData<T>> sheetDatas);
        /// <summary>
        /// 导出Excel(mutiple sheets） columns={propName,columnName}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns">excel列名称</param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        byte[] ExportDataSet(DataSet data);

        /// <summary>
        /// 导出DataTable到Excel中
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] ExportDataTable(DataTable data);

        /// <summary>
        /// 从Excel解析数据  columns={propName,columnName}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelStream"></param>
        /// <param name="columns">Excel和T属性映射关系{propName,columnName}</param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>
        List<T> GetDataList<T>(Stream excelStream, Dictionary<string, string> columns, int headerIndex = 0, bool isValidateHeader = false) where T : new();
        IWorkbook CreateWorkbook(Stream stream);
        Task ProcessPaged<T>(IWorkbook workbook, string sheetName, Dictionary<string, string> columnMap, Func<List<T>, Task> act) where T : new();
        /// <summary>
        /// 从Excel解析数据
        /// </summary>
        /// <param name="excelStream"></param>
        /// <param name="columns">DataTable,Excel列映射关系{datatable column,excel column}</param>
        /// <param name="headerIndex"></param>
        /// <returns></returns>
        DataTable GetDataTable(Stream excelStream, Dictionary<string, string> columns, int headerIndex = 0, bool isMapChecked = false);
        void ExportList<T>(ISheet worksheet, IEnumerable<T> list, Dictionary<string, string> dict);
    }
}
