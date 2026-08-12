SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_WARNINGS ON;
SET ANSI_PADDING ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;

DECLARE @SourceCodCliente varchar(50) = '8888888888';
DECLARE @SourceLogin varchar(255) = 'giulio.caruso';
DECLARE @TargetCodCliente varchar(50) = '8888888888';
DECLARE @TargetLogin varchar(255) = 'luca.pesola';
DECLARE @TargetName varchar(255) = 'Luca Pesola';
DECLARE @TargetEmail varchar(255) = '';
DECLARE @AppId int = 2;

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID('dbo.T_WEB_UTENTI') IS NULL
BEGIN
    THROW 51000, 'Tabella dbo.T_WEB_UTENTI non trovata.', 1;
END;

IF (
    SELECT COUNT(1)
    FROM dbo.T_WEB_UTENTI
    WHERE Wus_CodCliente = @SourceCodCliente
      AND Wus_Login = @SourceLogin
      AND Wus_Id_App = @AppId
) <> 1
BEGIN
    THROW 51001, 'Account sorgente giulio.caruso non trovato o non univoco.', 1;
END;

IF EXISTS (
    SELECT 1
    FROM dbo.T_WEB_UTENTI
    WHERE Wus_CodCliente = @TargetCodCliente
      AND Wus_Login = @TargetLogin
      AND Wus_Id_App = @AppId
)
BEGIN
    THROW 51002, 'Account target luca.pesola gia'' presente: nessuna modifica eseguita.', 1;
END;

DECLARE @insertColumns nvarchar(max);
DECLARE @selectColumns nvarchar(max);
DECLARE @sql nvarchar(max);

SELECT @insertColumns = STUFF((
    SELECT ', ' + QUOTENAME(c.name)
    FROM sys.columns c
    WHERE c.object_id = OBJECT_ID('dbo.T_WEB_UTENTI')
      AND COLUMNPROPERTY(c.object_id, c.name, 'IsIdentity') = 0
      AND COLUMNPROPERTY(c.object_id, c.name, 'IsComputed') = 0
    ORDER BY c.column_id
    FOR XML PATH(''), TYPE
).value('.', 'nvarchar(max)'), 1, 2, '');

SELECT @selectColumns = STUFF((
    SELECT ', ' + CASE c.name
        WHEN 'Wus_CodCliente' THEN '@TargetCodCliente'
        WHEN 'Wus_Login' THEN '@TargetLogin'
        WHEN 'Wus_Nome' THEN '@TargetName'
        WHEN 'Wus_Email' THEN '@TargetEmail'
        WHEN 'Wus_Logged' THEN 'CAST(''N'' AS char(1))'
        WHEN 'Wus_2FA_IsActive' THEN 'CAST(N''S'' AS nchar(2))'
        WHEN 'Wus_2FA_LastRequest' THEN 'CAST(NULL AS datetime)'
        WHEN 'Wus_2FA_Code' THEN 'CAST(NULL AS nvarchar(12))'
        WHEN 'Wus_2FA_Locked' THEN 'CAST(0 AS bit)'
        WHEN 'Wus_2FA_NumCell' THEN 'CAST(NULL AS nvarchar(40))'
        WHEN 'Wus_Id_App' THEN '@AppId'
        ELSE 's.' + QUOTENAME(c.name)
    END
    FROM sys.columns c
    WHERE c.object_id = OBJECT_ID('dbo.T_WEB_UTENTI')
      AND COLUMNPROPERTY(c.object_id, c.name, 'IsIdentity') = 0
      AND COLUMNPROPERTY(c.object_id, c.name, 'IsComputed') = 0
    ORDER BY c.column_id
    FOR XML PATH(''), TYPE
).value('.', 'nvarchar(max)'), 1, 2, '');

BEGIN TRANSACTION;

SET @sql = N'
    INSERT INTO dbo.T_WEB_UTENTI (' + @insertColumns + N')
    SELECT ' + @selectColumns + N'
    FROM dbo.T_WEB_UTENTI s
    WHERE s.Wus_CodCliente = @SourceCodCliente
      AND s.Wus_Login = @SourceLogin
      AND s.Wus_Id_App = @AppId;';

EXEC sp_executesql
    @sql,
    N'@SourceCodCliente varchar(50), @SourceLogin varchar(255), @TargetCodCliente varchar(50), @TargetLogin varchar(255), @TargetName varchar(255), @TargetEmail varchar(255), @AppId int',
    @SourceCodCliente = @SourceCodCliente,
    @SourceLogin = @SourceLogin,
    @TargetCodCliente = @TargetCodCliente,
    @TargetLogin = @TargetLogin,
    @TargetName = @TargetName,
    @TargetEmail = @TargetEmail,
    @AppId = @AppId;

COMMIT TRANSACTION;

SELECT
    Wus_IdUtente,
    Wus_CodCliente,
    Wus_Login,
    Wus_Nome,
    Wus_Email,
    Wus_2FA_NumCell,
    Wus_2FA_IsActive,
    Wus_2FA_Locked,
    Wus_Id_App
FROM dbo.T_WEB_UTENTI
WHERE Wus_CodCliente = @TargetCodCliente
  AND Wus_Login = @TargetLogin
  AND Wus_Id_App = @AppId;
