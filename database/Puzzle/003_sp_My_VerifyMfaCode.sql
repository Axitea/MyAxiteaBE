CREATE OR ALTER PROCEDURE dbo.sp_My_VerifyMfaCode
    @IdUtente bigint,
    @IdApp int = 2,
    @Code nvarchar(6),
    @NowUtc datetime = NULL,
    @TtlMinutes int = 5
AS
BEGIN
    SET NOCOUNT ON;

    SET @NowUtc = ISNULL(@NowUtc, GETUTCDATE());

    SELECT
        CAST(Wus_IdUtente AS bigint) AS Wus_IdUtenteToken,
        CAST(ISNULL(CAST(Wus_IdDipendente AS bigint), Wus_IdUtente) AS bigint) AS Wus_IdUtente,
        Wus_Login,
        Wus_Nome,
        Wus_Permessi,
        Wus_Email,
        Wus_CliRagSociale,
        Wus_IdScenario,
        Wus_Lingua,
        Wus_2FA_NumCell,
        Wus_2FA_IsActive,
        ISNULL(Wus_2FA_Locked, 0) AS Wus_2FA_Locked
    FROM dbo.T_WEB_UTENTI
    WHERE Wus_IdUtente = @IdUtente
      AND Wus_Id_App = @IdApp
      AND Wus_2FA_Code = @Code
      AND Wus_2FA_IsActive = 'S'
      AND ISNULL(Wus_2FA_Locked, 0) = 0
      AND ISNULL(Wus_Abilitato, 'S') = 'S'
      AND Wus_2FA_LastRequest >= DATEADD(MINUTE, -@TtlMinutes, @NowUtc);

    IF @@ROWCOUNT = 1
    BEGIN
        UPDATE dbo.T_WEB_UTENTI
        SET Wus_2FA_Code = NULL
        WHERE Wus_IdUtente = @IdUtente
          AND Wus_Id_App = @IdApp
          AND Wus_2FA_Code = @Code;
    END;
END;
GO
