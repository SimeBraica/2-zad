using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Pet
{
    public int PetId { get; set; }

    public string? PetName { get; set; }

    public string? Animal { get; set; }

    public int OwnerId { get; set; }

    public virtual Owner Owner { get; set; } = null!;
}
