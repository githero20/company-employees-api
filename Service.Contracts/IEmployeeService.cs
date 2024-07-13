using Entities.Models;
using Entities.LinkModels;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        // returns a tuple of 2 fields: employees, metadata
        Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId, 
            LinkParameters linkParameters, bool trackChanges);
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid employeeId, Guid id, bool trackChanges);
        Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto 
            employeeForCreation, bool trackChanges);
        Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges);
        Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto
            employeeForUpdate, bool compTrackChanges, bool empTrackChanges);
        // we have broken patch into two
        // first, we have to get the employee information that is to be updated
        // then, we save the changes into the DB
        Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
            Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
        Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee
            employeeEntity);
    }
}
