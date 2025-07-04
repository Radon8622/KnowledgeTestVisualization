using System;
using System.Collections.Generic;

namespace KnowledgeTestVisualization.EF;

public partial class UserAccount
{
    public int Id { get; set; }

    public int? LecturerId { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Lecturer? Lecturer { get; set; }
}
