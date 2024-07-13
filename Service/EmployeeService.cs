using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeLinks _employeeLinks;


        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, 
            IMapper mapper, IEmployeeLinks employeeLinks)
        { 
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _employeeLinks = employeeLinks;
        }

        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyByIdAsync(companyId, trackChanges);
            //check to ensure we don't try to find employees from a non-existent company
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists
            (Guid companyId, Guid id, bool trackChanges)
        {
            var employeeFromDb = await _repository.Employee.GetEmployeeByIdAsync(companyId, id, trackChanges);
            // tracking changes so once a property in the entity changes, EF core will set its state to Modified
            if (employeeFromDb is null)
                throw new EmployeeNotFoundException(id);
            return employeeFromDb;
        }

        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync
        (Guid companyId, LinkParameters linkParameters, bool trackChanges)
        {
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();
            //var company = await _repository.Company.GetCompanyByIdAsync(companyId, trackChanges);
            ////check to ensure we don't try to find employees from a non-existent company
            //if (company is null) 
            //    throw new CompanyNotFoundException(companyId);

            await CheckIfCompanyExists(companyId, trackChanges);
            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId,
                linkParameters.EmployeeParameters, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
            var links = _employeeLinks.TryGenerateLinks(employeesDto, 
                linkParameters.EmployeeParameters.Fields,
                companyId, linkParameters.Context);

            return (linkResponse: links, metaData: employeesWithMetaData.MetaData);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeFromDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            trackChanges);
            var employee = _mapper.Map<EmployeeDto>(employeeFromDb);
            return employee;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation,
            bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employeeForCreation); // map the user's request i.e. employeeForCreation DTO to an employee entity
            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity); // create an employee in the repository
            await _repository.SaveAsync(); // save the employee to the DB

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity); // map the entity back to the employee DTO
            return employeeToReturn; // return the employee DTO to the API caller
        }
        
        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeFromDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId,
            trackChanges);
            _repository.Employee.DeleteEmployee(employeeFromDb);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdate,
            bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employeeFromDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId,
            empTrackChanges);

            _mapper.Map(employeeForUpdate, employeeFromDb); // map the employeeForUpdate DTO to the employee entity.
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, 
            Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employeeFromDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            empTrackChanges);

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeFromDb); //maps the employee entity to the EmployeeForUpdate interface
            return (employeeToPatch, employeeFromDb);
        }

        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }
    }
}
