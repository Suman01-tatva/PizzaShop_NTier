namespace PizzaShop.Service.Interfaces;

public interface IMailService
{
    void SendMail(string toEmail, string body);
}
