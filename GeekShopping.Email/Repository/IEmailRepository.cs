using GeekShopping.OrderAPI.Messages;
using System.Threading.Tasks;

namespace GeekShopping.Email.Repository;

public interface IEmailRepository
{
    Task LogEmail(UpdatePaymentResultMessage message);
}
