using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UE.PostOffice.Api.Model;

public class DespatchDateRequest
{
    [Required(ErrorMessage = "At least one product ID is required")]
    public List<int> ProductIds { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; }
}