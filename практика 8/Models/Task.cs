using System;
using System.Collections.Generic;

namespace практика_8.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string OwnerLogin { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
