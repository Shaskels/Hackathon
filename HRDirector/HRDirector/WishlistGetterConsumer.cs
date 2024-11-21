using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using MassTransit;

namespace HRDirector
{
    public class WishlistGetterConsumer(DBOperators operators, ILogger<WishlistGetterConsumer> logger) : IConsumer<WishlistCreated>
    {
        public async Task Consume(ConsumeContext<WishlistCreated> context)
        {
            WishlistCreated wishlistIn = context.Message;
            if (wishlistIn != null)
            {
                logger.LogInformation($"Hackathon {wishlistIn.HackathonId} Employee {wishlistIn.Name} {wishlistIn.Id}");
                var employee = new Employee(wishlistIn.Id, wishlistIn.Name, wishlistIn.HackathonId);
                var list = new Wishlist(wishlistIn.Wishlist);
                list.Employee = employee;
                operators.SaveEmployee(employee, list);
                logger.LogInformation("Consumer saved");
            }
        }
    }
}
