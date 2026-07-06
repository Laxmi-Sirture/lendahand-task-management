using AutoMapper;
using LendAHand.Application.DTOs;
using LendAHand.Application.Interfaces.Repositories;
using LendAHand.Application.Interfaces.Services;
using LendAHand.Domain.Entities;
using LendAHand.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendAHand.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDTO<EmployeeDTO>> GetAllAsync(
            int page, int pageSize,
            string? search, string? sortBy, bool ascending)
        {
            var (employees, totalCount) = await _unitOfWork.Employees
                .GetPagedAsync(page, pageSize, search, sortBy, ascending);

            return new PagedResultDTO<EmployeeDTO>
            {
                Data = _mapper.Map<List<EmployeeDTO>>(employees),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<EmployeeDTO?> GetByIdAsync(Guid id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException(nameof(Employee), id);

            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<EmployeeDTO> CreateAsync(CreateEmployeeDTO dto)
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ValidationException("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.Name,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Domain.Enums.UserRole.Employee,
                CreatedAt = DateTime.UtcNow
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = dto.Name,
                Email = dto.Email.ToLower(),
                Department = dto.Department,
                Designation = dto.Designation,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<EmployeeDTO> UpdateAsync(Guid id, UpdateEmployeeDTO dto)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException(nameof(Employee), id);

            employee.Name = dto.Name;
            employee.Email = dto.Email.ToLower();
            employee.Department = dto.Department;
            employee.Designation = dto.Designation;
            employee.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException(nameof(Employee), id);

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<EmployeeDTO?> GetByUserIdAsync(Guid userId)
        {
            var employee = await _unitOfWork.Employees.GetByUserIdAsync(userId);
            if (employee == null) return null;
            return _mapper.Map<EmployeeDTO>(employee);
        }
    }
}
