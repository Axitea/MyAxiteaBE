CREATE OR ALTER PROCEDURE dbo.sp_My_SetMfaCode
    @IdUtente bigint,
    @IdApp int = 2,
    @Code nvarchar(6),
    @RequestedAtUtc datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @RequestedAtUtc = ISNULL(@RequestedAtUtc, GETUTCDATE());

    UPDATE dbo.T_WEB_UTENTI
    SET Wus_2FA_Code = @Code,
        Wus_2FA_LastRequest = @RequestedAtUtc
    WHERE Wus_IdUtente = @IdUtente
      AND Wus_Id_App = @IdApp
      AND ISNULL(Wus_2FA_Locked, 0) = 0
      AND ISNULL(Wus_Abilitato, 'S') = 'S';

    IF @@ROWCOUNT = 0
    BEGIN
        THROW 51001, 'MYA MFA code not saved: user not found, disabled, locked, or app mismatch.', 1;
    END;
END;
GO
