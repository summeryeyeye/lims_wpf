using Lims.Common.Dtos;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Data;

namespace Lims.WebAPI.Singleton
{
    public interface ITaskStatusService
    {
        Task<string> GetCurrentTaskStatusAsync();
    }
    public class TaskStatusService : ITaskStatusService
    {
        private readonly string _connectionString;

        public TaskStatusService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("POSTGRESQL")!;
        }

        public async Task<string> GetCurrentTaskStatusAsync()
        {
            string jsonData = string.Empty;
            await using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    TaskCountDto taskCountDto = new TaskCountDto();
                    var cmdStr = "SELECT tester,Count(itemid) count FROM itemmodel WHERE testprogress=101 Group BY tester;SELECT tester,Count(itemid) count FROM itemmodel WHERE testprogress=103 Group BY tester;SELECT tester,Count(itemid) count FROM itemmodel WHERE testprogress=102 Group BY tester;SELECT receivername tester, COUNT(id) count FROM loggermodel WHERE isreaded=FALSE AND loglevel=3 Group BY receivername;";
                    if (con.State != ConnectionState.Open)
                        await con.OpenAsync();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(cmdStr, con))
                    {
                        await using (Npgsql.NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            taskCountDto.MyReceivableTasks = ConvertDataReaderToDataTable(reader);
                            if (reader.NextResult())
                                taskCountDto.MyTestingTasks = ConvertDataReaderToDataTable(reader);
                            if (reader.NextResult())
                                taskCountDto.MyReturnedTasks = ConvertDataReaderToDataTable(reader);
                            if (reader.NextResult())
                                taskCountDto.MyUnreadLogs = ConvertDataReaderToDataTable(reader);
                        }
                    }
                    await using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT Count(itemid) count FROM itemmodel WHERE testprogress<104;SELECT COUNT(samplecode) count FROM (SELECT samplecode FROM itemmodel GROUP BY samplecode HAVING SUM(CASE WHEN testprogress <> 104 THEN 1 ELSE 0 END) = 0 ) t;SELECT COUNT(samplecode) count  FROM (SELECT samplecode FROM itemmodel GROUP BY samplecode HAVING SUM(CASE WHEN testprogress <> 105 THEN 1 ELSE 0 END) = 0 ) t;SELECT COUNT(samplecode) count  FROM (SELECT samplecode FROM itemmodel GROUP BY samplecode HAVING SUM(CASE WHEN testprogress <> 106 THEN 1 ELSE 0 END) = 0 ) t", con))
                    {
                        var reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                            taskCountDto.unFinishedTasks = reader.GetInt32(0);
                        if (await reader.NextResultAsync())
                            while (await reader.ReadAsync())
                                taskCountDto.firstCheckTasks = reader.GetInt32(0);
                        if (await reader.NextResultAsync())
                            while (await reader.ReadAsync())
                                taskCountDto.sencondCheckTasks = reader.GetInt32(0);
                        if (await reader.NextResultAsync())
                            while (await reader.ReadAsync())
                                taskCountDto.thirdCheckTasks = reader.GetInt32(0);
                    }
                    jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(taskCountDto);

                    //await _hubContext.Clients.All.SendAsync("ReceiveTaskChange", json);
                }
                catch (Exception)
                {

                    //throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return jsonData;
        }
        //将DataReader 转化为 DataTable
        static DataTable ConvertDataReaderToDataTable(NpgsqlDataReader dataReader)
        {
            DataTable datatable = new DataTable();
            try
            {    //添加表的列类型和列名
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    DataColumn column = new DataColumn();
                    column.DataType = dataReader.GetFieldType(i);
                    column.ColumnName = dataReader.GetName(i);
                    datatable.Columns.Add(column);
                }

                //添加表的数据
                while (dataReader.Read())
                {
                    DataRow row = datatable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row[i] = dataReader[i].ToString();
                    }
                    datatable.Rows.Add(row);
                    row = null;
                }
                //dataReader.Close();
                return datatable;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
