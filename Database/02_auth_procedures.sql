/* ==========================================================
   Nitro Rivals - 02_auth_procedures.sql
   Run AFTER standalone.sql (same database).
   Adds: sp_VerifyOtp, sp_ResendOtp, sp_GetPlayerLogin,
         sp_GetAdminLogin, sp_SeedAdminPassword
   Later: paste these into standalone.sql so one script builds all.
   ========================================================== */
USE NitroRivals;
GO

DROP PROCEDURE IF EXISTS sp_VerifyOtp;
DROP PROCEDURE IF EXISTS sp_ResendOtp;
DROP PROCEDURE IF EXISTS sp_GetPlayerLogin;
DROP PROCEDURE IF EXISTS sp_GetAdminLogin;
DROP PROCEDURE IF EXISTS sp_SeedAdminPassword;
GO

/* Error numbers used (mapped to C# exceptions in PlayerRepository):
   50010 no active OTP | 50011 OTP expired | 50012 too many attempts
   50013 wrong OTP     | 50014 player not found | 50015 already verified */
CREATE PROCEDURE sp_VerifyOtp
    @PlayerId INT,
    @OtpHash  VARBINARY(64)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Players WHERE PlayerId = @PlayerId)
        THROW 50014, 'Player not found.', 1;
    IF EXISTS (SELECT 1 FROM Players WHERE PlayerId = @PlayerId AND IsVerified = 1)
        THROW 50015, 'Account is already verified.', 1;

    DECLARE @OtpId INT, @Stored VARBINARY(64), @Expires DATETIME, @Attempts TINYINT;

    SELECT TOP 1 @OtpId = OtpId, @Stored = CodeHash,
                 @Expires = ExpiresAt, @Attempts = Attempts
    FROM OtpCodes
    WHERE PlayerId = @PlayerId AND IsUsed = 0
    ORDER BY OtpId DESC;

    IF @OtpId IS NULL        THROW 50010, 'No active code. Please request a new one.', 1;
    IF @Expires < GETDATE()  THROW 50011, 'This code has expired. Please request a new one.', 1;
    IF @Attempts >= 3        THROW 50012, 'Too many wrong attempts. Please request a new code.', 1;

    IF @Stored <> @OtpHash
    BEGIN
        UPDATE OtpCodes SET Attempts = Attempts + 1 WHERE OtpId = @OtpId;
        THROW 50013, 'Incorrect code.', 1;
    END

    BEGIN TRANSACTION;

        UPDATE OtpCodes SET IsUsed = 1 WHERE OtpId = @OtpId;

        -- UPDATE (not INSERT) so trg_CoinLog records the starting grant
        UPDATE Players SET IsVerified = 1, Coins = Coins + 500
        WHERE PlayerId = @PlayerId;

        DECLARE @StarterCarId INT = (SELECT TOP 1 CarId FROM Cars WHERE Price = 0 ORDER BY CarId);
        INSERT INTO PlayerCars (PlayerId, CarId, IsEquipped)
        VALUES (@PlayerId, @StarterCarId, 1);

        INSERT INTO PlayerLevelProgress (PlayerId, LevelId, IsUnlocked)
        SELECT @PlayerId, LevelId, 1 FROM Levels WHERE LevelNumber = 1;

    COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE sp_ResendOtp
    @PlayerId INT,
    @OtpHash  VARBINARY(64)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM Players WHERE PlayerId = @PlayerId)
        THROW 50014, 'Player not found.', 1;
    IF EXISTS (SELECT 1 FROM Players WHERE PlayerId = @PlayerId AND IsVerified = 1)
        THROW 50015, 'Account is already verified.', 1;

    BEGIN TRANSACTION;
        UPDATE OtpCodes SET IsUsed = 1 WHERE PlayerId = @PlayerId AND IsUsed = 0;
        INSERT INTO OtpCodes (PlayerId, CodeHash, ExpiresAt)
        VALUES (@PlayerId, @OtpHash, DATEADD(MINUTE, 5, GETDATE()));
    COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE sp_GetPlayerLogin @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PlayerId, Username, Email, PwdHash, PwdSalt, IsVerified, Coins
    FROM Players WHERE Username = @Username;
END;
GO

CREATE PROCEDURE sp_GetAdminLogin @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AdminId, Username, PwdHash, PwdSalt
    FROM Admins WHERE Username = @Username;
END;
GO

/* Called by the app at startup. Only changes the admin password while it is
   still the 0x00 placeholder from standalone.sql, so it is safe to call every time. */
CREATE PROCEDURE sp_SeedAdminPassword
    @PwdHash VARBINARY(64),
    @PwdSalt VARBINARY(32)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Admins SET PwdHash = @PwdHash, PwdSalt = @PwdSalt
    WHERE Username = 'admin' AND PwdHash = 0x00;
END;
GO
