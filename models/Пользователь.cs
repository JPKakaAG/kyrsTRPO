using System;
using System.Collections.Generic;

namespace kyrsTRPO.models;

public partial class Пользователь
{
    public int Id { get; set; }

    public string Логин { get; set; } = null!;

    public string Пароль { get; set; } = null!;
}
