using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core
{
    public interface IRoleAssignmentService
    {
        Task<List<RoleAssignmentViewModel>> GetAll();
        Task<Guid> Create(RoleAssignmentCreateModel model);
        Task<Guid> Update(Guid id, RoleAssignmentUpdateModel model);
        Task<Guid> Delete(Guid id);
    }
    public class RoleAssignmentService : IRoleAssignmentService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public RoleAssignmentService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<RoleAssignmentViewModel>> GetAll()
        {
            try
            {
                var queryData = await _dataContext.RoleAssignments
                .Where(x => !x.IsDeleted)
                .ToListAsync();

                var data = _mapper.Map<List<RoleAssignment>, List<RoleAssignmentViewModel>>(queryData);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Create(RoleAssignmentCreateModel model)
        {
            try
            {
                var existedUserRole = await _dataContext.RoleAssignments
                    .Where(x => !x.IsDeleted && x.UserId.Equals(model.UserId) && x.RoleId.Equals(model.RoleId))
                    .FirstOrDefaultAsync();
                if(existedUserRole != null)
                {
                    throw new Exception(ErrorMessage.RoleNameExist);
                }
                var data = _mapper.Map<RoleAssignmentCreateModel, RoleAssignment>(model);
                await _dataContext.RoleAssignments.AddAsync(data);
                await _dataContext.SaveChangesAsync();
                return data.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new AppException(e.Message);
            }
        }

        public async Task<Guid> Update(Guid id, RoleAssignmentUpdateModel model)
        {
            try
            {
                var data = await GetRoleAssignment(id);
                if (data == null)
                {
                    throw new AppException(ErrorMessage.IdNotExist);
                }
                var updateData = _mapper.Map(model, data);
                _dataContext.RoleAssignments.Update(updateData);
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
            var data = await GetRoleAssignment(id);
            if (data == null)
            {
                throw new AppException(ErrorMessage.IdNotExist);
            }
            data.IsDeleted = true;
            _dataContext.RoleAssignments.Update(data);
            await _dataContext.SaveChangesAsync();
            return data.Id;
        }


        // private method
        private async Task<RoleAssignment> GetRoleAssignment(Guid id)
        {
            try
            {
                var data = await _dataContext
                    .RoleAssignments
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
