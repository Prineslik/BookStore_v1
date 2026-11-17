using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.API.Requests.Book
{
    public record BookChangeOwnersRequest(
        Guid Id,
        List<Guid> OwnersIds);
}
