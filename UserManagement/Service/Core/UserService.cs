using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Service.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Core
{
    public interface IUserService
    {
        Task<JWTToken> Login(UserRequest model);
        Task<Guid> Register(UserCreateModel model);
        Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query);
        Task<UserViewModel> GetById(Guid id);
        Task<Guid> Create(UserCreateModel userCreateModel);
        Task<Guid> Update(Guid id, UserUpdateModel model);
        Task<Guid> Delete(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private ISortHelpers<User> _sortHelper;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(DataContext dataContext, ISortHelpers<User> sortHelper, IMapper mapper, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _sortHelper = sortHelper;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<JWTToken> Login(UserRequest model)
        {
            try
            {
                var user = await _dataContext.Users
                .Where(x => !x.IsDeleted && x.UserName == model.Username)
                .FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new AppException(ErrorMessage.InvalidAccount);
                }
                if(!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    throw new AppException(ErrorMessage.InvalidAccount);
                }
                var userRoles = await _dataContext.RoleAssignments
                    .Where(x => x.UserId.Equals(user.Id))
                    .Join(_dataContext.Roles,
                        x => x.RoleId,
                        y => y.Id,
                        (x, y) => new UserRole
                        {
                            UserId = user.Id,
                            RoleId = y.Id,
                            RoleName = y.Name
                        })
                    .Join(_dataContext.Users,
                        x => x.UserId,
                        y => y.Id,
                        (x, y) => new UserRole
                        {
                            UserId = y.Id,
                            RoleId = x.RoleId,
                            FullName = y.FullName,
                            UserName = y.UserName,
                            RoleName = x.RoleName
                        })
                    .Select(x => x.RoleName)
                    .ToListAsync();
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model?.Username ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach(var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var token = GenerateToken(authClaims, _configuration.GetSection("JWT").Get<JwtModel>());
                return token;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }

        }

        public async Task<Guid> Register(UserCreateModel model)
        {
            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    // new user register
                    var existedUser = await _dataContext.Users
                    .Where(x => !x.IsDeleted && x.UserName.Equals(model.UserName) || x.Phone.Equals(model.Phone))
                    .FirstOrDefaultAsync();
                    if (existedUser != null)
                    {
                        throw new AppException(ErrorMessage.UserNameExist + " or " + ErrorMessage.PhoneExist);
                    }
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    model.Password = passwordHash;
                    var dataUser = _mapper.Map<UserCreateModel, User>(model);
                    await _dataContext.Users.AddAsync(dataUser);
                    await _dataContext.SaveChangesAsync();

                    // role-base User
                    var getUserRole = await _dataContext.Roles
                        .Where(x => !x.IsDeleted && x.Name == "User")
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync();
                    RoleAssignmentCreateModel addRoleDefault = new RoleAssignmentCreateModel();
                    addRoleDefault.UserId = dataUser.Id;
                    addRoleDefault.RoleId = getUserRole;
                    var dataRole = _mapper.Map<RoleAssignmentCreateModel, RoleAssignment>(addRoleDefault);
                    await _dataContext.RoleAssignments.AddAsync(dataRole);
                    await _dataContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return dataUser.Id;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await transaction.RollbackAsync();
                    throw new AppException(e.Message);
                }
            }
        }

        public async Task<PagingModel<UserViewModel>> GetAll(UserQueryModel query)
        {
            try
            {
                var queryData = _dataContext.Users
                .Where(x => !x.IsDeleted);
                SearchByKeyWord(ref queryData, query.Search);

                var sortData = _sortHelper.ApplySort(queryData, query.OrderBy);

                var data = sortData.ToPagedList(query.PageIndex, query.PageSize);

                var pagingData = new PagingModel<UserViewModel>()
                {
                    PageIndex = data.CurrentPage,
                    PageSize = data.PageSize,
                    TotalCount = data.TotalCount,
                    pagingData = _mapper.Map<List<User>, List<UserViewModel>>(data)
                };
                return pagingData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Create(UserCreateModel model)
        {
            try
            {
                var existedUser = await _dataContext.Users
                    .Where(x => !x.IsDeleted && x.UserName.Equals(model.UserName) && x.Phone.Equals(model.Phone))
                    .FirstOrDefaultAsync();
                if (existedUser != null)
                {
                    throw new AppException(ErrorMessage.UserNameExist + " or " + ErrorMessage.PhoneExist);
                }
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                model.Password = passwordHash;
                var data = _mapper.Map<UserCreateModel, User>(model);
                await _dataContext.Users.AddAsync(data);
                await _dataContext.SaveChangesAsync();
                return data.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }
        public async Task<UserViewModel> GetById(Guid id)
        {
            try
            {
                var data = await GetUser(id);
                if (data == null)
                {
                    throw new AppException(ErrorMessage.IdNotExist);
                }
                return _mapper.Map<User, UserViewModel>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Update(Guid id, UserUpdateModel model)
        {
            try
            {
                var checkExistUser = await GetUser(id);
                if (checkExistUser == null)
                {
                    throw new AppException(ErrorMessage.IdNotExist);
                }
                var updateData = _mapper.Map(model, checkExistUser);
                _dataContext.Users.Update(updateData);
                await _dataContext.SaveChangesAsync();
                return checkExistUser.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Delete(Guid id)
        {
            var checkExistUser = await GetUser(id);
            if (checkExistUser == null)
            {
                throw new AppException(ErrorMessage.IdNotExist);
            }
            checkExistUser.IsDeleted = true;
            _dataContext.Users.Update(checkExistUser);
            await _dataContext.SaveChangesAsync();
            return checkExistUser.Id;
        }



        // private method
        private JWTToken GenerateToken(IEnumerable<Claim> claims, JwtModel? jwtModel)
        {
            var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtModel?.Secret ?? ""));
            var expirationTime = DateTime.UtcNow.AddHours(3);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtModel?.ValidIssuer,
                Audience = jwtModel?.ValidAudience,
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var jwtToken = new JWTToken
            {
                TokenString = tokenString,
                ExpiresInMilliseconds = (long)(expirationTime - DateTime.UtcNow).TotalMilliseconds
            };
            return jwtToken;
        }

        private void SearchByKeyWord(ref IQueryable<User> users, string keyword)
        {
            if (!users.Any() || string.IsNullOrWhiteSpace(keyword))
                return;
            users = users.Where(o => o.FullName.ToLower().Contains(keyword.Trim().ToLower()) || o.UserName.ToLower().Contains(keyword.Trim().ToLower()));
        }

        private async Task<User> GetUser(Guid id)
        {
            try
            {
                var data = await _dataContext
                    .Users
                    .Where(x => !x.IsDeleted && x.Id == id)
                    .SingleOrDefaultAsync();
                if (data == null) throw new AppException(ErrorMessage.IdNotExist);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }
    }
}
