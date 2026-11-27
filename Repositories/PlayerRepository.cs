using System.Data;
using Microsoft.Data.SqlClient;
using NitroRivals.Exceptions;
using NitroRivals.Models;

namespace NitroRivals.Repositories;

public class PlayerRepository
{
    public int Register(string username, string email, byte[] hash, byte[] salt, byte[] otpHash)
    {
        try
        {
            object? id = DbHelper.ExecuteScalar("sp_RegisterPlayer",
                DbHelper.Param("@Username", SqlDbType.NVarChar, username, 50),
                DbHelper.Param("@Email", SqlDbType.NVarChar, email, 100),
                DbHelper.Param("@PwdHash", SqlDbType.VarBinary, hash, 64),
                DbHelper.Param("@PwdSalt", SqlDbType.VarBinary, salt, 32),
                DbHelper.Param("@OtpHash", SqlDbType.VarBinary, otpHash, 64));
            return Convert.ToInt32(id);
        }
        catch (SqlException ex) when (ex.Number is 50001 or 50002)
        {
            throw new DuplicateAccountException(ex.Message);
        }
    }

    public void VerifyOtp(int playerId, byte[] otpHash)
    {
        try
        {
            DbHelper.ExecuteNonQuery("sp_VerifyOtp",
                DbHelper.Param("@PlayerId", SqlDbType.Int, playerId),
                DbHelper.Param("@OtpHash", SqlDbType.VarBinary, otpHash, 64));
        }
        catch (SqlException ex) when (ex.Number is 50010 or 50011)
        {
            throw new OtpExpiredException(ex.Message);
        }
        catch (SqlException ex) when (ex.Number is 50012 or 50013 or 50014 or 50015)
        {
            throw new InvalidOtpException(ex.Message);
        }
    }

    public void ResendOtp(int playerId, byte[] otpHash)
    {
        try
        {
            DbHelper.ExecuteNonQuery("sp_ResendOtp",
                DbHelper.Param("@PlayerId", SqlDbType.Int, playerId),
                DbHelper.Param("@OtpHash", SqlDbType.VarBinary, otpHash, 64));
        }
        catch (SqlException ex) when (ex.Number is 50014 or 50015)
        {
            throw new InvalidOtpException(ex.Message);
        }
    }

    public LoginRecord? GetPlayerLogin(string username)
    {
        DataTable t = DbHelper.ExecuteTable("sp_GetPlayerLogin",
            DbHelper.Param("@Username", SqlDbType.NVarChar, username, 50));
        if (t.Rows.Count == 0) return null;
        DataRow r = t.Rows[0];
        return new LoginRecord(
            (int)r["PlayerId"], (string)r["Username"], (string)r["Email"],
            (byte[])r["PwdHash"], (byte[])r["PwdSalt"],
            (bool)r["IsVerified"], (int)r["Coins"]);
    }

    public LoginRecord? GetAdminLogin(string username)
    {
        DataTable t = DbHelper.ExecuteTable("sp_GetAdminLogin",
            DbHelper.Param("@Username", SqlDbType.NVarChar, username, 50));
        if (t.Rows.Count == 0) return null;
        DataRow r = t.Rows[0];
        return new LoginRecord(
            (int)r["AdminId"], (string)r["Username"], "",
            (byte[])r["PwdHash"], (byte[])r["PwdSalt"], true, 0);
    }

    public void SeedAdminPassword(byte[] hash, byte[] salt)
    {
        DbHelper.ExecuteNonQuery("sp_SeedAdminPassword",
            DbHelper.Param("@PwdHash", SqlDbType.VarBinary, hash, 64),
            DbHelper.Param("@PwdSalt", SqlDbType.VarBinary, salt, 32));
    }
}
