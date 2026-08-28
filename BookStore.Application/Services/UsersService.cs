using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _paswordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRolesRepository _rolesRepository;

        public UsersService(IUsersRepository usersRepository, IPasswordHasher paswordHasher, 
            IJwtProvider jwtProvider, IRolesRepository rolesRepository)
        {
            _usersRepository = usersRepository;
            _paswordHasher = paswordHasher;
            _jwtProvider = jwtProvider;
            _rolesRepository = rolesRepository;
        }

        public async Task<Guid> CreateUser(UserEntity userEntity)
        {
            var existingUser = await _usersRepository.GetByEmail(userEntity.Email);

            if (existingUser != null)
                throw new DuplicateException($"User с email {userEntity.Email} уже существует");

            List<RoleEntity?> roles = new List<RoleEntity?>();

            //if (userRequest.RoleIds.Count > 0)
            //{
            //    //var rolesResult = await _rolesRepository.GetByList(userRequest.RoleIds);
            //    //if (rolesResult.IsSuccess)
            //    //roles = rolesResult.Value.ToList();
            //    roles = await _rolesRepository.GetByList(userRequest.RoleIds);
            //}

            var newUserId = await _usersRepository.Create(userEntity/*, roles*/);
            return newUserId;
        }

        public async Task<string> LoginUser(string enteredEmail, string enteredPassword)
        {
            var existingUserEntity = await _usersRepository.GetByEmail(enteredEmail);

            if (existingUserEntity == null)
                throw new UnauthorizedException("Пользователь ввел неправильный email или пароль");
                //return Result<string>.Failure(Error.NotFound("User","Email", userRequest.Email));

            var resultVerify = _paswordHasher.Verify(enteredPassword, existingUserEntity.PasswordHash);

            var rolesByUserResult = await _rolesRepository.GetByUser(existingUserEntity.Id);

            return resultVerify
                ? _jwtProvider.GenerateToken(existingUserEntity, rolesByUserResult)
                : throw new UnauthorizedException("Пользователь ввел неправильный email или пароль");
            /*return resultVerify 
                ? Result<string>.Success(_jwtProvider.GenerateToken(resultUserEntity.Value, rolesByUserResult.Value))
                : Result<string>.Failure(Error.Unexpected("Ошибка при аутентификации"));*/
        }

        public async Task<Guid> DeleteUser(Guid id)
        {
            if(await _usersRepository.IsExist(id))
                throw new NotFoundException("User", id);

            return await _usersRepository.Delete(id);
                //: Result<Guid>.Failure(Error.NotFound("User", "Id", id));
        }

        public Task<List<UserEntity?>> GetAllUsers()
        {
            return _usersRepository.GetAll();
        }

        public Task<UserEntity?> GetUserById(Guid id)
        {
            return _usersRepository.GetById(id);
        }

        public Task<UserEntity?> GetUsersByEmail(string email)
        {
            return _usersRepository.GetByEmail(email);
        }

        public async Task<Guid> UpdateUser(UserEntity userEntity)
        {
            var existingUserEntity = await _usersRepository.GetById(userEntity.Id);

            if (existingUserEntity == null)
                throw new NotFoundException("User", userEntity.Id);

            List<Guid?> roles = new List<Guid?>();

            if (userEntity.RoleIds != null)
            {
                var rolesResult = await _rolesRepository.GetByList(userEntity.RoleIds.ToList());
                
                roles = rolesResult.Select(r => (Guid?)r.Id).ToList();
            }

            return await _usersRepository.Update(userEntity);
        }

        public async Task<PagedResult<UserEntity>> GetPagedUsersAsync(UserQueryParameters parameters)
        {
            return await _usersRepository.GetPagedAsync(parameters);
        }
    }
}
