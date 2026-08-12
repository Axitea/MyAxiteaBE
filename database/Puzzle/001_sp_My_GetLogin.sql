CREATE OR ALTER PROCEDURE dbo.sp_My_GetLogin
    @CodCliente varchar(50),
    @Login varchar(255),
    @Password nvarchar(255),
    @IdApp int = 2
AS
BEGIN
    SET NOCOUNT ON;

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
    WHERE Wus_CodCliente = @CodCliente
      AND Wus_Login = @Login
      AND Wus_NewSatPassword = @Password
      AND Wus_Id_App = @IdApp
      AND ISNULL(Wus_2FA_Locked, 0) = 0
      AND ISNULL(Wus_Abilitato, 'S') = 'S';
END;
GO
