using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UE.PostOffice.Api.Model;

public class DespatchDateRequest
{
    /// <summary>
    /// The list of product IDs to use in calculating the estimated despatch date
    /// </summary>
    /// <example>[1, 49, 62342]</example>
    [Required(ErrorMessage = "At least one product ID is required")]
    public List<int> ProductIds { get; set; }
    
    /// <summary>
    /// The date of when the order was placed
    /// </summary>
    /// <example>2025-06-14T09:00:00Z</example>
    [Required]
    public DateTime OrderDate { get; set; }
}