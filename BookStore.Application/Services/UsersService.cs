using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.Commands;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Application.Contracts.Users.Queries;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersReadRepository _usersReadRepository;
        private readonly IRolesReadRepository _rolesReadRepository;
        private readonly IPasswordHasher _paswordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUnitOfWork _unitOfWork;

        public UsersService(IUsersRepository usersRepository, IUsersReadRepository usersReadRepository, IPasswordHasher paswordHasher, 
            IJwtProvider jwtProvider, IRolesReadRepository rolesReadRepository, IUnitOfWork unitOfWork)
        {
            _usersRepository = usersRepository;
            _usersReadRepository = usersReadRepository;
            _paswordHasher = paswordHasher;
            _jwtProvider = jwtProvider;
            _rolesReadRepository = rolesReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> CreateUser(RegisterUserCommand registerUserCommand)
        {
            if (await _usersReadRepository.IsExistByEmail(registerUserCommand.Email))
                return Result<Guid>.Failure(Error.Duplicate("Email", registerUserCommand.Email));

            var passwordHash = _paswordHasher.Hash(registerUserCommand.Password);

            var defaultRole = await _rolesReadRepository.GetDefaultRole();

            var userEntity = UserEntity.Create(
                registerUserCommand.Name,
                registerUserCommand.Email,
                passwordHash,
                null,
                null,
                new List<Guid> { defaultRole },
                new List<Guid>());

            var newUserId = await _usersRepository.Create(userEntity/*, roles*/);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(newUserId);
        }

        public async Task<Result<string>> LoginUser(string enteredEmail, string enteredPassword)
        {
            var existingUserEntity = await _usersReadRepository.GetByEmail(enteredEmail);

            if (existingUserEntity == null)
                throw new UnauthorizedException("Пользователь ввел неправильный email или пароль");
            
            //var rolesByUserResult = await _rolesReadRepository.GetByUser(existingUserEntity.Id);

            var resultVerify = _paswordHasher.Verify(enteredPassword, existingUserEntity.PasswordHash);

            if(!resultVerify)
                return Result<string>.Failure(Error.Unauthorized("Пользователь ввел неправильный email или пароль"));

            var token = _jwtProvider.GenerateToken(existingUserEntity.Id, existingUserEntity.Roles);

            return Result<string>.Success(token); 
        }

        public async Task<Result<Guid>> DeleteUser(Guid id)
        {
            var userEntity = await _usersRepository.GetById(id);

            if (userEntity != null)
            {
                await _usersRepository.Delete(userEntity);

                await _unitOfWork.CommitAsync();
            }

            return Result<Guid>.Success(id);
        }

        public async Task<Result<List<UserShortDTO>>> GetAllUsers()
        {
            var users = await _usersReadRepository.GetAll();

            return Result<List<UserShortDTO>>.Success(users);
        }

        public async Task<Result<UserDetailsDTO?>> GetUserById(Guid id)
        {
            var user = await _usersReadRepository.GetById(id);
            if (user == null)
                return Result<UserDetailsDTO?>.Failure(Error.NotFound("User", "Id", id));

            return Result<UserDetailsDTO?>.Success(user);
        }

        public async Task<Result<UserAuthDTO?>> GetUserByEmail(string email)
        {
            var user = await _usersReadRepository.GetByEmail(email);
            
            return Result<UserAuthDTO?>.Success(user); 
        }

        public async Task<Result<Guid>> UpdateUserProfile(UpdateUserProfileCommand updateProfileCommand)
        {
            var user = await _usersRepository.GetById(updateProfileCommand.Id);

            if (user == null)
                return Result<Guid>.Failure(Error.NotFound("User", "Id", updateProfileCommand.Id));//throw new NotFoundException("User"/*, updateProfileCommand.Id*/);

            user.Rename(updateProfileCommand.Name);
            user.ChangeEmail(updateProfileCommand.Email);
            user.ChangeProfilePhotoUrl(updateProfileCommand.ProfilePhotoURL);

            var updatedUserId = await _usersRepository.Update(user);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(updatedUserId);
        }

        public async Task<Result<PagedResult<UserShortDTO>>> GetPagedUsersAsync(UserQueryParameters parameters)
        {
            var pagedUsers = await _usersReadRepository.GetPagedAsync(parameters);

            return Result<PagedResult<UserShortDTO>>.Success(pagedUsers);
        }
    }
}