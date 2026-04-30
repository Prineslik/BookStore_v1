using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class UsersService : IUsersService
    {
        public IUsersRepository _usersRepository { get; set; }
        public IPasswordHasher _paswordHasher { get; set; }

        public UsersService(IUsersRepository usersRepository, IPasswordHasher paswordHasher)
        {
            _usersRepository = usersRepository;
            _paswordHasher = paswordHasher;
        }

        public async Task<Result<Guid>> CreateUser(/*UserEntity*/ UsersRequest userRequest)
        {
            var userResult = await _usersRepository.GetByEmail(userRequest.Email);

            if (userResult.IsFailure)
                return Result<Guid>.Failure(Error.Duplicate("Email", userRequest.Email));
            
            var (userEntity, error) = UserEntity.Create(
                Guid.NewGuid(),
                userRequest.Name,
                userRequest.Email,
                _paswordHasher.Hash(userRequest.Password),
                userRequest.ProfilePhotoURL);

            if (!string.IsNullOrEmpty(error))
                return Result<Guid>.Failure(Error.Validation(error));

            var newUserId = await _usersRepository.Create(userEntity);

            return Result<Guid>.Success(newUserId);
        }

        public Task<Guid> DeleteUser(Guid id)
        {
            return _usersRepository.Delete(id);
        }

        public Task<Result<List<UserEntity>>> GetAllUsers()
        {
            return _usersRepository.GetAll();
        }

        public Task<Result<UserEntity?>> GetUserById(Guid id)
        {
            return _usersRepository.GetById(id);
        }

        public Task<Result<List<UserEntity?>>> GetUsersByEmail(string email)
        {
            return _usersRepository.GetByEmail(email);
        }

        public Task<Guid> UpdateUser(UserEntity userEntity)
        {
            return _usersRepository.Update(userEntity);
        }

        public Task<PagedResult<UserEntity>> GetPagedBookAsync(UserQueryParameters parameters)
        {
            return _usersRepository.GetPagedAsync(parameters);
        }
    }
}
