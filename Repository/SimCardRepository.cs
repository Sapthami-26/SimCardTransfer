using SimCardApi.Models;
using SimCardApi.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace SimCardApi.Repositories.Services
{
    public class SimCardRepository : ISimCardRepository
    {
        private readonly string _connectionString;

        public SimCardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<SimCard>> GetSimCardsByEmployeeIdAsync(int employeeId)
        {
            var simCards = new List<SimCard>();
            using (var connection = new SqlConnection(_connectionString))
            {
                // Calls [dbo].[SimCard_GetSimCardsDetailsbyMEmpID]
                using (var cmd = new SqlCommand("SimCard_GetSimCardsDetailsbyMEmpID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MEmpID", employeeId);

                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // Get column ordinals safely, setting to -1 if not found
                        int mSimIdOrdinal = reader.GetOrdinal("MSimID");
                        int mobileNoOrdinal = reader.GetOrdinal("MobileNo");
                        int isSelectedOrdinal = reader.GetOrdinal("IsSelected");
                        int employeeNameOrdinal = -1;

                        try
                        {
                            employeeNameOrdinal = reader.GetOrdinal("Employee");
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // The "Employee" column does not exist in the result set.
                            // employeeNameOrdinal will remain -1.
                        }

                        while (await reader.ReadAsync())
                        {
                            simCards.Add(new SimCard
                            {
                                Id = reader.GetInt32(mSimIdOrdinal),
                                MobileNumber = reader.GetString(mobileNoOrdinal),
                                EmployeeName = employeeNameOrdinal != -1 ? reader.GetString(employeeNameOrdinal) : "N/A", // Safely handle the missing column
                                IsActive = reader.GetBoolean(isSelectedOrdinal)
                            });
                        }
                    }
                }
            }
            return simCards;
        }

        public async Task<IEnumerable<SimCard>> GetTransferDetailsByMasterIdAsync(long masterId)
        {
            var simCards = new List<SimCard>();
            using (var connection = new SqlConnection(_connectionString))
            {
                // Calls [dbo].[SimCard_GetTranferDetailsByMasterID]
                using (var cmd = new SqlCommand("SimCard_GetTranferDetailsByMasterID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCTID", masterId);

                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // Safely get the ordinal for the 'Employee' column.
                        int mobileNoOrdinal = reader.GetOrdinal("MobileNo");
                        int employeeNameOrdinal = -1;
                        try
                        {
                            employeeNameOrdinal = reader.GetOrdinal("Employee");
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // The column is missing, so we'll handle it below.
                        }

                        while (await reader.ReadAsync())
                        {
                            simCards.Add(new SimCard
                            {
                                MobileNumber = reader.GetString(mobileNoOrdinal),
                                EmployeeName = employeeNameOrdinal != -1 ? reader.GetString(employeeNameOrdinal) : "N/A" // Safely handle if 'Employee' column is not returned.
                            });
                        }
                    }
                }
            }
            return simCards;
        }

        public async Task<int> AddSimCardTransferAsync(SimCardTransferDto transferData)
        {
            int masterId = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    // Calls [dbo].[SimCard_InsertSimCardsDetails]
                    using (var cmd = new SqlCommand("SimCard_InsertSimCardsDetails", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MEmpID", transferData.CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@TransferedTo", transferData.TransferToEmployeeId);

                        SqlParameter outputParam = cmd.Parameters.Add("@SCTID", SqlDbType.BigInt);
                        outputParam.Direction = ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        masterId = Convert.ToInt32(outputParam.Value);
                    }

             
                    var dtSimTransferDetails = new DataTable();
                    dtSimTransferDetails.Columns.Add("SCTID", typeof(int));
                    dtSimTransferDetails.Columns.Add("MSimID", typeof(int));

                    foreach (var simId in transferData.SimCardIds)
                    {
                        dtSimTransferDetails.Rows.Add(masterId, simId);
                    }

   
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = "SimCardTransferDetails";
                        bulkCopy.ColumnMappings.Add("SCTID", "SCTID");
                        bulkCopy.ColumnMappings.Add("MSimID", "MSimID");
                        await bulkCopy.WriteToServerAsync(dtSimTransferDetails);
                    }
                    
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return masterId;
        }

        public async Task UpdateSimCardMasterAsync(int simId, int newOwnerEmployeeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Calls [dbo].[Simcard_UpdateSimcard_Master]
                using (var cmd = new SqlCommand("Simcard_UpdateSimcard_Master", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MSimID", simId);
                    cmd.Parameters.AddWithValue("@OwnerEmPIR", newOwnerEmployeeId);
                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}