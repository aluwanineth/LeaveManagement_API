﻿using Microsoft.AspNetCore.Identity;

namespace LeaveManagement.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
}
