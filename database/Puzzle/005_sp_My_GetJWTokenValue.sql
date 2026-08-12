CREATE OR ALTER PROCEDURE dbo.sp_My_GetJWTokenValue
    @RefreshToken nvarchar(200),
    @IdApp int = 2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        idToken,
        tokenValue,
        username,
        dateToken,
        permission,
        refreshToken,
        idApp
    FROM dbo.JWTokens
    WHERE refreshToken = @RefreshToken
      AND idApp = @IdApp;
END;
GO
