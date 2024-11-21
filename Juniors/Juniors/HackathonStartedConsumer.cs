﻿using AllForTheHackathon.Domain;
using MassTransit;

namespace Juniors
{
    public class HackathonStartedConsumer : IConsumer<Hackathon>
    {
        WishlistSender _wishlistSender;
        public HackathonStartedConsumer(WishlistSender wishlistSender)
        {
            _wishlistSender = wishlistSender;
        }
        public async Task Consume(ConsumeContext<Hackathon> context)
        {
            Hackathon hackathon = context.Message;
            if (hackathon != null)
            {
                _wishlistSender.SendWishlist(hackathon.Id);
            }
        }
    }
}
