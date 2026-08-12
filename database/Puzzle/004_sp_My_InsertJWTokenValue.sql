CREATE OR ALTER PROCEDURE dbo.sp_My_InsertJWTokenValue
    @TokenValue nvarchar(max),
    @Username nvarchar(30),
    @Permission int = 0,
    @RefreshToken nvarchar(200),
    @IdApp int = 2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.JWTokens
    (
        tokenValue,
        username,
        dateToken,
        permission,
        refreshToken,
        idApp
    )
    VALUES
    (
        @TokenValue,
        @Username,
        GETUTCDATE(),
        ISNULL(@Permission, 0),
        @RefreshToken,
        @IdApp
    );
END;
GO
