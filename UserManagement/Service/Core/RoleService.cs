using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core
{
    public interface IRoleService
    {
        Task<PagingModel<RoleViewModel>> GetAll(RoleQueryModel query);
        Task<RoleViewModel> GetById(Guid id);
        Task<Guid> Create(RoleCreateModel model);
        Task<Guid> Update(Guid id, RoleUpdateModel model);
        Task<Guid> Delete(Guid id);
    }

    public class RoleService : IRoleService
    {
        private readonly DataContext _dataContext;
        private ISortHelpers<Role> _sortHelper;
        private readonly IMapper _mapper;

        public RoleService(DataContext dataContext, ISortHelpers<Role> sortHelper, IMapper mapper)
        {
            _dataContext = dataContext;
            _sortHelper = sortHelper;
            _mapper = mapper;
        }

        public async Task<PagingModel<RoleViewModel>> GetAll(RoleQueryModel query)
        {
            try
            {
                var queryData = _dataContext.Roles
                .Where(x => !x.IsDeleted);
                SearchByKeyWord(ref queryData, query.Search);

                var sortData = _sortHelper.ApplySort(queryData, query.OrderBy);

                var data = sortData.ToPagedList(query.PageIndex, query.PageSize);

                var pagingData = new PagingModel<RoleViewModel>()
                {
                    PageIndex = data.CurrentPage,
                    PageSize = data.PageSize,
                    TotalCount = data.TotalCount,
                    pagingData = _mapper.Map<List<Role>, List<RoleViewModel>>(data)
                };
                return pagingData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<RoleViewModel> GetById(Guid id)
        {
            try
            {
                var data = await GetRole(id);
                if (data == null)
                {
                    throw new AppException(ErrorMessage.IdNotExist);
                }
                return _mapper.Map<Role, RoleViewModel>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Create(RoleCreateModel model)
        {
            try
            {
                var existedRole = await _dataContext.Roles
                    .Where(x => !x.IsDeleted && x.Name.Equals(model.Name))
                    .FirstOrDefaultAsync();
                if (existedRole != null)
                {
                    throw new AppException(ErrorMessage.RoleNameExist);
                }
                var data = _mapper.Map<RoleCreateModel, Role>(model);
                await _dataContext.Roles.AddAsync(data);
                await _dataContext.SaveChangesAsync();
                return data.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Update(Guid id, RoleUpdateModel model)
        {
            try
            {
                var data = await GetRole(id);
                if (data == null)
                {
                    throw new AppException(ErrorMessage.IdNotExist);
                }
                var updateData = _mapper.Map(model, data);
                _dataContext.Roles.Update(updateData);
                await _dataContext.SaveChangesAsync();
                return data.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Delete(Guid id)
        {
            var data = await GetRole(id);
            if (data == null)
            {
                throw new AppException(ErrorMessage.IdNotExist);
            }
            data.IsDeleted = true;
            _dataContext.Roles.Update(data);
            await _dataContext.SaveChangesAsync();
            return data.Id;
        }


        // private method
        private void SearchByKeyWord(ref IQueryable<Role> roles, string keyword)
        {
            if (!roles.Any() || string.IsNullOrWhiteSpace(keyword))
                return;
            roles = roles.Where(o => o.Name.ToLower().Contains(keyword.Trim().ToLower()) || o.Name.ToLower().Contains(keyword.Trim().ToLower()));
        }

        private async Task<Role> GetRole(Guid id)
        {
            try
            {
                var data = await _dataContext
                    .Roles
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
