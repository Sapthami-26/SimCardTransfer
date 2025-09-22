using SimCardApi.Models;
using SimCardApi.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

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
                        while (await reader.ReadAsync())
                        {
                            simCards.Add(new SimCard
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("MSimID")),
                                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNo")),
                                EmployeeName = reader.GetString(reader.GetOrdinal("Employee")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsSelected"))
                            });
                        }
                    }
                }
            }
            return simCards;
        }

        public async Task<IEnumerable<SimCard>> GetTransferDetailsByMasterIdAsync(int masterId)
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
                        while (await reader.ReadAsync())
                        {
                            simCards.Add(new SimCard
                            {
                                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNo")),
                                EmployeeName = reader.GetString(reader.GetOrdinal("Employee"))
                            });
                        }
                    }
                }
            }
            return simCards;
        }

        public async Task<int> AddSimCardTransferAsync(SimCardTransferDto transferData)
        {
            long masterId = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();
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

                        masterId = Convert.ToInt64(outputParam.Value);
                    }

                    // Calls [dbo].[SimCard_InsertSimCardsTransferDetails]
                    foreach (var simId in transferData.SimCardIds)
                    {
                        using (var cmd = new SqlCommand("SimCard_InsertSimCardsTransferDetails", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SCTID", masterId);
                            cmd.Parameters.AddWithValue("@MSimID", simId);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return (int)masterId;
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