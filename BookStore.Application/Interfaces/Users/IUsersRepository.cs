using BookStore.Application.Contracts;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Users
{
    public interface IUsersRepository
    {
        Task<Guid> Create(UserEntity newUserEntity);
        Task<Guid> Delete(UserEntity userEntity);
        Task<Guid> Update(UserEntity userEntity);
        Task<UserEntity?> GetById(Guid id);
    }
}