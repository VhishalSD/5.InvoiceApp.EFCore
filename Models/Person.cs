
using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.EFCore.Models;

// Represents a person with basic details and validation attributes
public class Person
{
    // Primary key
    public int Id { get; set; }

    // Full name, required
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = "";

    // Email address with built-in email format validation
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = "";

    // Date of birth
    public DateTime DateOfBirth { get; set; }
}
