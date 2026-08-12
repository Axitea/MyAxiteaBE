CREATE OR ALTER PROCEDURE dbo.sp_My_EnqueueMfaSms
    @PhoneNumber nchar(20),
    @Message varchar(160),
    @RequestedAt datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @RequestedAt = ISNULL(@RequestedAt, GETDATE());

    EXEC dbo.sp_Insert_PZ_MacroVpn_Dispatcher_SmsEmail
        @Macro_Comando = N'N',
        @Uscita_Macro = N'0',
        @Azione = N'K',
        @Code = NULL,
        @IdAutomezzo = NULL,
        @SN_Gestito = N'N',
        @Id_Ope = 73,
        @Data = @RequestedAt,
        @DataGestione = NULL,
        @txt_mess = @Message,
        @Telefono_user = @PhoneNumber,
        @Telefono_ope = NULL;
END;
GO
