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

        public async Task<Guid> CreateUser(UsersRequest userRequest)
        {
            //var existingUser = await _usersRepository.GetByEmail(userEntity.Email);

            //if (existingUser != null)
            //    throw new DuplicateException($"User с email {userEntity.Email} уже существует");

            //List<RoleEntity?> roles = new List<RoleEntity?>();

            //if (userRequest.RoleIds.Count > 0)
            //{
            //    //var rolesResult = await _rolesRepository.GetByList(userRequest.RoleIds);
            //    //if (rolesResult.IsSuccess)
            //    //roles = rolesResult.Value.ToList();
            //    roles = await _rolesRepository.GetByList(userRequest.RoleIds);
            //}

            var passwordHash = _paswordHasher.Hash(userRequest.Password);

            var userEntity = UserEntity.Create(
                userRequest.Id,
                userRequest.Name,
                userRequest.Email,
                passwordHash,
                userRequest.ProfilePhotoURL,
                userRequest.RoleIds).User;

            var newUserId = await _usersRepository.Create(userEntity/*, roles*/);
            return newUserId;
        }

        public async Task<string> LoginUser(string enteredEmail, string enteredPassword)
        {
            var existingUserEntity = await _usersRepository.GetByEmail(enteredEmail);

            if (existingUserEntity == null)
                throw new UnauthorizedException("Пользователь ввел неправильный email или пароль");

            var resultVerify = _paswordHasher.Verify(enteredPassword, existingUserEntity.PasswordHash);

            var rolesByUserResult = await _rolesRepository.GetByUser(existingUserEntity.Id);

            return resultVerify
                ? _jwtProvider.GenerateToken(existingUserEntity, rolesByUserResult)
                : throw new UnauthorizedException("Пользователь ввел неправильный email или пароль");
        }

        public async Task<Guid> DeleteUser(Guid id)
        {
            return await _usersRepository.Delete(id);
        }

        public async Task<List<UserEntity?>> GetAllUsers()
        {
            return await _usersRepository.GetAll();
        }

        public async Task<UserEntity?> GetUserById(Guid id)
        {
            return await _usersRepository.GetById(id);
        }

        public async Task<UserEntity?> GetUserByEmail(string email)
        {
            return await _usersRepository.GetByEmail(email);
        }

        public async Task<Guid> UpdateUser(UserEntity userEntity)
        {
            List<Guid?> roles = new List<Guid?>();

            if (userEntity.RoleIds != null)
            {
                var rolesResult = await _rolesRepository.GetByList(userEntity.RoleIds.ToList());
                
                roles = rolesResult
                    .Select(r => (Guid?)r.Id)
                    .ToList();
            }

            return await _usersRepository.Update(userEntity);
        }

        public async Task<PagedResult<UserEntity>> GetPagedUsersAsync(UserQueryParameters parameters)
        {
            return await _usersRepository.GetPagedAsync(parameters);
        }
    }
}