using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace HR.DTO
{
    public class DepartmentLeaveSummaryDTO
    {
        public string Department {  get; set; }
        public int TotalLeaves { get; set; }

        public List<string> EmployeesOnLeave { get; set; }

        public bool HasConflict { get; set; }

    }
}
