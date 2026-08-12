namespace MYA.Business.Security;

public interface IMfaCodeGenerator
{
    string Generate(int length);
}
